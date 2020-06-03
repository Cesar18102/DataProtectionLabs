using System.Linq;
using System.Collections.Generic;

using System.Net;
using System.Net.Http;
using System.Net.Sockets;

using System.Web.Http;

using ChatServer.Dto.Input;
using ChatServer.Dto.Output;
using ChatServer.Dto.Output.Exceptions;

using ChatServer.Models;
using ChatServer.Models.SocketActions;

namespace ChatServer.Controllers
{
    public class ChatController : ControllerBase
    {
        private static readonly ICollection<Chat> Chats = new List<Chat>();

        private Chat GetChatByName(string name)
        {
            Chat found = Chats.FirstOrDefault(chat => chat.Name == name);

            if (found == null)
                throw new NotFoundException("chat");

            return found;
        }

        private Member GetChatMemberByName(Chat chat, string name)
        {
            return chat.Members.FirstOrDefault(member => member.Key.Name == name).Key;
        }

        private static string GetAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(hostName);
            return host.AddressList[0].ToString();
        }

        [HttpGet]
        public HttpResponseMessage GetServerAddress()
        {
            return ProtectedExecuteAndWrapResult(() => GetAddress());
        }

        [HttpPost]
        public HttpResponseMessage Create([FromBody] CreateChatDto createDto)
        {
            return ProtectedExecuteAndWrapResult(() =>
            {
                if (Chats.FirstOrDefault(chat => chat.Name == createDto.ChatName) != null)
                    throw new ConflictException("chat name");

                Member creator = new Member(createDto);
                Notifier notifier = new Notifier(creator.Endpoint, creator.Port);
                Chat created = new Chat(createDto.ChatName, creator, notifier);

                created.AssignKeys(createDto.P.Value, createDto.G.Value);
                Chats.Add(created);

                return new ChatDto(created);
            }, createDto);
        }

        [HttpPost]
        public HttpResponseMessage Join([FromBody] JoinChatDto joinDto)
        {
            return ProtectedExecuteAndWrapResult(() =>
            {
                Chat chat = GetChatByName(joinDto.ChatName);

                if (GetChatMemberByName(chat, joinDto.Member.Name) != null)
                    throw new ConflictException("chat member");

                Member chatMember = new Member(joinDto);
                MemberDto chatMemberDto = new MemberDto(chatMember);

                Notifier joinedNotifier = new Notifier(chatMember.Endpoint, chatMember.Port);
                chat.Members.Add(chatMember, joinedNotifier);

                foreach (KeyValuePair<Member, Notifier> pair in chat.Members)
                {
                    SocketActionBase action = new JoinChatSocketAction(chatMemberDto);

                    try { pair.Value.Send(action); }
                    catch (SocketException ex) { chat.Members.Remove(pair.Key); }
                }

                return new ChatDto(chat);
            }, joinDto);
        }

        public void Leave()
        {
            //?
        }

        [HttpGet]
        public HttpResponseMessage Info(string name)
        {
            return ProtectedExecuteAndWrapResult(() =>
            {
                Chat chat = GetChatByName(name);
                return new ChatDto(chat);
            }, name);
        }

        [HttpGet]
        public HttpResponseMessage List()
        {
            return ProtectedExecuteAndWrapResult(
                () => Chats.Select(chat => chat.Name)
            );
        }
    }
}
