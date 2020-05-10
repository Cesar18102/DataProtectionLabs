using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;

using PemUtils;
using Newtonsoft.Json;

namespace DataProtectionLab2Client
{
    public class Program
    {
        private const string SERVER_SIGN_UP_URL = "http://localhost:8080/security_labs/registration_controller.php";
        private const string SERVER_PUBLIC_KEY_URL = "http://localhost:8080/security_labs/keys/public.pem";
        private const string URL_ENCODED_FROM_TYPE = "application/x-www-form-urlencoded";

        private static RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();

        public class Credentials
        {
            [JsonProperty("login")]
            public string Login { get; private set; }

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
            byte[] credsEncrypted = Encrypt(credentials);

            SignUpResponseDto result = SendSignUp(credsEncrypted);

            Console.WriteLine(result.Status + ": " + result.Message);
        }

        public static byte[] Encrypt(Credentials credentials)
        {
            string credsJson = JsonConvert.SerializeObject(credentials);
            byte[] credsBytes = Encoding.UTF8.GetBytes(credsJson);
            return Rsa.Encrypt(credsBytes, RSAEncryptionPadding.Pkcs1);
        }

        public static SignUpResponseDto SendSignUp(byte[] credsEncrypted)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(SERVER_SIGN_UP_URL);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = URL_ENCODED_FROM_TYPE;
            request.ContentLength = credsEncrypted.Length;

            using (Stream str = request.GetRequestStream())
                str.Write(credsEncrypted, 0, credsEncrypted.Length);

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
