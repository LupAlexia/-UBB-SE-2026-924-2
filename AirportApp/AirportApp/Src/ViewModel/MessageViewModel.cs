using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Src.ViewModel
{
    public partial class MessageViewModel : ObservableObject
    {
        private readonly IMessageService messageService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        private readonly int chatId;
        private readonly int currentUserId;

        public ObservableCollection<MessageDTO> Messages { get; } = new ();

        public MessageViewModel(
            IMessageService messageService,
            IUserService userService,
            IMapper mapper,
            int chatId,
            int currentUserId)
        {
            this.messageService = messageService;
            this.userService = userService;
            this.mapper = mapper;
            this.chatId = chatId;
            this.currentUserId = currentUserId;

            _ = LoadMessagesAsync();
        }

        public async Task LoadMessagesAsync()
        {
            var messagesFromDb = await messageService.GetAllMessagesAsync(chatId);
            Messages.Clear();

            foreach (var message in messagesFromDb)
            {
                Messages.Add(mapper.Map<MessageDTO>(message));
            }
        }

        [RelayCommand]
        public async Task SendMessageAsync(FAQOption selectedOption)
        {
            if (selectedOption == null)
            {
                throw new ArgumentNullException(nameof(selectedOption));
            }

            // Lazily resolve the current user only when needed.
            var sender = await userService.GetByIdAsync(currentUserId);

            BotMessage botReply = await messageService.SendMessageAsync(chatId, sender, selectedOption);

            Messages.Add(mapper.Map<MessageDTO>(new Message(sender, botReply.GetChat(), selectedOption.Label)));
            Messages.Add(mapper.Map<MessageDTO>(botReply));
        }
    }
}