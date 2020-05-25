using System;
using System.Linq;
using System.Collections.Generic;

namespace DiffieHellmann
{
    public class DiffieHellmann
    {
        private const int MAX_PRIME = 1000;
        private static ICollection<int> Primes = new List<int>();

        private static readonly Random Rand = new Random();

        private static void InitPrimesLessThan(int n)
        {
            if (n <= 0)
                return;

            bool[] crossed = new bool[n];
            for (int i = 1; i < n; ++i)
                if (!crossed[i])
                {
                    int step = i + 1;
                    for (int j = i; j < n; j += step)
                        crossed[j] = true;
                    Primes.Add(step);
                }
        }

        private bool IsPrime(int num)
        {
            if (num <= 1)
                return false;

            int max = (int)Math.Sqrt(num);
            for (int i = 2; i <= max; ++i)
                if (num % i == 0)
                    return false;
            return true;
        }

        private ICollection<int> GetSimpleSquares(int prime)
        {
            ICollection<int> squares = new List<int>();
            for (int i = 1; i < prime; ++i)
            {
                bool found = true;
                for (int j = 1; j < prime - 1; ++j)
                    if (Math.Pow(i, j) % prime == 1)
                    {
                        found = false;
                        break;
                    }

                if (found && Math.Pow(i, prime - 1) % prime == 1)
                    squares.Add(i);
            }

            return squares;
        }

        static DiffieHellmann()
        {
            InitPrimesLessThan(MAX_PRIME);
        }

        public (int p, int g) GetPG()
        {
            int prime = Primes.ElementAt(Rand.Next(0, Primes.Count));
            ICollection<int> squares = GetSimpleSquares(prime).Where(square => IsPrime(square)).ToList();

            if (squares.Count == 0)
                return GetPG();

            return (prime, squares.ElementAt(Rand.Next(0, squares.Count)));
        }

        public (int publ, int priv) GetKeys(int p, int g)
        {
            int priv = Rand.Next(0, p);
            int publ = (int)Math.Pow(g, priv) % p;
            return (publ, priv);
        }

        /// <summary>
        /// Not used at server
        /// </summary>
        public int GetCommonPrivateKey(int publicKeyOther, int privateKeyOwn, int p)
        {
            return (int)Math.Pow(publicKeyOther, privateKeyOwn) % p;
        }
    }
}
