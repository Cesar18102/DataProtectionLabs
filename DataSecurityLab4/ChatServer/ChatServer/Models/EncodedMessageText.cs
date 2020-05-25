namespace ChatServer.Models
{
    public class EncodedMessageText
    {
        public Member Reciever { get; private set; }
        public string EncodedText { get; private set; }

        public EncodedMessageText(string encodedText, Member reciever)
        {
            Reciever = reciever;
            EncodedText = encodedText;
        }
    }
}