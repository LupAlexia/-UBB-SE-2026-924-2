using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Dto;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class FAQNavigationData
    {
        public int CurrentPersonId { get; set; }
        public bool IsEmployee { get; set; }
        public FAQEntryDTO? FAQEntry { get; set; }
    }
}
