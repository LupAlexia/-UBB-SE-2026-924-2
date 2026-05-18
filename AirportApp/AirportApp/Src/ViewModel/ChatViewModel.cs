using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.ViewModel
{
    public sealed partial class ChatViewModel : ObservableObject
    {
        public ObservableCollection<FAQOption> CurrentOptions { get; } = new ();
        public ObservableCollection<MessageDTO> ChatHistory { get; } = new ();

        private IMessageService messageService;
        private IChatService chatService;
        private IUserService userService;
        private IDecisionTreeService decisionTreeService;
        private IMapper mapper;
        private Chat chat;
        private User user;
        private const int FirstOptionNodeId = 1;

        public ChatViewModel(IMessageService messageService, IChatService chatService, IMapper mapper, IUserService userService, IDecisionTreeService decisionTreeService, User testUser = null)
        {
            this.messageService = messageService;
            this.chatService = chatService;
            this.mapper = mapper;
            this.userService = userService;
            this.decisionTreeService = decisionTreeService;

            user = testUser ?? (App.Current as App)?.User;

            if (user == null)
            {
                return;
            }

            _ = InitializeChatAsync();
        }

        private async Task InitializeChatAsync()
        {
            try
            {
                chat = await this.chatService.OpenChatAsync(user);

                await LoadChatHistoryAsync();

                if (ChatHistory.Count == 0)
                {
                    await LoadFirstMessageAsync();
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeChatAsync error: {exception}");
            }
        }

        public string FormatUserId => "User Id: " + user.RetrieveUniqueDatabaseIdentifierForBot().ToString();

        public async Task CloseChatAsync()
        {
            if (chat != null)
            {
                await chatService.CloseChatAsync(chat.Id);
            }
        }

        private async Task LoadChatHistoryAsync()
        {
            try
            {
                ChatHistory.Clear();
                var messages = await messageService.GetAllMessagesAsync(chat.Id);
                var currentUserId = user.RetrieveUniqueDatabaseIdentifierForBot();
                foreach (var message in messages)
                {
                    var dataTransferObject = mapper.Map<MessageDTO>(message);
                    var senderId = dataTransferObject.Sender?.RetrieveUniqueDatabaseIdentifierForBot() ?? dataTransferObject.SenderId;

                    System.Diagnostics.Debug.WriteLine($"Message: {dataTransferObject.MessageText}, SenderId: {senderId}");
                    dataTransferObject.IsOutgoing = senderId == currentUserId;
                    ChatHistory.Add(dataTransferObject);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"LoadChatHistoryAsync error: {exception}");
            }
        }

        [RelayCommand]
        private async Task HandleOptionClickAsync(FAQOption option)
        {
            if (option == null)
            {
                return;
            }

            BotMessage botReply = await messageService.SendMessageAsync(chat.Id, user, option);
            System.Diagnostics.Debug.WriteLine($"User selected: {option.Label}");

            await LoadChatHistoryAsync();
            await UpdateAvailableOptionsAsync(botReply);
        }

        private async Task UpdateAvailableOptionsAsync(BotMessage botReply)
        {
            CurrentOptions.Clear();
            var nextOptions = (botReply as IMessage).GetNextOptions();

            if (nextOptions != null)
            {
                foreach (var option in nextOptions)
                {
                    CurrentOptions.Add(option);
                }
            }
            else
            {
                var restartNode = await decisionTreeService.GetNodeByIdAsync(FirstOptionNodeId);
                CurrentOptions.Add(new FAQOption("Restart Chat", restartNode));
            }
        }

        private async Task LoadFirstMessageAsync()
        {
            var firstNode = await decisionTreeService.GetNodeByIdAsync(FirstOptionNodeId);
            await HandleOptionClickAsync(new FAQOption("Hello! I need help.", firstNode));
        }
    }
}

