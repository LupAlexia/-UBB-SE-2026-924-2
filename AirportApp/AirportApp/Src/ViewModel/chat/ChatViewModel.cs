using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.Src.Dto;
using AirportApp.Src.Model;
using AirportApp.Src.Model.Chats;
using AirportApp.Src.Model.Faq.Bot;
using AirportApp.Src.Model.Message;
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

        private MessageService messageService;
        private ChatService chatService;
        private IUserService userService;
        private IMapper mapper;
        private Chat chat;
        private User user;
        private const int FIRST_OPTION = 1;
        public ChatViewModel(MessageService msgService, ChatService chatService, IMapper mapper, IUserService userService, User testUser = null)
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

            chat = this.chatService.OpenChat(user.RetrieveUniqueDatabaseIdentifierForBot());

            LoadChatHistory();

            if (ChatHistory.Count == 0)
            {
                LoadFirstMessage();
            }
        }

        public string FormatUserId => "User Id: " + user.RetrieveUniqueDatabaseIdentifierForBot().ToString();
        public void CloseChat()
        {
            chatService.CloseChat(chat.ChatId);
        }
        private void LoadChatHistory()
        {
            ChatHistory.Clear();
            var messages = messageService.GetAllMessages(chat.ChatId);
            var currentUserId = user.RetrieveUniqueDatabaseIdentifierForBot();
            foreach (var message in messages)
            {
                var dateTime = mapper.Map<MessageDTO>(message);
                dateTime.SenderName = userService.GetById(dateTime.SenderId)?.RetrieveConfiguredDisplayFullNameForBot();
                dateTime.IsOutgoing = (dateTime.SenderId == currentUserId);
                ChatHistory.Add(dateTime);
            }
        }

        [RelayCommand]
        private void HandleOptionClick(FAQOption option)
        {
            if (option == null)
            {
                return;
            }

            BotMessage botReply = messageService.SendMessage(chat.ChatId, user, option);
            System.Diagnostics.Debug.WriteLine($"User selected: {option.label}");

            LoadChatHistory();
            UpdateAvailableOptions(botReply);
        }

        private void UpdateAvailableOptions(BotMessage botReply)
        {
            CurrentOptions.Clear();
            var nextOptions = (botReply as IMessage).GetNextOptions();

            // var dto = _mapper.Map<MessageDTO>();
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

        private void LoadFirstMessage()
        {
            HandleOptionClick(new FAQOption("Hello! I need help.", FIRST_OPTION));
            // _messageService.SendMessage(_chat.ChatId, _user, new FAQOption("Hello! I need help.", _FIRST_OPTION));
        }
    }
}
