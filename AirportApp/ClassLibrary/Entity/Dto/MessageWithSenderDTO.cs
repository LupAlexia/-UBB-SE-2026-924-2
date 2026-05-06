using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class MessageWithSenderDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int ChatId { get; set; }
        public int? SenderUserId { get; set; }
        public int? SenderEmployeeId { get; set; }
        public int SenderId => SenderUserId ?? 0;
    }
}
