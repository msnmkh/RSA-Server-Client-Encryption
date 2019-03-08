using System;
using System.Collections.Generic;
using System.Numerics;

namespace serverConsole
{
    // Create a list of primes, with ability to create keys using the list.
    public class Primes
    {
        private static int n;
        private static int e;
        private static int d;
        private const int MaxValue = 25000;
        private readonly bool[] isPrime = new bool[MaxValue + 1];
        private readonly List<int> primes = new List<int>();

        public Primes()
        {
            for (var i = 2; i <= MaxValue; i++)
            {
                if (!isPrime[i])
                {
                    primes.Add(i);
                    for (var j = i * i; j <= MaxValue; j += i)
                    {
                        isPrime[j] = true;
                    }
                }
            }
        }

        /*
        * Create a key using two random primes. 
        * The variable n is both primes multiplied, 
        * phi is each prime minus one multiplied together.
        * The variable e has to satisfy gcd(e, phi) = 1, so just another prime. 
        * The variable d has to satisfyed = 1 mod phi.
        * The public key is (e, n) and the private key is (d, n).
        */

        public int[] GetKey()
        {
            var end = primes.Count - 1;
            var start = end / 4;
            var random = new Random();
            var primeOne = primes[random.Next(start, end)];
            var primeTwo = primes[random.Next(start, end)];
            while (primeTwo == primeOne)
            {
                primeTwo = primes[random.Next(start, end)];
            }

            n = primeOne * primeTwo;
            var phi = (primeOne - 1) * (primeTwo - 1);

            // Create random number 
            random = new Random();
            do
            {
                do
                {
                    // e is a prime 
                    e = primes[random.Next(start, end)];
                } while (e == primeOne || e == primeTwo);
            } while (!IsFoundD(phi));

            // Create new array to send e,d,n.
            int[] en = new int[3];
            en[0] = d;
            en[1] = e;
            en[2] = n;

            return en;
        }

        public bool IsFoundD(int phi)
        {
            for (var i = phi - 1; i > 1; i--)
            {
                var mul = BigInteger.Multiply(e, i);
                var result = BigInteger.Remainder(mul, phi);

                // If result=1 means that e*d=1 mode(phi)               
                if (result.Equals(1))
                {
                    d = i;
                   // Console.WriteLine("Private Key: (d, n) = (" + d + ", " + n + ")");
                    return true;
                }
            }
            return false;
        }
    }
}
