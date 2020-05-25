using System;

namespace Chat.Models
{
    public class DecodedMessage
    {
        public string Text { get; private set; }
        public string Sender { get; private set; }
        public DateTime TimeSpan { get; private set; }

        public DecodedMessage(string text, string sender, DateTime timeSpan)
        {
            Text = text;
            Sender = sender;
            TimeSpan = timeSpan;
        }
    }
}
