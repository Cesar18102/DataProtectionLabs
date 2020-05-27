using System;
using System.IO;
using System.Net;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;

using PemUtils;
using Newtonsoft.Json;

namespace DataProtectionLab2Client
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EncryptedAttribute : Attribute { }

    public class Program
    {
        private const string SERVER_SIGN_UP_URL = "http://109.86.209.135:8080/security_labs/registration_controller.php";
        private const string SERVER_PUBLIC_KEY_URL = "http://109.86.209.135:8080/security_labs/keys/public.pem";
        private const string JSON_FROM_TYPE = "application/json";

        private static RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();

        public class Credentials
        {
            [JsonProperty("login")]
            public string Login { get; private set; }

            [Encrypted]
            [JsonProperty("password")]
            public string Password { get; private set; }

            public Credentials(string login, string password)
            {
                Login = login;
                Password = password;
            }
        }

        public class SignUpResponseDto
        {
            [JsonProperty("status")]
            public string Status { get; private set; }

            [JsonProperty("message")]
            public string Message { get; private set; }

            public SignUpResponseDto() { }

            public SignUpResponseDto(string status, string message)
            {
                Status = status;
                Message = message;
            }
        }

        public static void Main(string[] args)
        {
            RSAParameters parameters = GetParameters();
            Rsa.ImportParameters(parameters);
            SignUp();
        }

        public static RSAParameters GetParameters()
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(SERVER_PUBLIC_KEY_URL);
            request.Method = WebRequestMethods.Http.Get;

            using (Stream str = request.GetResponse().GetResponseStream())
            using (PemReader reader = new PemReader(str))
                return reader.ReadRsaKey();
        }

        public static void SignUp()
        {
            Console.WriteLine("Enter login: ");
            string login = Console.ReadLine();

            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();

            Credentials credentials = new Credentials(login, password);
            object encrypted = Encrypt<Credentials>(credentials);
            string encryptedJson = JsonConvert.SerializeObject(encrypted);

            SignUpResponseDto result = SendSignUp(encryptedJson);
            Console.WriteLine(result.Status + ": " + result.Message);
        }

        public static Dictionary<string, object> Encrypt<T>(T obj) => Encrypt(typeof(T), obj);
        public static Dictionary<string, object> Encrypt(Type type, object obj)
        {
            PropertyInfo[] properties = type.GetProperties();
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (PropertyInfo property in properties)
            {
                JsonPropertyAttribute jsonAttr = property.GetCustomAttribute<JsonPropertyAttribute>();
                string name = jsonAttr == null ? property.Name : jsonAttr.PropertyName;
                object value = property.GetValue(obj);

                if (property.PropertyType.IsPrimitive || property.PropertyType.Equals(typeof(string)))
                {
                    if (property.GetCustomAttribute<EncryptedAttribute>() != null)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(value.ToString());
                        byte[] encrypted = Rsa.Encrypt(bytes, RSAEncryptionPadding.Pkcs1);
                        string base64 = Convert.ToBase64String(encrypted);
                        result.Add(name, base64);
                    }
                    else
                        result.Add(name, value);
                }
                else
                    result.Add(name, Encrypt(property.PropertyType, property.GetValue(obj)));
            }

            return result;
        }

        public static SignUpResponseDto SendSignUp(string signUpData)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(SERVER_SIGN_UP_URL);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = JSON_FROM_TYPE;

            using (StreamWriter strw = new StreamWriter(request.GetRequestStream()))
                strw.WriteLine($"{signUpData}");

            try
            {
                using (StreamReader str = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    string responseJson = str.ReadToEnd();
                    return JsonConvert.DeserializeObject<SignUpResponseDto>(responseJson);
                }
            }
            catch(WebException ex)
            {
                if (ex.Response == null)
                    return new SignUpResponseDto("failed", ex.Message);

                using (StreamReader str = new StreamReader(ex.Response?.GetResponseStream()))
                {
                    string errorResponseJson = str?.ReadToEnd();
                    if (errorResponseJson == null)
                        return new SignUpResponseDto("failed", "unknown error");
                    return JsonConvert.DeserializeObject<SignUpResponseDto>(errorResponseJson);
                }
            }
        }
    }
}
