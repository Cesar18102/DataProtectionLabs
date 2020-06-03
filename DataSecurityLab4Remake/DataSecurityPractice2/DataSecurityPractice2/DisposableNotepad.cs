using System;
using System.Linq;
using System.Text;

namespace DataSecurityPractice2
{
    public class DisposableNotepad
    {
        private static readonly char[] MOST_USED_LETTERS = "аитесно".ToCharArray();
        private static readonly char[] OTHER_LETTERS = Enumerable.Range('а', 'я' - 'a' + 1)
            .Select(letter => (char)letter)
            .Except(MOST_USED_LETTERS)
            .Except(new char[] { 'й' })
            .ToArray();

        private string this[char toDigit]
        {
            get
            {
                if (toDigit == 'й')
                    toDigit = 'и';

                if (toDigit == 'ё')
                    toDigit = 'е';

                if (!char.IsLetter(toDigit))
                    return "00";

                char lower = char.ToLower(toDigit);
                int mostUsedIndex = Array.IndexOf(MOST_USED_LETTERS, lower);

                if (mostUsedIndex != -1)
                    return (mostUsedIndex + 1).ToString();

                int otherIndex = Array.IndexOf(OTHER_LETTERS, lower);

                int rowId = ((otherIndex / 10) + 8) % 10;
                int colId = (otherIndex + 1) % 10;

                return rowId.ToString() + colId.ToString();
            }
        }

        public string GetCodeText(string code)
        {
            StringBuilder result = new StringBuilder();
            for(int i = 0; i < code.Length; ++i)
            {
                int codeNum = code[i] - '0';
                if (codeNum == 0)
                    codeNum = 10;

                if (codeNum <= 7)
                {
                    result.Append(MOST_USED_LETTERS[codeNum - 1]);
                    continue;
                }

                if (++i >= code.Length)
                    break;

                int next = code[i] - '0';
                if (next == 0)
                    next = 10;

                if (codeNum == 10 && next == 10)
                {
                    result.Append(' ');
                    continue;
                }

                result.Append(OTHER_LETTERS[(codeNum - 8) * 10 + next - 1]);
            }

            return result.ToString();
        }

        private string GetTextCode(string text) =>
            string.Join("", text.Select(letter => this[letter]));

        public string Encode(int text, int key) =>
            EncodeCode(text.ToString(), Math.Abs(key).ToString());

        public string Encode(int text, string key) =>
            EncodeCode(text.ToString(), GetTextCode(key));

        public string Encode(string text, int key) =>
            EncodeCode(GetTextCode(text), Math.Abs(key).ToString());

        public string Encode(string text, string key) =>
            EncodeCode(GetTextCode(text), GetTextCode(key));

        private char GetSum(char num1, char num2) =>
            (char)(((num1 - '0' + num2 - '0') % 10) + '0');

        private char GetDiff(char num1, char num2) =>
            (char)((num1 - num2 + 10) % 10 + '0');

        public string Decode(string encoded, int key) =>
            DecodeCode(encoded, Math.Abs(key).ToString());

        public string Decode(string encoded, string key) =>
            DecodeCode(encoded, GetTextCode(key));

        private void FitInputs(ref string textCode, ref string keyCode)
        {
            StringBuilder keyCodeFull = new StringBuilder(keyCode);
            while (textCode.Length > keyCodeFull.Length)
                keyCodeFull.Append(keyCode);

            keyCode = keyCodeFull.ToString();
            if (textCode.Length < keyCode.Length)
                textCode += new string('0', keyCode.Length - textCode.Length);
        }

        private string EncodeCode(string textCode, string keyCode)
        {
            FitInputs(ref textCode, ref keyCode);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < textCode.Length; ++i)
                result.Append(GetSum(textCode[i], keyCode[i]));
            return result.ToString();
        }

        public string DecodeCode(string encoded, string keyCode)
        {
            FitInputs(ref encoded, ref keyCode);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < encoded.Length; ++i)
                result.Append(GetDiff(encoded[i], keyCode[i]));
            return result.ToString();
        }
    }
}
