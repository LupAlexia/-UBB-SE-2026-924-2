using System;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class MessageMappingProfile : Profile
    {
        public MessageMappingProfile()
        {
            CreateMap<IMessage, MessageDTO>()
                .ForMember(destination => destination.MessageText, option => option.MapFrom(source => source.Text))
                .ForMember(destination => destination.Timestamp, option => option.MapFrom(source =>
                    new DateTimeOffset(source.Timestamp.Ticks, TimeSpan.Zero)))
                .ForMember(destination => destination.FaqOptions, option => option.MapFrom(source => source.GetNextOptions()))
                .ForMember(destination => destination.ChatId, option => option.MapFrom(source => source.GetChat().Id))
                .ForMember(destination => destination.SenderId, option => option.MapFrom(source => source.GetSender().RetrieveUniqueDatabaseIdentifierForBot()))
                .ForMember(destination => destination.Sender, option => option.MapFrom(source => source.GetSender()))
                .ForMember(destination => destination.MessageId, option => option.Ignore())
                .ForMember(destination => destination.IsOutgoing, option => option.Ignore());

            CreateMap<BotMessage, MessageDTO>()
                .IncludeBase<IMessage, MessageDTO>()
                .ForMember(destination => destination.Sender, option => option.MapFrom(source => source.GetSender()));

            CreateMap<Message, MessageDTO>()
                .IncludeBase<IMessage, MessageDTO>()
                .ForMember(destination => destination.Sender,
                    option => option.MapFrom(source => source.GetSender()));
        }
    }
}