using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using ChatClientSocket.Dto.Output;
using ChatClientSocket.Dto.Input;
using ChatClientSocket.Dto.Input.Exceptions;

using ChatClientSocket.Sockets;
using ChatClientSocket.Models.SocketActions;

using Crypto;
using DataSecurityPractice2;

using Newtonsoft.Json;
using ChatClientSocket.Models;

namespace ChatClientSocket
{
    public class Program
    {
        private static Listener LISTENER { get; set; }
        private static readonly ChatClient CHAT_CLIENT = new ChatClient("http://192.168.0.108:5000/api/");

        private static readonly DiffieHellmann DIFFIE = new DiffieHellmann();
        private static readonly DisposableNotepad NOTEPAD = new DisposableNotepad();

        private static MemberDto ME = new MemberDto();

        private static int CURRENT_CHAT_COMMON_KEY = -1;
        private static ChatDto CURRENT_CHAT { get; set; }

        private static IDictionary<string, ChatData> USER_CHAT_KEYS = new Dictionary<string, ChatData>();
        private static IDictionary<string, Notifier> USER_NOTIFIERS = new Dictionary<string, Notifier>();

        private class ChatData
        {
            public ChatDto Chat { get; private set; }

            public int PublicKey { get; private set; }
            public int PrivateKey { get; private set; }

            public ChatData(ChatDto chat, int publicKey, int privateKey)
            {
                Chat = chat;

                PublicKey = publicKey;
                PrivateKey = privateKey;
            }
        }

        private class UserKeyHandler
        {
            public char Key { get; private set; }
            public string ActionName { get; private set; }
            public Action Handler { get; private set; }

            public UserKeyHandler(char key, string actionName, Action handler)
            {
                Key = key;
                ActionName = actionName;
                Handler = handler;
            }
        }

        private static readonly IDictionary<char, UserKeyHandler> MainMenuOptions = new Dictionary<char, UserKeyHandler>()
        {
            { '1', new UserKeyHandler('1', "Print chat list", () => { PrintChatsAsync().GetAwaiter().GetResult(); PrintMenu(); }) },
            { '2', new UserKeyHandler('2', "Join chat", () => { JoinChatAsync().GetAwaiter().GetResult(); }) },
            { '3', new UserKeyHandler('3', "Create chat", () => { CreateChatAsync().GetAwaiter().GetResult(); }) },
        };

        /*private static readonly IDictionary<char, UserKeyHandler> ChatMenuOptions = new Dictionary<char, UserKeyHandler>()
        {
            { '1', new UserKeyHandler('1', "Write message", () => { WriteMessageAsync().GetAwaiter().GetResult(); PrintChatMenu(); }) },
            { '2', new UserKeyHandler('2', "Update chat", () => { OpenChatAsync(CURRENT_CHAT.Name).GetAwaiter().GetResult(); PrintChatMenu(); }) },
            { '3', new UserKeyHandler('3', "Close chat", () => { PrintMenu(); }) }
        };*/

        private static void WriteOptions(IDictionary<char, UserKeyHandler> options)
        {
            foreach (KeyValuePair<char, UserKeyHandler> option in options)
                ConsoleWriteLine($"{option.Key}) {option.Value.ActionName}");
            ConsoleWriteLine();
        }

        private static void HandleUserKeys(IDictionary<char, UserKeyHandler> options)
        {
            string line = ConsoleReadLine();

            if (string.IsNullOrEmpty(line))
            {
                ConsoleWriteLine("Invalid input. Try again.");
                HandleUserKeys(options);
            }
            else
            {
                ConsoleWriteLine();
                char key = line[0];

                if (!options.ContainsKey(key))
                {
                    ConsoleWriteLine("Invalid input. Try again.");
                    HandleUserKeys(options);
                }
                else
                    options[key].Handler();
            }
        }

        private static string GetAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(hostName);
            return host.AddressList[0].ToString();
        }

        public static void PrintMenu()
        {
            WriteOptions(MainMenuOptions);
            HandleUserKeys(MainMenuOptions);
        }

        private static async Task ExecuteProtected(Func<Task> executor, Action<CustomException> handler = null)
        {
            try { await executor(); }
            catch (CustomException ex)
            {
                ConsoleWriteLine($"ERROR: {ex.Message}\n");
                handler?.Invoke(ex);
            }
        }

