using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AutoMapper;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class ChatMappingProfile : Profile
    {
        public ChatMappingProfile()
        {
            CreateMap<Chat, ChatDTO>()
                .ConstructUsing(chat => new ChatDTO(
                    chat.Id,
                    chat.UserId,
                    chat.Status,
                    chat.MessageCount()))
                .ForAllMembers(options => options.Ignore());
        }
    }
}
