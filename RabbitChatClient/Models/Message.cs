using System;

namespace RabbitChatClient.Models;

public class Message
{
    public int RabbitUserId { get; set; }
    
    public DateTime TimeSent { get; set; }
    
    public string MessageContent { get; set; }
    
    public int RoomId { get; set; }
}