        private static string ReadChatName()
        {
            Console.Write("Enter chat name: ");
            return ConsoleReadLine();
        }

        public static void Main(string[] args)
        {
            ConsoleWriteLine();
            ConsoleWriteLine();

            InitServerListener();

            ConsoleWriteLine("Welcome to Consolegram!");
            ConsoleWriteLine("***********************");

            Console.Write("Please, enter your login: ");

            ME.Name = ConsoleReadLine();
            ME.Endpoint = GetAddress();

            ConsoleWriteLine();

            PrintMenu();

            try { LISTENER.Listening.GetAwaiter().GetResult(); }
            catch (Exception e) { ConsoleWriteLine(e.ToString()); }

            ConsoleWriteLine("finish");
            ConsoleReadLine();
        }

        public static void InitServerListener()
        {
            LISTENER = new Listener(GetAddress());
            LISTENER.Recieved += ServerListener_Recieved;

            ME.Port = LISTENER.Port;

            ConsoleWriteLine($"Got port: {ME.Port}");

            LISTENER.Init();
        }

        public static async Task PrintChatsAsync()
        {
            IEnumerable<string> chatNames = await CHAT_CLIENT.GetChats();

            if (chatNames.Count() == 0)
                ConsoleWriteLine("\n-----NO CHATS YET-----\n");
            else
                ConsoleWriteLine(string.Join("\n", chatNames));
            ConsoleWriteLine();
        }

        public static async Task CreateChatAsync()
        {
            await ExecuteProtected(async () =>
            {
                string name = ReadChatName();

                (int p, int g) = DIFFIE.GetPG();
                (int publ, int priv) = DIFFIE.GetKeys(p, g);

                CreateChatDto createDto = new CreateChatDto();
                createDto.ChatName = name;
                createDto.Creator = ME;
                createDto.P = p;
                createDto.G = g;

                ChatDto chat = await CHAT_CLIENT.CreateChat(createDto);
                ConsoleWriteLine($"Chat {chat.Name} created, public: {publ}, private: {priv}\n");

                ChatData chatData = new ChatData(chat, publ, priv);
                USER_CHAT_KEYS.Add(chat.Name, chatData);
                CURRENT_CHAT = chat;
                StartChatting();
            }, ex => PrintMenu());
        }

        public static async Task JoinChatAsync()
        {
            await ExecuteProtected(async () =>
            {
                ChatDto chat = await CHAT_CLIENT.GetChatInfo(ReadChatName());
                (int publ, int priv) = DIFFIE.GetKeys(chat.P, chat.G);

                JoinChatDto joinDto = new JoinChatDto();
                joinDto.ChatName = chat.Name;
                joinDto.Member = ME;

                chat = await CHAT_CLIENT.JoinChat(joinDto);
                ConsoleWriteLine($"Joined {chat.Name}, public: {publ}, private: {priv}\n");

                ChatData chatData = new ChatData(chat, publ, priv);
                USER_CHAT_KEYS.Add(chat.Name, chatData);
                CURRENT_CHAT = chat;

                foreach (MemberDto member in CURRENT_CHAT.Members)
                    if (!member.Equals(ME))
                        ConnectTo(member);
                StartChatting();
            }, ex => PrintMenu());
        }

