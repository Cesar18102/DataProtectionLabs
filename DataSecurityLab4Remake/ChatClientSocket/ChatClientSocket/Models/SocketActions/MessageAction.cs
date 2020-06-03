namespace ChatClientSocket.Models.SocketActions
{
    public class MessageAction : SocketActionBase
    {
        public MessageAction(Message message)
        {
            Action = SocketActions.MESSAGE;
            Data = message;
        }
    }
}
