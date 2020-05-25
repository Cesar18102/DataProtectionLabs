using System;
using System.Collections.Generic;

namespace ChatServer.Models
{
    public class Message
    {
        public IEnumerable<EncodedMessageText> TextTable { get; private set; }
        public Member Writer { get; private set; }
        public DateTime TimeSpan { get; private set; }

        public Message(IEnumerable<EncodedMessageText> textTable , Member writer, DateTime timeSpan)
        {
            Writer = writer;
            TimeSpan = timeSpan;
            TextTable = textTable;
        }
    }
}