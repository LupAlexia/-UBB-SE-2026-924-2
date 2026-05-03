using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.ClassLibrary.Entity.Dto.MappingProfiles
{
    public class TicketMappingProfile : Profile
    {
        public TicketMappingProfile()
        {
            CreateMap<Ticket, TicketDTO>()
                .ConstructUsing(ticket => new TicketDTO(
                    ticket.TicketId,
                    ticket.Creator.UserId,
                    ticket.Creator.RetrieveConfiguredEmailAddressForBotContact(),
                    ticket.UrgencyLevel,
                    ticket.CurrentStatus,
                    ticket.Category.CategoryId,
                    ticket.Category.CategoryName,
                    ticket.Subcategory.SubcategoryId,
                    ticket.Subcategory.SubcategoryName,
                    ticket.Subject,
                    ticket.Description,
                    ticket.CreationTimestamp));
        }
    }
}
