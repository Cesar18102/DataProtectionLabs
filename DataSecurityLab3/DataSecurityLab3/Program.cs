using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using System.Windows.Forms;

namespace DataSecurityLab3
{
    public class Program
    {
        private static readonly CustomHasherBase hasher2 = new CustomHasherBase(2);
        private static readonly CustomHasherBase hasher4 = new CustomHasherBase(4);
        private static readonly CustomHasherBase hasher8 = new CustomHasherBase(8);

        private static byte[] GetHash(string input, CustomHasherBase hasher)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            return hasher.ComputeHash(data);
        }

        private static IEnumerable<byte[]> GetHashes(byte[] input, params CustomHasherBase[] hashers) =>
            hashers.Select(hasher => hasher.ComputeHash(input));

        private static IEnumerable<byte[]> GetHashes(string input, params CustomHasherBase[] hashers) =>
            hashers.Select(hasher => GetHash(input, hasher));

        private static int GetDiffBytesCount(byte[] hash, byte[] hashOther)
        {
            int count = 0;
            for (int i = 0; i < hash.Length && i < hashOther.Length; ++i)
                if (hash[i] != hashOther[i])
                    ++count;
            return count + Math.Abs(hash.Length - hashOther.Length);
        }

        private static double GetDiffPercentage(byte[] hash, byte[] hashOther) =>
            GetDiffBytesCount(hash, hashOther) * 100.0 / Math.Max(hash.Length, hashOther.Length);

        private static void WriteHashes(IEnumerable<byte[]> hashes)
        {
            Console.WriteLine("\n********");
            for (int j = 0; j < hashes.Count(); ++j)
                Console.WriteLine(BitConverter.ToString(hashes.ElementAt(j)));
            Console.WriteLine("********\n");
        }

        private static void TestWords()
        {
            Console.Write("Input count of test words: ");
            int n = Convert.ToInt32(Console.ReadLine());

            List<string> testWords = new List<string>();
            List<List<byte[]>> allHashes = new List<List<byte[]>>()
            {
                 new List<byte[]>(),
                 new List<byte[]>(),
                 new List<byte[]>()
            };

            for (int i = 0; i < n; ++i)
            {
                Console.Write("Input test word: ");
                string word = Console.ReadLine();

                testWords.Add(word);

                IEnumerable<byte[]> hashes = GetHashes(word, hasher2, hasher4, hasher8);
                for (int j = 0; j < allHashes.Count; ++j)
                    allHashes[j].Add(hashes.ElementAt(j));

                WriteHashes(hashes);
            }

            Console.WriteLine("\n\n*****Hash diffs*****\n\n");

            for (int i = 0; i < n; ++i)
            {
                for (int j = i + 1; j < n; ++j)
                {
                    double diff2 = GetDiffPercentage(allHashes[0][i], allHashes[0][j]);
                    double diff4 = GetDiffPercentage(allHashes[1][i], allHashes[1][j]);
                    double diff8 = GetDiffPercentage(allHashes[2][i], allHashes[2][j]);

                    Console.WriteLine($"{testWords[i]} and {testWords[j]}: {diff2}%; {diff4}%; {diff8}%");
                }
            }
        }

