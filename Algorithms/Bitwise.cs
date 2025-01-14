namespace Algorithms
{
    public class Bitwise
    {
        #region Promenljive

        private string key;
        public string Key { get => key; set => key = value; }

        #endregion

        #region Konstruktor
        public Bitwise()
        {
            Key = "LEMON";
        }

        #endregion

        #region Enkripcija
        public string Encrypt(string message)
        {
            string encryptedMessage = string.Empty;
            for (int i = 0; i < message.Length; i++)
            {
                char messageChar = message[i];
                char keyChar = Key[i % Key.Length];

                int encryptedChar = messageChar ^ keyChar;

                encryptedMessage += ((char)encryptedChar);
            }
            return encryptedMessage.ToString();
        }

        #endregion

        #region Dekripcija
        public string Decrypt(string encryptedMessage)
        {
            string decryptedMessage = string.Empty;
            for (int i = 0; i < encryptedMessage.Length; i++)
            {
                char encryptedChar = encryptedMessage[i];
                char keyChar = Key[i % Key.Length];

                int decryptedChar = encryptedChar ^ keyChar;

                decryptedMessage += ((char)decryptedChar);
            }

            return decryptedMessage.ToString();
        }

        #endregion
    }
}
