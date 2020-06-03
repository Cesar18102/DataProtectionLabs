using System.Threading.Tasks;
using System.Collections.Generic;

using ChatClientSocket.Dto.Input;
using ChatClientSocket.Dto.Output;

namespace ChatClientSocket
{
    public class ChatClient
    {
        public string ServerUrl { get; set; }
        private Connector Connector = new Connector();

        public ChatClient(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        private const string GET_ADDRESS_ENDPOINT = "chat/getserveraddress";
        public async Task<string> GetServerAddress()
        {
            return await Connector.SendGet<string>(
                ServerUrl + GET_ADDRESS_ENDPOINT
            );
        }

        private const string CREATE_CHAT_ENDPOINT = "chat/create";
        public async Task<ChatDto> CreateChat(CreateChatDto createDto)
        {
            return await Connector.SendPost<CreateChatDto, ChatDto>(
                ServerUrl + CREATE_CHAT_ENDPOINT, createDto
            );
        }

        private const string JOIN_CHAT_ENDPOINT = "chat/join";
        public async Task<ChatDto> JoinChat(JoinChatDto joinDto)
        {
            return await Connector.SendPost<JoinChatDto, ChatDto>(
                ServerUrl + JOIN_CHAT_ENDPOINT, joinDto
            );
        }

        private const string GET_CHAT_ENDPOINT = "chat/info";
        public async Task<ChatDto> GetChatInfo(string chatName)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("name", chatName);

            return await Connector.SendGet<ChatDto>(
                ServerUrl + GET_CHAT_ENDPOINT, parameters
            );
        }

        private const string GET_CHAT_LIST_ENDPOINT = "chat/list";
        public async Task<IEnumerable<string>> GetChats()
        {
            return await Connector.SendGet<IEnumerable<string>>(
                ServerUrl + GET_CHAT_LIST_ENDPOINT
            );
        }
    }
}
