namespace AirportApp.Mvc.Models.Chat
{
    public class ChatDetailViewModel
    {
        public int Id { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public string UserFullName { get; set; } = string.Empty;

        public List<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();
    }
}
