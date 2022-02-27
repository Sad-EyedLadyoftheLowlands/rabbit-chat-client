namespace RabbitChatClient.Models.Requests
{
    internal class CreateMessageRequest
    {
        public int SendingUserId { get; set; }
        public int RoomId { get; set; }
        public string Content { get; set; }
    }
}