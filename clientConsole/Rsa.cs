using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace clientConsole
{

    /*
    * Create a key using two primes. The variable n is both primes multiplied, phi is each prime minus one multiplied
    * together. The variable e has to satisfy gcd(e, phi) = 1, so just another prime. The variable d has to satisfy
    * ed = 1 mod phi. The public key is (e, n) and the private key is (d, n).
    */
    public class Rsa
    {
        // Encrypt message with  F(m,e) = m^e mode(n)=cipher text
        public static int[] Encrypt(string message, int n, int e)
        {
            var charArray = message.ToCharArray();
            var array = new int[charArray.Length];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = (int)BigInteger.ModPow(charArray[i], e, n);
            }
            return array;
        }

        // Decrypt encrypted message with  F(c,d) = c^d mode(n) = plain text(message)
        public static string Decrypt(int[] cyphertext, int n, int d)
        {
            var array = new char[cyphertext.Length];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = (char)BigInteger.ModPow(cyphertext[i], d, n);
            }
            return new string(array);
        }
    }
}
