using System;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Entity.Dto.MappingProfiles
{
    public class MessageMappingProfile : Profile
    {
        public MessageMappingProfile()
        {
            CreateMap<IMessage, MessageDTO>()
                .ForMember(destination => destination.MessageText, option => option.MapFrom(source => source.Text))
                .ForMember(destination => destination.Timestamp, option => option.MapFrom(source =>
                    new DateTimeOffset(((IMessage)source).Timestamp.Ticks, TimeSpan.Zero)))
                .ForMember(destination => destination.FaqOptions, option => option.MapFrom(source => source.GetNextOptions()))
                .ForMember(destination => destination.ChatId, option => option.MapFrom(source => source.GetChat().Id))
                .ForMember(destination => destination.SenderName, option => option.MapFrom(source => source.GetSender().RetrieveConfiguredDisplayFullNameForBot()))
                .ForMember(destination => destination.SenderId, option => option.MapFrom(source => source.GetSender().RetrieveUniqueDatabaseIdentifierForBot()))
                .ForMember(destination => destination.MessageId, option => option.Ignore())
                .ForMember(destination => destination.IsOutgoing, option => option.Ignore());

            CreateMap<BotMessage, MessageDTO>()
                .IncludeBase<IMessage, MessageDTO>()
                .ForMember(destination => destination.SenderId, option => option.MapFrom(source => BotEngineIdentity.CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER));

            CreateMap<Message, MessageDTO>()
                .IncludeBase<IMessage, MessageDTO>()
                .ForMember(destination => destination.SenderId,
                    option => option.MapFrom(source => source.SenderUserId ?? 0));
        }
    }
}