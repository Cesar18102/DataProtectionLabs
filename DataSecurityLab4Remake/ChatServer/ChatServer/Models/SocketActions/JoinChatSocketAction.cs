using ChatServer.Dto.Output;

namespace ChatServer.Models.SocketActions
{
    public class JoinChatSocketAction : SocketActionBase
    {
        public JoinChatSocketAction(MemberDto member)
        {
            Action = "JOIN";
            Data = member;
        }
    }
}