        private static FileStream SelectAndOpenFile(string fileTypeName, string dir, bool repeatIfFailed, params string[] extensions)
        {
            string filter = string.Join(";", extensions.Select(ex => "*." + ex));
            FileDialog dialog = new OpenFileDialog()
            {
                Title = $"Select a {fileTypeName} to be hashed",
                Filter = $"{fileTypeName}s ({filter})|{filter}",
                InitialDirectory = dir,
                CheckFileExists = true,
                AddExtension = true
            };
            DialogResult result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                if (repeatIfFailed)
                    return SelectAndOpenFile(fileTypeName, dir, repeatIfFailed, extensions);

                return null;
            }
            else
                return File.OpenRead(dialog.FileName);
        }

        private static byte[] SelectOpenAndReadFile(string fileTypeName, string dir, params string[] extensions)
        {
            FileStream fs = SelectAndOpenFile(fileTypeName, dir, true, extensions);

            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);

            fs.Close();

            return buffer;
        }

        private static IEnumerable<byte[]> GetFileHashes(string fileTypeName, string dir, params string[] extensions)
        {
            byte[] bytes = SelectOpenAndReadFile(fileTypeName, dir, extensions);
            return GetHashes(bytes, hasher2, hasher4, hasher8);
        }

        private static void TestFiles()
        { 
            Console.WriteLine("Word file hashes");
            WriteHashes(GetFileHashes("Word file", Environment.CurrentDirectory + "\\tests", "doc", "docx"));

            Console.WriteLine("\nImage file hashes");
            WriteHashes(GetFileHashes("Image file", Environment.CurrentDirectory + "\\tests", "jpg", "png", "jpeg", "bmp"));

            Console.WriteLine("\nSource code file hashes");
            WriteHashes(GetFileHashes(
                "Source code file", Environment.CurrentDirectory + "\\tests",
                "cs", "cpp", "h", "lua", "java", 
                "py", "php", "awk", "sh", "bat", 
                "js", "html", "css", "vert", "frag"
            ));
        }

        public static ICollection<byte[]> GenerateTest(int size, byte max)
        {
            if (size == 1)
                return Enumerable.Range(0, max).Select(i => new byte[] { (byte)i }).ToList();

            ICollection<byte[]> concatList = GenerateTest(size - 1, max);
            ICollection<byte[]> result = new List<byte[]>();

            for (byte i = 0; i < max; ++i)
            {
                byte[] num = new byte[] { i };
                foreach (byte[] concatee in concatList)
                    result.Add(num.Concat(concatee).ToArray());
            }

            return result;
        }

        public static byte[] GetCollision(byte[] source, CustomHasherBase hasher)
        {
            byte[] collision = new byte[source.Length];
            Array.Copy(source, collision, source.Length);

            Dictionary<byte, int> counter = new Dictionary<byte, int>();

            for (int i = 0; i < collision.Length; ++i)
            {
                byte key = (byte)(collision[i] % hasher.Size + 1);

                if (!counter.ContainsKey(key))
                    counter.Add(key, 0);

                ++counter[key];
            }

            foreach (byte key in counter.Keys.ToList())
                if (counter[key] % 2 != 0)
                    --counter[key];

            for (int i = 0; i < collision.Length; ++i)
            {
                byte key = (byte)(collision[i] % hasher.Size + 1);

                if (counter[key] == 0)
                    continue;

                if (counter[key] % 2 == 0)
                    collision[i] += (byte)(hasher.Size);
                else
                    collision[i] -= (byte)(hasher.Size);
                --counter[key];
            }

            return collision;
        }

        public static void TestCollision(byte[] source, params CustomHasherBase[] hashers)
        {
            foreach(CustomHasherBase hasher in hashers)
            {
                byte[] collision = GetCollision(source, hasher);

                string sourceText = Encoding.UTF8.GetString(source);
                string collisionText = Encoding.UTF8.GetString(collision);

                byte[] sourceHash = hasher.ComputeHash(source);
                byte[] collisionHash = hasher.ComputeHash(collision);

                Console.WriteLine($"Source text: {sourceText};\nHash={BitConverter.ToString(sourceHash)}\n");
                Console.WriteLine($"Collision text: {collisionText};\nHash={BitConverter.ToString(collisionHash)}\n");
            }
        }

        public static void TestCollisions()
        {
            Console.WriteLine("\nSelect a file to find collision for");
            byte[] bytes = SelectOpenAndReadFile("Any file", Environment.CurrentDirectory + "\\tests", "*");
            TestCollision(bytes, hasher2, hasher4, hasher8);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            TestWords();
            TestFiles();
            TestFiles();
            TestCollisions();

            Console.ReadLine();
        }
    }
}
