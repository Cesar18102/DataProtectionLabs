using System;
using System.Collections.Generic;

namespace ChatServer.Models
{
    public class Chat : IEquatable<Chat>
    {
        public string Name { get; private set; }

        public int? P { get; private set; }
        public int? G { get; private set; }

        public Member Creator { get; private set; }
        public IDictionary<Member, Notifier> Members { get; private set; } = new Dictionary<Member, Notifier>();

        public Chat(string name, Member creator, Notifier notifier)
        {
            Name = name;
            Creator = creator;
            Members.Add(creator, notifier);
        }

        public void AssignKeys(int p, int g)
        {
            P = p;
            G = g;
        }

        public bool Equals(Chat other)
        {
            return other.Name == Name;
        }//?
    }
}