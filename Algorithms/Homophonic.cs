using System;

namespace Algorithms
{
    public class Homophonic
    {
        #region Inicijalizacija promenljivih

        private const int ALPHABET_LENGTH = 26;

        private int[] primaryKey = new int[ALPHABET_LENGTH];
        private int[] secondaryKey = new int[ALPHABET_LENGTH];
        private Random rand = new Random();
        private int seed = 42;

        #endregion

        #region Konstruktor
        public Homophonic()
        {
            rand = new Random(seed);
            GenerateKeys();
        }

        #endregion

        #region Provera za samoglasnike
        public static bool IsVowel(char c)
        {
            return c == 'A' || c == 'E' || c == 'I' || c == 'O' || c == 'U';
        }

        #endregion

        #region Generisanje kljuceva
        public void GenerateKeys()
        {
            int start = rand.Next(10, 100);

            for (int i = 0; i < ALPHABET_LENGTH; i++)
            {
                primaryKey[i] = start;

                if (IsVowel((char)('A' + i)))
                {
                    secondaryKey[i] = (rand.Next(2) == 0) ? (start -1) : (start - 3);
                }
                else
                {
                    secondaryKey[i] = -1;
                }

                start -= 2;

                if (start < 10)
                    start = 99;
            }
        }

        #endregion

        #region Vracanje kljuceva
        public string GetKeyAsString()
        {
            string primaryKeyFormatted = string.Join(" ", primaryKey);
            string secondaryKeyFormatted = string.Join(" ", secondaryKey);

            string result = $"~ Primarni kljuc:   {primaryKeyFormatted}";
            result += $"\n~ Sekundarni kljuc: {secondaryKeyFormatted}";
            return result;
        }

        #endregion

        #region Enkripcija

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


        #endregion

        #region Dekripcija
        public string Decrypt(string encryptedMessage)
        {
            string decryptedMessage = string.Empty;
            string[] encryptedParts = encryptedMessage.Split(' ');

            foreach (string part in encryptedParts)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    decryptedMessage += " ";
                    continue;
                }

                if (int.TryParse(part, out int number))
                {
                    char decryptedChar = '\0';
                    for (int i = 0; i < ALPHABET_LENGTH; i++)
                    {
                        if (primaryKey[i] == number || secondaryKey[i] == number)
                        {
                            decryptedChar = (char)('A' + i);
                            break;
                        }
                    }

                    if (decryptedChar != '\0')
                    {
                        decryptedMessage += decryptedChar;
                    }
                }
                else
                {
                    decryptedMessage += part;
                }
            }

            return decryptedMessage;
        }

        #endregion
    }
}