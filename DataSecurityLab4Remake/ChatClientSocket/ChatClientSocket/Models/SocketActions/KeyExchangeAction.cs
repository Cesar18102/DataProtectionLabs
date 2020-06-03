using ChatClientSocket.Dto.Output;

namespace ChatClientSocket.Models.SocketActions
{
    public class KeyExchangeAction : SocketActionBase
    {
        public KeyExchangeAction(KeyExchangeDto dto)
        {
            Action = SocketActions.KEY_UPDATE;
            Data = dto;
        }
    }
}
