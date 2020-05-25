using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;

using ChatServer.Models;
using ChatServer.Dto.Input;
using ChatServer.Dto.Output;
using ChatServer.Dto.Output.Exceptions;

namespace ChatServer.Controllers
{
    public class ChatController : ControllerBase
    {
        private static readonly ICollection<Chat> Chats = new List<Chat>();

        [HttpPost]
        public HttpResponseMessage Create([FromBody] CreateChatDto createDto)
        {
            return ProtectedExecuteAndWrapResult(() =>
            {
                if (Chats.FirstOrDefault(chat => chat.Name == createDto.Name) != null)
                    throw new ConflictException("chat name");

                Member creator = new Member(createDto.CreatorName, createDto.PublicKey.Value);
                Chat created = new Chat(createDto.Name, creator);

                created.AssignKeys(createDto.P.Value, createDto.G.Value);
                Chats.Add(created);

                return new ChatInfoDto(created);
            }, createDto);
        }

        private Chat GetChatByName(string name)
        {
            Chat found = Chats.FirstOrDefault(chat => chat.Name == name);

            if (found == null)
                throw new NotFoundException("chat");

            return found;
        }

        private Member GetChatMemberByName(Chat chat, string name)
        {
            return chat.Members.FirstOrDefault(member => member.Name == name);
        }

        [HttpPost]
        public HttpResponseMessage Join([FromBody] JoinChatDto joinDto)
        {
            return ProtectedExecuteAndWrapResult(() =>
            {
                Chat chat = GetChatByName(joinDto.ChatName);

                if (GetChatMemberByName(chat, joinDto.MemberName) != null)
                    throw new ConflictException("chat member");

                Member chatMember = new Member(joinDto.MemberName, joinDto.PublicKey.Value);
                chat.Members.Add(chatMember);

                return new ChatInfoDto(chat);
            }, joinDto);
        }

        private MessageDto GetMessageEncodedForSpecifiedUser(Member user, Message message)
        {
            EncodedMessageText messageEncoded = message.TextTable.FirstOrDefault(msg => msg.Reciever.Equals(user));
            string text = messageEncoded == null ? "******" : messageEncoded.EncodedText;
            return new MessageDto(text, new MemberDto(message.Writer), message.TimeSpan);
        }

        [HttpPost]
        public HttpResponseMessage Write([FromBody] WriteMessageDto messageDto)
        {
            return ProtectedExecuteAndWrapResult(() =>
            {
                Chat chat = GetChatByName(messageDto.ChatName);
                Member sender = GetChatMemberByName(chat, messageDto.SenderName);

                if (sender == null)
                    throw new NotFoundException("chat member");

                ICollection<EncodedMessageText> messageTable = new List<EncodedMessageText>();
                foreach(EncodedMessageDto message in messageDto.MessageTable)
                {
                    Member reciever = GetChatMemberByName(chat, message.RecieverName);

                    if (reciever == null)
                        continue;

                    messageTable.Add(new EncodedMessageText(message.EncodedText, reciever));
                }

                Message result = new Message(messageTable, sender, DateTime.Now);
                chat.Messages.Add(result);

                return GetMessageEncodedForSpecifiedUser(sender, result);
            }, messageDto);
        }

        [HttpGet]
        public HttpResponseMessage Open(string name, string reader)
        {
            return ProtectedExecuteAndWrapResult(() =>
            {
                Chat chat = GetChatByName(name);
                Member chatMember = GetChatMemberByName(chat, reader);

                if (chatMember == null)
                    throw new NotFoundException("member");

                ChatDto result = new ChatDto(
                    chat.Name,
                    chat.Members.Select(member => new MemberDto(member)).ToList(),
                    chat.Messages.Select(message => GetMessageEncodedForSpecifiedUser(chatMember, message)).ToList()
                );

                return result;
            });
        }

        [HttpGet]
        public HttpResponseMessage Info(string name)
        {
            return ProtectedExecuteAndWrapResult(() =>
            {
                Chat chat = GetChatByName(name);
                return new ChatInfoDto(chat);
            }, name);
        }

        [HttpGet]
        public HttpResponseMessage List()
        {
            return ProtectedExecuteAndWrapResult(() => Chats.Select(chat => new ChatInfoDto(chat)));
        }
    }
}
