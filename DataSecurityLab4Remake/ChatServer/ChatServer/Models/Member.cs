using System;

using ChatServer.Dto.Input;

namespace ChatServer.Models
{
    public class Member : IEquatable<Member>
    {
        public string Name { get; private set; }
        public string Endpoint { get; private set; }
        public int Port { get; private set; }

        public Member(CreateChatDto dto)
        {
            Name = dto.Creator.Name;
            Endpoint = dto.Creator.Endpoint;
            Port = dto.Creator.Port;
        }

        public Member(JoinChatDto dto)
        {
            Name = dto.Member.Name;
            Endpoint = dto.Member.Endpoint;
            Port = dto.Member.Port;
        }

        public bool Equals(Member other)
        {
            return other.Name == Name;
        }//?
    }
}