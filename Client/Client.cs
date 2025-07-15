using Algorithms;
using Communication;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Client
{
    internal class Client
    {
        static void Main(string[] args)
        {
            while (true)
            {
                #region Odabir protokola i algoritma

                string protocol = CheckValidInputProtocol();
                Console.WriteLine($"Izabrani protokol: {protocol}");

                string algorithm = CheckValidInputAlgorithm();
                Console.WriteLine($"Izabrani algoritam: {algorithm}\n");
                #endregion

                #region UDP

                if (protocol == "UDP")
                {
                    #region Inicijalizacija i povezivanje

                    Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 50001);
                    EndPoint serverResponseEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    clientSocket.Blocking = false;

                    #endregion

                    #region Komunikacija

                    #region Homofono sifrovanje

                    if (algorithm == "HOMOFONO")
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

                        while (true)
                        {
                            try
                            {
                                Console.Write("Unesite poruku ('kraj' za izlaz): ");
                                string message = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(message))
                                    continue;

                                Homophonic homophonic = new Homophonic();
                                string encryptingMessage = homophonic.Encrypt(message);

                                string key = homophonic.GetKeyAsString();
                                NacinKomunikacije nacin = new NacinKomunikacije(algorithm, key);

                                byte[] objectBytes;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    formatter.Serialize(ms, nacin);
                                    objectBytes = ms.ToArray();
                                }

                                byte[] messageBytes = Encoding.UTF8.GetBytes(encryptingMessage);
                                byte[] dataToSend = new byte[objectBytes.Length + messageBytes.Length + 1];
                                Array.Copy(objectBytes, 0, dataToSend, 0, objectBytes.Length);
                                dataToSend[objectBytes.Length] = (byte)'|'; // separator
                                Array.Copy(messageBytes, 0, dataToSend, objectBytes.Length + 1, messageBytes.Length);

                                clientSocket.SendTo(dataToSend, serverEndPoint);

                                if (message.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }

                                if (clientSocket.Poll(30 * 1000 * 1000, SelectMode.SelectRead))
                                {
                                    byte[] buffer = new byte[1024];
                                    int receivedBytes = clientSocket.ReceiveFrom(buffer, ref serverResponseEndPoint);

                                    string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes).Trim();
                                    Console.WriteLine($"Primljen enkriptovani odgovor: {encryptedMessage}");

                                    string decryptedMessage = homophonic.Decrypt(encryptedMessage);
                                    Console.WriteLine($"Dekriptovani odgovor od servera: {decryptedMessage.ToLower()}");

                                    if (decryptedMessage.ToLower() == "kraj")
                                    {
                                        Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                        break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Poruka nije stigla.");
                                }

                            }
                            catch (SocketException ex)
                            {
                                Console.WriteLine($"Doslo je do greske tokom slanja poruke! \n\n{ex}");
                                break;
                            }
                        }
                    }
                    #endregion

                    #region Sifrovanje upotrebom bitova

                    if (algorithm == "BITOVI")
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

                        while (true)
                        {
                            try
                            {
                                Console.Write("Unesite poruku ('kraj' za izlaz): ");
                                string message = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(message))
                                    continue;

                                Bitwise bitwise = new Bitwise();
                                string encryptingMessage = bitwise.Encrypt(message);

                                string key = bitwise.Key;
                                NacinKomunikacije nacin = new NacinKomunikacije(algorithm, key);

                                byte[] objectBytes;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    formatter.Serialize(ms, nacin);
                                    objectBytes = ms.ToArray();
                                }

                                byte[] messageBytes = Encoding.UTF8.GetBytes(encryptingMessage);
                                byte[] dataToSend = new byte[objectBytes.Length + messageBytes.Length + 1];
                                Array.Copy(objectBytes, 0, dataToSend, 0, objectBytes.Length);
                                dataToSend[objectBytes.Length] = (byte)'|'; // separator
                                Array.Copy(messageBytes, 0, dataToSend, objectBytes.Length + 1, messageBytes.Length);

                                clientSocket.SendTo(dataToSend, serverEndPoint);

                                if (message.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }

                                if (clientSocket.Poll(30 * 1000 * 1000, SelectMode.SelectRead))
                                {
                                    byte[] buffer = new byte[1024];
                                    int receivedBytes = clientSocket.ReceiveFrom(buffer, ref serverResponseEndPoint);

                                    string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                                    Console.WriteLine($"Primljen enkriptovani odgovor: {encryptedMessage}");

                                    string decryptedMessage = bitwise.Decrypt(encryptedMessage);
                                    Console.WriteLine($"Dekriptovani odgovor od servera: {decryptedMessage}");

                                    if (decryptedMessage.ToLower() == "kraj")
                                    {
                                        Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                        break;
                                    }

                                }
                                else
                                {
                                    Console.WriteLine("Poruka nije stigla.");
                                }
                            }
                            catch (SocketException ex)
                            {
                                Console.WriteLine($"Doslo je do greske tokom slanja poruke! \n\n{ex}");
                                break;
                            }
                        }
                    }
                    #endregion

                    #region Viznerov algoritam

                    if (algorithm == "VIZNER")
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

                        while (true)
                        {
                            try
                            {
                                Console.Write("Unesite poruku ('kraj' za izlaz): ");
                                string message = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(message))
                                    continue;

                                Vignere vignere = new Vignere();
                                string encryptingMessage = vignere.Encrypt(message);

                                string key = vignere.Key;
                                NacinKomunikacije nacin = new NacinKomunikacije(algorithm, key);

                                byte[] objectBytes;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    formatter.Serialize(ms, nacin);
                                    objectBytes = ms.ToArray();
                                }

                                byte[] messageBytes = Encoding.UTF8.GetBytes(encryptingMessage);
                                byte[] dataToSend = new byte[objectBytes.Length + messageBytes.Length + 1];
                                Array.Copy(objectBytes, 0, dataToSend, 0, objectBytes.Length);
                                dataToSend[objectBytes.Length] = (byte)'|'; // separator
                                Array.Copy(messageBytes, 0, dataToSend, objectBytes.Length + 1, messageBytes.Length);

                                clientSocket.SendTo(dataToSend, serverEndPoint);

                                if (message.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }

                                byte[] buffer = new byte[1024];
                                int receivedBytes = clientSocket.ReceiveFrom(buffer, ref serverResponseEndPoint);

                                string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                                Console.WriteLine($"Primljen enkriptovani odgovor: {encryptedMessage}");

                                string decryptedMessage = vignere.Decrypt(encryptedMessage);
                                Console.WriteLine($"Dekriptovani odgovor od servera: {decryptedMessage}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }
                            }
                            catch (SocketException ex)
                            {
                                Console.WriteLine($"Doslo je do greske tokom slanja poruke! \n\n{ex}");
                                break;
                            }
                        }
                    }
                    #endregion


                    #endregion

                    #region Zatvaranje uticnice

                    Console.WriteLine("UDP klijent zavrsava sa radom.");
                    clientSocket.Close();

                    #endregion
                }

                #endregion

                #region TCP

                else if (protocol == "TCP")
                {
                    #region Inicijalizacija i povezivanje
                    Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 50001);

                    byte[] buffer = new byte[1024];

                    try
                    {
                        Console.WriteLine("\nPovezivanje sa serverom...");
                        clientSocket.Connect(serverEndPoint);
                        Console.WriteLine("\nKlijent je uspesno povezan sa serverom!");
                    }

                    catch (SocketException ex)
                    {
                        Console.WriteLine($"\nDoslo je do greske prilikom povezivanja sa serverom! \n\n{ex}");
                        break;
                    }

                    #endregion

                    #region Komunikacija

                    #region Homofono sifrovanje

                    if (algorithm == "HOMOFONO")
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        while (true)
                        {
                            try
                            {
                                Console.Write("\nUnesite poruku ('kraj' za izlaz): ");
                                string message = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(message))
                                    continue;


                                Homophonic homophonic = new Homophonic();
                                string encryptingMessage = homophonic.Encrypt(message);

                                string key = homophonic.GetKeyAsString();
                                NacinKomunikacije nacin = new NacinKomunikacije(algorithm, key);

                                byte[] objectBytes;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    formatter.Serialize(ms, nacin);
                                    objectBytes = ms.ToArray();
                                }

                                byte[] messageBytes = Encoding.UTF8.GetBytes(encryptingMessage);
                                byte[] dataToSend = new byte[objectBytes.Length + messageBytes.Length + 1];
                                Array.Copy(objectBytes, 0, dataToSend, 0, objectBytes.Length);
                                dataToSend[objectBytes.Length] = (byte)'|'; // Separator
                                Array.Copy(messageBytes, 0, dataToSend, objectBytes.Length + 1, messageBytes.Length);

                                int numOfBytes = clientSocket.Send(dataToSend);
                                numOfBytes = clientSocket.Receive(buffer);

                                if (message.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa serverom.");
                                    break;
                                }

                                if (numOfBytes == 0)
                                {
                                    Console.WriteLine("\nServer je zavrsio sa radom");
                                    break;
                                }

                                string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, numOfBytes);
                                Console.WriteLine($"Primljen enkriptovani odgovor: {encryptedMessage}");
                                string decryptedMessage = homophonic.Decrypt(encryptedMessage);
                                Console.WriteLine($"Dekriptovani odgovor od servera: {decryptedMessage.ToLower()}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa serverom.");
                                    break;
                                }
                            }
                            catch (SocketException ex)
                            {
                                Console.WriteLine($"Doslo je do greske tokom slanja poruke! \n{ex}");
                                break;
                            }
                        }
                    }

                    #endregion

                    #region Sifrovanje upotrebom bitova

                    if (algorithm == "BITOVI")
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        while (true)
                        {
                            try
                            {
                                Console.Write("\nUnesite poruku ('kraj' za izlaz): ");
                                string message = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(message))
                                    continue;


                                Bitwise bitwise = new Bitwise();
                                string encryptingMessage = bitwise.Encrypt(message);

                                string key = bitwise.Key;
                                NacinKomunikacije nacin = new NacinKomunikacije(algorithm, key);

                                byte[] objectBytes;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    formatter.Serialize(ms, nacin);
                                    objectBytes = ms.ToArray();
                                }

                                byte[] messageBytes = Encoding.UTF8.GetBytes(encryptingMessage);
                                byte[] dataToSend = new byte[objectBytes.Length + messageBytes.Length + 1];
                                Array.Copy(objectBytes, 0, dataToSend, 0, objectBytes.Length);
                                dataToSend[objectBytes.Length] = (byte)'|'; // Separator
                                Array.Copy(messageBytes, 0, dataToSend, objectBytes.Length + 1, messageBytes.Length);

                                int numOfBytes = clientSocket.Send(dataToSend);
                                numOfBytes = clientSocket.Receive(buffer);

                                if (message.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa serverom.");
                                    break;
                                }

                                if (numOfBytes == 0)
                                {
                                    Console.WriteLine("\nServer je zavrsio sa radom");
                                    break;
                                }

                                string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, numOfBytes);
                                Console.WriteLine($"Primljen enkriptovani odgovor: {encryptedMessage}");
                                string decryptedMessage = bitwise.Decrypt(encryptedMessage);
                                Console.WriteLine($"Dekriptovani odgovor od servera: {decryptedMessage}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa serverom.");
                                    break;
                                }
                            }
                            catch (SocketException ex)
                            {
                                Console.WriteLine($"Doslo je do greske tokom slanja poruke! \n{ex}");
                                break;
                            }
                        }
                    }
                    #endregion

                    #region Viznerov algoritam

                    if (algorithm == "VIZNER")
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        while (true)
                        {
                            try
                            {
                                Console.Write("\nUnesite poruku ('kraj' za izlaz): ");
                                string message = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(message))
                                    continue;


                                Vignere vignere = new Vignere();
                                string encryptingMessage = vignere.Encrypt(message);

                                string key = vignere.Key;
                                NacinKomunikacije nacin = new NacinKomunikacije(algorithm, key);

                                byte[] objectBytes;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    formatter.Serialize(ms, nacin);
                                    objectBytes = ms.ToArray();
                                }

                                byte[] messageBytes = Encoding.UTF8.GetBytes(encryptingMessage);
                                byte[] dataToSend = new byte[objectBytes.Length + messageBytes.Length + 1];
                                Array.Copy(objectBytes, 0, dataToSend, 0, objectBytes.Length);
                                dataToSend[objectBytes.Length] = (byte)'|'; // Separator
                                Array.Copy(messageBytes, 0, dataToSend, objectBytes.Length + 1, messageBytes.Length);

                                int numOfBytes = clientSocket.Send(dataToSend);
                                numOfBytes = clientSocket.Receive(buffer);

                                if (message.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa serverom.");
                                    break;
                                }

                                if (numOfBytes == 0)
                                {
                                    Console.WriteLine("\nServer je zavrsio sa radom");
                                    break;
                                }

                                string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, numOfBytes);
                                Console.WriteLine($"Primljen enkriptovani odgovor: {encryptedMessage}");
                                string decryptedMessage = vignere.Decrypt(encryptedMessage);
                                Console.WriteLine($"Dekriptovani odgovor od servera: {decryptedMessage}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa serverom.");
                                    break;
                                }
                            }
                            catch (SocketException ex)
                            {
                                Console.WriteLine($"Doslo je do greske tokom slanja poruke! \n{ex}");
                                break;
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #region Zatvaranje uticnice

                    Console.WriteLine("TCP klijent zavrsava sa radom.");
                    clientSocket.Close();

                    #endregion

                }

                #endregion

                CheckReconnection();

            }
        }

        #region Provera unosa
        static string CheckValidInputProtocol()
        {
            Console.Write("Unesite protokol za rad servera (TCP ili UDP): ");
            string input = Console.ReadLine().Trim().ToUpper();

            while (input != "UDP" && input != "TCP")
            {
                Console.Write("\nGRESKA! Unesite protokol za rad servera (TCP ili UDP): ");
                input = Console.ReadLine().Trim().ToUpper();
            }
            return input;
        }
        static string CheckValidInputAlgorithm()
        {
            Console.Write("Unesite algoritam za enkripciju poruke (Homofono/Bitovi/Vizner): ");
            string input = Console.ReadLine().Trim().ToUpper();

            while (input != "HOMOFONO" && input != "BITOVI" && input != "VIZNER")
            {
                Console.Write("\nGRESKA! Unesite algoritam za enkripciju poruke (Homofono/Bitovi/Vizner): ");
                input = Console.ReadLine().Trim().ToUpper();
            }
            return input;
        }

        static void CheckReconnection()
        {
            Console.Write("\nDa li zelite ponovo da uspostavite komunikaciju sa serverom? (da/ne): ");
            string answer = Console.ReadLine().Trim().ToLower();

            while (answer != "da" && answer != "ne")
            {
                Console.Write("\nGRESKA: Unesite samo 'da' ili 'ne': ");
                answer = Console.ReadLine().Trim().ToLower();
            }

            if (answer == "ne")
            {
                Console.WriteLine("\nKlijent zavrsava sa radom.");
                Environment.Exit(0);
            }

            Console.Clear();
        }

        #endregion
    }
}