        private static void ServerListener_Recieved(object sender, Listener.RecieveEventArgs e)
        {
            string json = Encoding.UTF8.GetString(e.Data);
            SocketActionBase action = JsonConvert.DeserializeObject<SocketActionBase>(json);

            //ConsoleWriteLine($"Recieved {json} via sockets");

            switch (action.Action)
            {
                case SocketActions.JOIN:
                    MemberDto member = JsonConvert.DeserializeObject<MemberDto>(JsonConvert.SerializeObject(action.Data));
                    ConsoleWriteLine($"{member.Name} joined chat");

                    if (!CURRENT_CHAT.Members.Contains(member))
                        CURRENT_CHAT.Members.Add(member);

                    if (!member.Equals(ME))
                        ConnectTo(member);

                    UpdateCommonKey();
                    break;

                case SocketActions.KEY_UPDATE:
                    KeyExchangeDto keyExchange = JsonConvert.DeserializeObject<KeyExchangeDto>(JsonConvert.SerializeObject(action.Data));

                    if (keyExchange.TTL + 1 == CURRENT_CHAT.Members.Count)
                    {
                        CURRENT_CHAT_COMMON_KEY = DIFFIE.GetCommonPrivateKey(
                            keyExchange.KeyTemp,
                            USER_CHAT_KEYS[CURRENT_CHAT.Name].PrivateKey,
                            CURRENT_CHAT.P
                        );

                        ConsoleWriteLine($"Got common key: {CURRENT_CHAT_COMMON_KEY}");
                    }
                    else
                        UpdateCommonKey(keyExchange);

                    break;
                case SocketActions.MESSAGE:
                    Message message = JsonConvert.DeserializeObject<Message>(JsonConvert.SerializeObject(action.Data));

                    string textCode = NOTEPAD.Decode(message.MessageText, CURRENT_CHAT_COMMON_KEY);
                    string text = NOTEPAD.GetCodeText(textCode);

                    ConsoleWriteLine($"{message.SenderName}: {text} ({message.TimeStamp.ToShortTimeString()})");
                    break;
            }
        }

        public static void ConnectTo(MemberDto member)
        {
            if (USER_NOTIFIERS.ContainsKey(member.Name))
                USER_NOTIFIERS.Remove(member.Name);

            Notifier notifier = new Notifier(member.Endpoint, member.Port);
            USER_NOTIFIERS.Add(member.Name, notifier);
        }

        public static void UpdateCommonKey(KeyExchangeDto previous = null)
        {
            int myIndex = CURRENT_CHAT.Members.FindIndex(member => member.Equals(ME));
            int nextIndex = (myIndex + 1) % CURRENT_CHAT.Members.Count;

            if (myIndex == nextIndex)
                throw new ConflictException("sender loop");

            MemberDto nextMember = CURRENT_CHAT.Members[nextIndex];

            KeyExchangeDto dto = dto = new KeyExchangeDto();

            if (previous == null)
                dto.KeyTemp = USER_CHAT_KEYS[CURRENT_CHAT.Name].PublicKey;
            else
            {
                dto.KeyTemp = DIFFIE.GetCommonPrivateKey(
                    previous.KeyTemp,
                    USER_CHAT_KEYS[CURRENT_CHAT.Name].PrivateKey,
                    CURRENT_CHAT.P
                );
                dto.TTL = previous.TTL + 1;
            }

            MoveCursorToEnd();
            ConsoleWriteLine($"Key exchange with user on port {nextMember.Port}");

            KeyExchangeAction action = new KeyExchangeAction(dto);
            USER_NOTIFIERS[nextMember.Name].Send(action);
        }

        public static void StartChatting()
        {
            CHATTING = true;
            while(true)
            {
                WaitMessage();
                string text = Console.ReadLine();
                string encrypted = NOTEPAD.Encode(text, CURRENT_CHAT_COMMON_KEY);

                Message message = new Message();
                message.TimeStamp = DateTime.Now;
                message.SenderName = ME.Name;
                message.MessageText = encrypted;

                ConsoleWriteLine($"{ME.Name}: {text} ({message.TimeStamp.ToShortTimeString()})");

                MessageAction action = new MessageAction(message);

                foreach (Notifier notifier in USER_NOTIFIERS.Values)
                    notifier.Send(action);

            }
            CHATTING = false;
        }

        private static bool CHATTING = false;
        private static int HEIGHT = 2;
        public static void ConsoleWriteLine(string text = "")
        {
            MoveCursorToEnd();
            Console.WriteLine(text);

            int newlines = text.Count(ch => ch == '\n') + 1;
            int length = text.Length - newlines;

            HEIGHT += length / Console.BufferWidth + newlines;

            if (CHATTING)
                WaitMessage();
        }

        public static string ConsoleReadLine()
        {
            string read = Console.ReadLine();
            HEIGHT += (int)Math.Ceiling((double)read.Length / Console.BufferWidth);

            if (CHATTING)
                WaitMessage();

            return read;
        }

        public static void WaitMessage()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Enter your message: ");
        }

        public static void MoveCursorToEnd()
        {
            Console.SetCursorPosition(0, HEIGHT);
        }
    }
}
