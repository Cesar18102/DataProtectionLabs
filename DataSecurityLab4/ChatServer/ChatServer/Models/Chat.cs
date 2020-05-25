using System;
using System.Collections.Generic;

namespace ChatServer.Models
{
    public class Chat : IEquatable<Chat>
    {
        public string Name { get; private set; }
        public Member Creator { get; private set; }

        public ICollection<Member> Members { get; private set; } = new List<Member>();
        public ICollection<Message> Messages { get; private set; } = new List<Message>();

        public int? P { get; private set; }
        public int? G { get; private set; }

        public Chat(string name, Member creator)
        {
            Name = name;
            Creator = creator;
            Members.Add(creator);
        }

        public void AssignKeys(int p, int g)
        {
            P = p;
            G = g;
        }

        public bool Equals(Chat other)
        {
            return other.Name == Name;
        }
    }
}