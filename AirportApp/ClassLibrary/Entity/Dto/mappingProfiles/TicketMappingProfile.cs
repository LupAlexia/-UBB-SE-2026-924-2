using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto.MappingProfiles
{
    public class TicketMappingProfile : Profile
    {
        public TicketMappingProfile()
        {
            CreateMap<ComplaintTicket, TicketDTO>()
                .ConstructUsing(ticket => new TicketDTO(
                    ticket.Id,
                    ticket.Creator.UserId,
                    ticket.Creator.RetrieveConfiguredEmailAddressForBotContact(),
                    ticket.UrgencyLevel,
                    ticket.CurrentStatus,
                    ticket.Category.Id,
                    ticket.Category.CategoryName,
                    ticket.Subcategory.Id,
                    ticket.Subcategory.SubcategoryName,
                    ticket.Subject,
                    ticket.Description,
                    ticket.CreationTimestamp));
        }
    }
}
