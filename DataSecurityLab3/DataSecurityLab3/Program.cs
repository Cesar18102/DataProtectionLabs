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

        private static IEnumerable<byte[]> GetFileHashes(string fileTypeName, string dir, params string[] extensions)
        {
            FileStream fs = SelectAndOpenFile(fileTypeName, dir, true, extensions);

            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);

            fs.Close();

            return GetHashes(buffer, hasher2, hasher4, hasher8);
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

        public static void TestBruteForce()
        {
            Console.Write("Input a string to find hash collision for it: ");
            string input = Console.ReadLine();

            List<CustomHasherBase> hashers = new List<CustomHasherBase>() { hasher2, hasher4, hasher8 };
            List<byte[]> hashes = GetHashes(input, hashers.ToArray()).ToList();

            for (int i = 0; i < hashes.Count; ++i)
            {
                bool found = false;
                ulong upperBound = (ulong)Math.Pow(256, hashes[i].Length);
                for(ulong j = 0; j < upperBound; ++j)
                {
                    byte[] test = BitConverter.GetBytes(j);
                    int diff = GetDiffBytesCount(test, hashes[i]);

                    if (diff == 0)
                    {
                        found = true;
                        Console.WriteLine("Collision found: " + input + " and " + Encoding.UTF7.GetString(test));
                        break;
                    }
                }

                if (!found)
                    Console.WriteLine("Collisions not found");
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            //TestWords();
            //TestFiles();
            //TestFiles();
            TestBruteForce();

            Console.ReadLine();
        }
    }
}
