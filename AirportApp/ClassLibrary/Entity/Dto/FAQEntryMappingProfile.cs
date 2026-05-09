using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Tar;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class FAQEntryMappingProfile : Profile
    {
        public FAQEntryMappingProfile()
        {
            CreateMap<FAQEntry, FAQEntryDTO>()
                .ConstructUsing(sourceEntity => new FAQEntryDTO(
                    sourceEntity.Id,
                    sourceEntity.Question,
                    sourceEntity.Answer,
                    sourceEntity.Category,
                    sourceEntity.ViewCount,
                    sourceEntity.HelpfulVotesCount,
                    sourceEntity.NotHelpfulVotesCount));

            CreateMap<FAQEntryDTO, FAQEntry>()
                .ConstructUsing(sourceEntity => new FAQEntry(
                    sourceEntity.Id,
                    sourceEntity.Question,
                    sourceEntity.Answer,
                    sourceEntity.Category,
                    sourceEntity.ViewCount,
                    sourceEntity.HelpfulVotesCount,
                    sourceEntity.NotHelpfulVotesCount));
    }
    }
}
