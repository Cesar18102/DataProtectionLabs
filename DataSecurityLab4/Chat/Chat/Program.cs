using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DH = DiffieHellmann;

using Chat.Models;
using Chat.Dto.Input;
using Chat.Dto.Output;
using Chat.Dto.Input.Exceptions;

using DataSecurityPractice2;

namespace Chat
{
    public class Program
    {
        private static readonly ChatClient CHAT_CLIENT = new ChatClient("http://localhost:5000/api/");
        private static readonly DH.DiffieHellmann DIFFIE = new DH.DiffieHellmann();
        private static readonly DisposableNotepad NOTEPAD = new DisposableNotepad();

        private static string LOGIN { get; set; }
        private static ChatDto CURRENT_CHAT { get; set; }
        private static readonly IDictionary<string, UserChatKeys> USER_CHAT_KEYS = new Dictionary<string, UserChatKeys>();

        private class UserChatKeys
        {
            public ChatInfoDto Chat { get; private set; }
            public int PublicKey { get; private set; }
            public int PrivateKey { get; private set; }

            public UserChatKeys(ChatInfoDto chat, int publicKey, int privateKey)
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
            { '2', new UserKeyHandler('2', "Join chat", () => { JoinChatAsync().GetAwaiter().GetResult(); PrintMenu(); }) },
            { '3', new UserKeyHandler('3', "Create chat", () => { CreateChatAsync().GetAwaiter().GetResult(); PrintMenu(); }) },
            { '4', new UserKeyHandler('4', "Open chat", () => { OpenChatAsync().GetAwaiter().GetResult(); }) }
        };

        private static readonly IDictionary<char, UserKeyHandler> ChatMenuOptions = new Dictionary<char, UserKeyHandler>()
        {
            { '1', new UserKeyHandler('1', "Write message", () => { WriteMessageAsync().GetAwaiter().GetResult(); PrintChatMenu(); }) },
            { '2', new UserKeyHandler('2', "Update chat", () => { OpenChatAsync(CURRENT_CHAT.Name).GetAwaiter().GetResult(); PrintChatMenu(); }) },
            { '3', new UserKeyHandler('3', "Close chat", () => { PrintMenu(); }) }
        };

        private static void WriteOptions(IDictionary<char, UserKeyHandler> options)
        {
            foreach (KeyValuePair<char, UserKeyHandler> option in options)
                Console.WriteLine($"{option.Key}) {option.Value.ActionName}");
            Console.WriteLine();
        }

        private static void HandleUserKeys(IDictionary<char, UserKeyHandler> options)
        {
            string line = Console.ReadLine();

            if (string.IsNullOrEmpty(line))
            {
                Console.WriteLine("Invalid input. Try again.");
                HandleUserKeys(options);
            }
            else
            {
                Console.WriteLine();
                char key = line[0];

                if (!options.ContainsKey(key))
                {
                    Console.WriteLine("Invalid input. Try again.");
                    HandleUserKeys(options);
                }
                else
                    options[key].Handler();
            }
        }

        public static void PrintMenu()
        {
            WriteOptions(MainMenuOptions);
            HandleUserKeys(MainMenuOptions);
        }

        public static void PrintChatMenu()
        {
            WriteOptions(ChatMenuOptions);
            HandleUserKeys(ChatMenuOptions);
        }

        private static async Task ExecuteProtected(Func<Task> executor, bool throwOn = false)
        {
            try { await executor(); }
            catch (CustomException ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}\n");

                if (throwOn)
                    throw;
            }
        }

        private static string ReadChatName()
        {
            Console.Write("Enter chat name: ");
            return Console.ReadLine();
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Consolegram!");
            Console.WriteLine("***********************");

            Console.Write("Please, enter your login: ");
            LOGIN = Console.ReadLine();
            Console.WriteLine();

            PrintMenu();
            Console.ReadLine();
        }

        public static async Task PrintChatsAsync()
        {
            IEnumerable<ChatInfoDto> chats = await CHAT_CLIENT.GetChats();

            if (chats.Count() == 0)
                Console.WriteLine("\n-----NO CHATS YET-----\n");
            else
            {
                foreach (ChatInfoDto chat in chats)
                    Console.WriteLine(chat.Name);
            }
        }

        public static async Task JoinChatAsync()
        {
            await ExecuteProtected(async () =>
            {
                ChatInfoDto chat = await CHAT_CLIENT.GetChatInfo(ReadChatName());

                (int publ, int priv) = DIFFIE.GetKeys(chat.P, chat.G);
                JoinChatDto joinDto = new JoinChatDto(chat.Name, LOGIN, publ);
                ChatInfoDto chatInfo = await CHAT_CLIENT.JoinChat(joinDto);

                Console.WriteLine($"Joined {chat.Name}, public: {publ}, private: {priv}\n");
                USER_CHAT_KEYS.Add(chat.Name, new UserChatKeys(chat, publ, priv));

                await OpenChatAsync(chat.Name);
            });
        }

