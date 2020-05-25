using System.Threading.Tasks;
using System.Collections.Generic;

using Chat.Dto.Input;
using Chat.Dto.Output;

namespace Chat
{
    public class ChatClient
    {
        public string ServerUrl { get; set; }
        private Connector Connector = new Connector();

        public ChatClient(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        private const string CREATE_CHAT_ENDPOINT = "chat/create";
        public async Task<ChatInfoDto> CreateChat(CreateChatDto createDto)
        {
            return await Connector.SendPost<CreateChatDto, ChatInfoDto>(
                ServerUrl + CREATE_CHAT_ENDPOINT, createDto
            );
        }

        private const string JOIN_CHAT_ENDPOINT = "chat/join";
        public async Task<ChatInfoDto> JoinChat(JoinChatDto joinDto)
        {
            return await Connector.SendPost<JoinChatDto, ChatInfoDto>(
                ServerUrl + JOIN_CHAT_ENDPOINT, joinDto
            );
        }

        private const string GET_CHAT_ENDPOINT = "chat/info";
        public async Task<ChatInfoDto> GetChatInfo(string chatName)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("name", chatName);

            return await Connector.SendGet<ChatInfoDto>(
                ServerUrl + GET_CHAT_ENDPOINT, parameters
            );
        }

        private const string GET_CHAT_LIST_ENDPOINT = "chat/list";
        public async Task<IEnumerable<ChatInfoDto>> GetChats()
        {
            return await Connector.SendGet<IEnumerable<ChatInfoDto>>(
                ServerUrl + GET_CHAT_LIST_ENDPOINT
            );
        }

        private const string OPEN_CHAT_ENDPOINT = "chat/open";
        public async Task<ChatDto> OpenChat(string chatName, string userName)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("name", chatName);
            parameters.Add("reader", userName);

            return await Connector.SendGet<ChatDto>(
                ServerUrl + OPEN_CHAT_ENDPOINT,
                parameters
            );
        }

        private const string WRITE_MESSAGE_ENDPOINT = "chat/write";
        public async Task<MessageDto> WriteMessage(WriteMessageDto messageDto)
        {
            return await Connector.SendPost<WriteMessageDto, MessageDto>(
                ServerUrl + WRITE_MESSAGE_ENDPOINT, messageDto
            );
        }
    }
}
