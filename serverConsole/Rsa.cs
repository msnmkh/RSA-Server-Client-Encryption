using System;
using System.Numerics;

namespace serverConsole
{
    public class Rsa
    {
        // Encrypt message with  F(m,e) = m^e mode(n)=cipher text
        public static int[] Encrypt(string message,int n,int e)
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
