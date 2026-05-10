using System;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CreateMessageDTO(
        int chatId,
        string text,
        int senderId,
        DateTimeOffset timestamp);
}
