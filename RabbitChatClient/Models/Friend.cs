namespace RabbitChatClient.Models;

public class Friend
{
    public string UserName { get; set; }

    public Friend(string userName)
    {
        UserName = userName;
    }
}