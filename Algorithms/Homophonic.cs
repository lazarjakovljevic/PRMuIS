using System;
using System.Runtime.InteropServices;

namespace Algorithms
{
    public class Homophonic
    {
        private const int ALPHABET_LENGTH = 26;

        private int[] primaryKey = new int[ALPHABET_LENGTH];
        private int[] secondaryKey = new int[ALPHABET_LENGTH];
        private Random rand = new Random();

        public Homophonic() 
        {
            rand = new Random(42);
            GenerateKeys();
        }

        public static bool IsVowel(char c)
        {
            return c == 'A' || c == 'E' || c == 'I' || c == 'O' || c == 'U';
        }

        public void GenerateKeys()
        {
            int start = rand.Next(10, 100); // broj izmedju 10 i 100

            for (int i = 0; i < ALPHABET_LENGTH; i++)
            {
                primaryKey[i] = start;
                start -= 2;

                if (start < 10)
                    start = 99;

                secondaryKey[i] = IsVowel((char)('A' + i)) ? (start - 1) : -1;
            }
        }

        public string Encrypt(string message)
        {    
            string cryptoMessage = string.Empty;

            foreach (char c in message)
            {
                if (!char.IsLetter(c))
                {
                    cryptoMessage += c; 
                    continue;
                }

                char upperChar = char.ToUpper(c);
                int index = upperChar - 'A';

                int chosenKey = primaryKey[index];

                if (secondaryKey[index] != -1 && rand.Next(2) == 0)
                {
                    chosenKey = secondaryKey[index];
                }

                cryptoMessage += chosenKey.ToString() + " ";
            }

            return cryptoMessage;
        }
    } 
}
