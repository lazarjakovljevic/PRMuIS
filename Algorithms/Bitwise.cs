using System.Text;
using System;

namespace Algorithms
{
    public class Bitwise
    {
        private string key;

        public string Key { get => key; set => key = value; }

        public Bitwise()
        {
            Key = "LEMON";
        }

        public string Encrypt(string message)
        {
            string encryptedMessage = string.Empty;
            for (int i = 0; i < message.Length; i++)
            {
                char messageChar = message[i];
                char keyChar = Key[i % Key.Length];

                // Pretvaranje karaktera u ASCII vrednosti i primena XOR operacije
                int encryptedChar = messageChar ^ keyChar;

                // Dodavanje šifrovanog karaktera u rezultat
                encryptedMessage+=((char)encryptedChar);
            }
            return encryptedMessage.ToString();
        }

        // Metoda za dekripciju
        public string Decrypt(string encryptedMessage)
        {
            string decryptedMessage = string.Empty;
            for (int i = 0; i < encryptedMessage.Length; i++)
            {
                char encryptedChar = encryptedMessage[i];
                char keyChar = Key[i % Key.Length];

                // Primena XOR operacije za vraćanje originalnog karaktera
                int decryptedChar = encryptedChar ^ keyChar;

                // Dodavanje dešifrovanog karaktera u rezultat
                decryptedMessage+=((char)decryptedChar);
            }
            return decryptedMessage.ToString();
        }
    }
    
}
