using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ConstructUsing(user => new UserDTO(
                    user.RetrieveConfiguredDisplayFullNameForBot(),
                    user.RetrieveConfiguredEmailAddressForBotContact())).ForAllMembers(option => option.Ignore());
        }
    }
}
