using System;
using System.Security.Cryptography;

namespace DataSecurityLab3
{
    public class CustomHasherBase : HashAlgorithm
    {
        public int Size { get; private set; }
        private new byte[] Hash { get; set; }

        public CustomHasherBase(int size)
        {
            Hash = new byte[size];
            Size = size;
            Initialize();
        }

        public override void Initialize()
        {
            Array.Clear(Hash, 0, Hash.Length);
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            for (int i = 0; i < cbSize; ++i)
            {
                int mod = array[i + ibStart] % Hash.Length + 1;
                for (int j = 0; j < Hash.Length; j += mod)
                    Hash[j] += array[i + ibStart];
            }
        }

        protected override byte[] HashFinal()
        {
            return Hash;
        }
    }
}
