using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.Src.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.ViewModel.Chats
{
    public sealed partial class ChatViewModel : ObservableObject
    {
        public ObservableCollection<FAQOption> CurrentOptions { get; } = new ();
        public ObservableCollection<MessageDTO> ChatHistory { get; } = new ();

        private IMessageService messageService;
        private IChatService chatService;
        private IUserService userService;
        private IMapper mapper;
        private Chat chat;
        private User user;
        private const int FIRST_OPTION = 1;

        public ChatViewModel(IMessageService msgService, IChatService chatService, IMapper mapper, IUserService userService, User testUser = null)
        {
            messageService = msgService;
            this.chatService = chatService;
            this.mapper = mapper;
            this.userService = userService;

            // uses the injected user for tests, otherwise fallback to App.Current
            user = testUser ?? (App.Current as App)?.User;

            if (user == null)
            {
                return;
            }

            _ = InitializeChatAsync();
        }

        private async Task InitializeChatAsync()
        {
            chat = await this.chatService.OpenChatAsync(user.RetrieveUniqueDatabaseIdentifierForBot());

            await LoadChatHistoryAsync();

            if (ChatHistory.Count == 0)
            {
                await LoadFirstMessageAsync();
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
            ChatHistory.Clear();
            var messages = await messageService.GetAllMessagesAsync(chat.Id);
            var currentUserId = user.RetrieveUniqueDatabaseIdentifierForBot();
            foreach (var message in messages)
            {
                var dataTransferObject = mapper.Map<MessageDTO>(message);

                if (dataTransferObject.SenderId == BotEngineIdentity.CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER)
                {
                    dataTransferObject.SenderName = "Carlos";
                }
                else
                {
                    var senderUser = await userService.GetByIdAsync(dataTransferObject.SenderId);
                    dataTransferObject.SenderName = senderUser?.RetrieveConfiguredDisplayFullNameForBot();
                }

                dataTransferObject.IsOutgoing = (dataTransferObject.SenderId == currentUserId);
                ChatHistory.Add(dataTransferObject);
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
            UpdateAvailableOptions(botReply);
        }

        private void UpdateAvailableOptions(BotMessage botReply)
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
                CurrentOptions.Add(new FAQOption("Restart Chat", FIRST_OPTION));
            }
        }

        private async Task LoadFirstMessageAsync()
        {
            await HandleOptionClickAsync(new FAQOption("Hello! I need help.", FIRST_OPTION));
        }
    }
}
