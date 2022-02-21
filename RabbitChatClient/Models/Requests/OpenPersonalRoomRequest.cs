namespace RabbitChatClient.Models.Requests;

public class OpenPersonalRoomRequest
{
    public int RequestUserId { get; set; }
    
    public int FriendId { get; set; }
}