using System;

namespace ChatServer.Models
{
    public class Member : IEquatable<Member>
    {
        public string Name { get; private set; }
        public int PublicKey { get; private set; }

        public Member(string name, int publicKey)
        {
            Name = name;
            PublicKey = publicKey;
        }

        public bool Equals(Member other)
        {
            return other.Name == Name;
        }
    }
}