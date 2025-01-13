using System;
using System.Runtime.InteropServices;
using System.Text;

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

        public string GetKeyAsString()
        {
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.AppendLine("~ Primarni kljuc: " + string.Join(", ", primaryKey));
            keyBuilder.AppendLine("~ Sekundarni kljuc: " + string.Join(", ", secondaryKey));
            return keyBuilder.ToString();
        }

        public void GenerateKeys()
        {
            int start = rand.Next(10, 100);

            for (int i = 0; i < ALPHABET_LENGTH; i++)
            {
                primaryKey[i] = start;

                if (IsVowel((char)('A' + i)))
                {
                    secondaryKey[i] = (rand.Next(2) == 0) ? start : (start - 1);
                }
                else
                {
                    secondaryKey[i] = -1;
                }

                secondaryKey[i] = IsVowel((char)('A' + i)) ? (start - 1) : -1;

                start -= 2;

                if (start < 10)
                    start = 99;
            }
        }

        public string Encrypt(string message)
        {
            string cryptoMessage = string.Empty;

            foreach (char c in message)
            {
                if(!char.IsLetter(c))
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

        public string Decrypt(string encryptedMessage)
        {
            StringBuilder decryptedMessage = new StringBuilder();
            string[] encryptedParts = encryptedMessage.Split(' '); // Razdvajamo šifrovane delove po razmacima

            foreach (string part in encryptedParts)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    decryptedMessage.Append(' '); // Ako je razmak, dodaj ga direktno
                    continue;
                }

                if (int.TryParse(part, out int number))
                {
                    // Pronađi odgovarajuće slovo
                    char decryptedChar = '\0';
                    for (int i = 0; i < ALPHABET_LENGTH; i++)
                    {
                        if (primaryKey[i] == number || secondaryKey[i] == number)
                        {
                            decryptedChar = (char)('A' + i);
                            break;
                        }
                    }

                    // Dodaj dekriptovano slovo u rezultat
                    if (decryptedChar != '\0')
                    {
                        decryptedMessage.Append(decryptedChar);
                    }
                }
                else
                {
                    // Ako nije broj (npr. interpunkcija), dodaj direktno u rezultat
                    decryptedMessage.Append(part);
                }
            }

            return decryptedMessage.ToString();
        }


    }
}