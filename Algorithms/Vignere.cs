namespace Algorithms
{
    public class Vignere
    {
        private string key;

        public string Key { get => key; set => key = value; }

        public Vignere()
        {
            Key = "MOUSE";
        }

        public string Encrypt(string message)
        {
            string encryptedMessage = string.Empty;

            for (int i = 0; i < message.Length; i++)
            {
                char messageChar = char.ToUpper(message[i]);

                if (!char.IsLetter(messageChar)) 
                {
                    encryptedMessage += messageChar; 
                    continue; 
                }

                char keyChar = char.ToUpper(Key[i % Key.Length]);
                int encryptedChar = (messageChar - 'A' + keyChar - 'A') % 26 + 'A';
                encryptedMessage += (char)encryptedChar;
            }

            return encryptedMessage;
        }

        public string Decrypt(string encryptedMessage)
        {
            string decryptedMessage = string.Empty;

            for (int i = 0; i < encryptedMessage.Length; i++)
            {
                char encryptedChar = char.ToUpper(encryptedMessage[i]);
                if (!char.IsLetter(encryptedChar)) 
                {
                    decryptedMessage += encryptedChar; 
                    continue; 
                }

                char keyChar = char.ToUpper(Key[i % Key.Length]);
                int decryptedChar = (encryptedChar - 'A' - keyChar + 'A' + 26) % 26 + 'A';
                decryptedMessage += (char)decryptedChar;
            }

            return decryptedMessage;
        }
    }
}