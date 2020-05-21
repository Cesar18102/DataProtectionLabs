using System;
using System.Linq;
using System.Collections.Generic;

namespace DataSecurityPractice1
{
    public class Caesar
    {
        private int SIZE = 'я' - 'а' + 1;
        private List<char> DEFAULT_ALPHABET = Enumerable.Range('а', 'е' - 'а' + 1)
            .Append('ё')
            .Concat(Enumerable.Range('ж', 'я' - 'ж' + 1))
            .Select(letter => (char)letter)
            .ToList();

        private Dictionary<char, char> EncodeAlphabet { get; set; } = new Dictionary<char, char>();
        private Dictionary<char, char> DecodeAlphabet { get; set; } = new Dictionary<char, char>();

        public Caesar(int key)
        {
            foreach(char letter in DEFAULT_ALPHABET)
            {
                char encodedLetter = DEFAULT_ALPHABET[(DEFAULT_ALPHABET.IndexOf(letter) + key) % DEFAULT_ALPHABET.Count];
                EncodeAlphabet.Add(letter, encodedLetter);
                DecodeAlphabet.Add(encodedLetter, letter);
            }
        }

        public Caesar(string wordKey) : this(wordKey, 0) { }
        public Caesar(string wordKey, int key)
        {
            IEnumerable<char> wordKeyUnique = wordKey
                .ToLower()
                .ToCharArray()
                .Where(letter => char.IsLetter(letter))
                .Distinct();

            int keySize = wordKeyUnique.Count();
            IEnumerable<char> other = DEFAULT_ALPHABET.Except(wordKeyUnique);

            char[] encodeAlphabet = other
                .Skip(DEFAULT_ALPHABET.Count - key - keySize)
                .Concat(wordKeyUnique)
                .Concat(other.Take(DEFAULT_ALPHABET.Count - key - keySize))
                .ToArray();

            for(int i = 0; i < DEFAULT_ALPHABET.Count; ++i)
            {
                EncodeAlphabet.Add(DEFAULT_ALPHABET[i], encodeAlphabet[i]);
                DecodeAlphabet.Add(encodeAlphabet[i], DEFAULT_ALPHABET[i]);
            }
        }

        private string Process(string input, bool encoding)
        {
            char[] letters = input.ToCharArray();
            for (int i = 0; i < letters.Length; ++i)
                if (char.IsLetter(letters[i]))
                {
                    bool wasUpper = char.IsUpper(letters[i]);
                    char lower = char.ToLower(letters[i]);
                    char processed = encoding ? EncodeAlphabet[lower] : DecodeAlphabet[lower];
                    letters[i] = wasUpper ? char.ToUpper(processed) : processed;
                }
            return string.Join("", letters);
        }

        public string Encode(string input) => Process(input, true);
        public string Decode(string input) => Process(input, false);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("\n*****************************\n");
                    Console.Write("Input key: ");
                    string key = Console.ReadLine();

                    int ikey = 0;
                    Caesar caesar = null;

                    if (int.TryParse(key, out ikey))
                        caesar = new Caesar(ikey);
                    else
                    {
                        Console.Write("Input number key: ");
                        caesar = new Caesar(key, Convert.ToInt32(Console.ReadLine()));
                    }

                    Console.Write("Input text to encode: ");
                    string text = Console.ReadLine();

                    string encoded = caesar.Encode(text);
                    Console.WriteLine($"\nEncoded text: {encoded}\n");

                    string decoded = caesar.Decode(encoded);
                    Console.WriteLine($"Decoded text: {decoded}\n");

                    Console.WriteLine("\n*****************************\n");
                }
                catch { Console.WriteLine("\n\n***ERROR***\n\n"); }
            }
        }
    }
}
