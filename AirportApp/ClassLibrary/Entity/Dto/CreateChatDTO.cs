using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CreateChatDTO(int userId, ChatStatus status);
}