        public static async Task CreateChatAsync()
        {
            await ExecuteProtected(async () =>
            {
                string name = ReadChatName();

                (int p, int g) = DIFFIE.GetPG();
                (int publ, int priv) = DIFFIE.GetKeys(p, g);

                CreateChatDto createDto = new CreateChatDto(name, LOGIN, p, g, publ);
                ChatInfoDto chat = await CHAT_CLIENT.CreateChat(createDto);

                Console.WriteLine($"Chat {chat.Name} created, public: {publ}, private: {priv}\n");
                USER_CHAT_KEYS.Add(chat.Name, new UserChatKeys(chat, publ, priv));

                await OpenChatAsync(chat.Name);
            });
        }

        private static async Task UpdateCurrentChat()
        {
            await ExecuteProtected(async () =>
            {
                CURRENT_CHAT = await CHAT_CLIENT.OpenChat(CURRENT_CHAT.Name, LOGIN);
            });
        }

        private static UserChatKeys GetChatKeys(ChatDto chat)
        {
            if (chat == null || !USER_CHAT_KEYS.ContainsKey(chat.Name))
            {
                Console.WriteLine($"Unable to read \"{chat.Name}\" chat");
                throw new Exception("No chat keys error");
            }

            return USER_CHAT_KEYS[chat.Name];
        }

        public static IEnumerable<DecodedMessage> DecodeChat(ChatDto chat)
        {
            UserChatKeys keys = GetChatKeys(chat);

            ICollection<DecodedMessage> messages = new List<DecodedMessage>();
            foreach (MessageDto encodedMessage in chat.Messages)
            {
                if(encodedMessage.EncodedText == "******")
                {
                    messages.Add(new DecodedMessage(encodedMessage.EncodedText, encodedMessage.Sender.Name, encodedMessage.TimeSpan));
                    continue;
                }

                int commonKey = DIFFIE.GetCommonPrivateKey(encodedMessage.Sender.PublicKey, keys.PrivateKey, keys.Chat.P);
                string decodedMessage = NOTEPAD.GetCodeText(NOTEPAD.Decode(encodedMessage.EncodedText, commonKey));
                messages.Add(new DecodedMessage(decodedMessage, encodedMessage.Sender.Name, encodedMessage.TimeSpan));
            }

            return messages;
        }

        public static WriteMessageDto EncodeMessage(string message, ChatDto chat)
        {
            UserChatKeys keys = GetChatKeys(chat);

            ICollection<EncodedMessageDto> encodedMessages = new List<EncodedMessageDto>();
            foreach (MemberDto member in chat.Members)
            {
                int commonKey = DIFFIE.GetCommonPrivateKey(member.PublicKey, keys.PrivateKey, keys.Chat.P);
                string encodedMessage = NOTEPAD.Encode(message, commonKey);
                encodedMessages.Add(new EncodedMessageDto(encodedMessage, member.Name));
            }

            return new WriteMessageDto(chat.Name, LOGIN, encodedMessages);
        }

        public static void DrawChat(ChatDto chat)
        {
            List<DecodedMessage> messages = DecodeChat(chat).OrderBy(message => message.TimeSpan).ToList();

            if (messages.Count == 0)
                Console.WriteLine("-----NOT MESSAGES HERE YET-----\n");
            else
            {
                foreach (DecodedMessage message in messages)
                    Console.WriteLine($"{message.Sender}: {message.Text}, at {message.TimeSpan.ToShortTimeString()}");
                Console.WriteLine();
            }
        }

        public static async Task OpenChatAsync()
        {
            await OpenChatAsync(ReadChatName());
        }

        public static async Task OpenChatAsync(string name)
        {
            try
            {
                await ExecuteProtected(async () =>
                {
                    CURRENT_CHAT = await CHAT_CLIENT.OpenChat(name, LOGIN);
                    DrawChat(CURRENT_CHAT);
                }, true);

                PrintChatMenu();
            }
            catch(Exception e) { Console.WriteLine($"{e.Message}\n"); PrintMenu(); }
        }

        public static async Task WriteMessageAsync()
        {
            Console.Write("Enter your message: ");
            string message = Console.ReadLine();

            await UpdateCurrentChat();
            WriteMessageDto writeMessage = EncodeMessage(message, CURRENT_CHAT);

            if (writeMessage == null)
            {
                Console.WriteLine("Failed to write a message. Try again.");
                await WriteMessageAsync();
            }
            else
            {
                try
                {
                    await ExecuteProtected(async () =>
                    {
                        MessageDto messageDto = await CHAT_CLIENT.WriteMessage(writeMessage);
                        Console.WriteLine("\nMessage sent!\n\n");

                        await UpdateCurrentChat();
                        DrawChat(CURRENT_CHAT);
                    }, true);
                }
                catch(Exception e) { Console.WriteLine($"{e.Message}\n"); PrintChatMenu(); }
            }
        }
    }
}
