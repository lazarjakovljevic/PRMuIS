using Algorithms;
using Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Server
{
    internal class Server
    {
        static void Main(string[] args)
        {
            int width = 150;
            int height = 30;
            Console.SetWindowSize(width, height);

            while (true)
            {
                #region Odabir protokola

                string protocol = CheckValidInputProtocol();
                Console.WriteLine($"Izabrani protokol: {protocol}");

                #endregion

                #region UDP

                if (protocol == "UDP")
                {
                    #region Inicijalizacija i povezivanje

                    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50001);
                    EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    serverSocket.Bind(serverEP);

                    Console.WriteLine($"\nServer je stavljen u stanje osluskivanja i ocekuje komunikaciju na \"{serverEP}\"\n");

                    #endregion

                    #region Komunikacija

                    byte[] buffer = new byte[1024];
                    BinaryFormatter formatter = new BinaryFormatter();
                    List<NacinKomunikacije> communication = new List<NacinKomunikacije>();

                    while (true)
                    {
                        try
                        {
                            int numOfBytes = serverSocket.ReceiveFrom(buffer, ref clientEndPoint);

                            if (numOfBytes == 0)
                            {
                                Console.WriteLine("\nKlijent je zavrsio sa radom.");
                                break;
                            }

                            int separatorIndex = Array.IndexOf(buffer, (byte)'|');

                            if (separatorIndex == -1)
                            {
                                Console.WriteLine("GRESKA: Separator nije pronadjen.");
                                continue;
                            }

                            byte[] objectBytes = new byte[separatorIndex];
                            Array.Copy(buffer, 0, objectBytes, 0, separatorIndex);

                            int messageLength = numOfBytes - separatorIndex - 1;
                            byte[] messageBytes = new byte[messageLength];
                            Array.Copy(buffer, separatorIndex + 1, messageBytes, 0, messageLength);

                            NacinKomunikacije nacin;
                            using (MemoryStream ms = new MemoryStream(objectBytes))
                            {
                                nacin = (NacinKomunikacije)formatter.Deserialize(ms);
                                nacin.ClientEndPoint = clientEndPoint;
                                communication.Add(nacin);
                            }

                            string encryptedMessage = Encoding.UTF8.GetString(messageBytes);
                            Console.WriteLine($"Primljena enkriptovana poruka: {encryptedMessage}");
                            Console.WriteLine($"Adresa posiljaoca: {nacin.ClientEndPoint}");
                            Console.WriteLine($"Algoritam: {nacin.Algorithm}");
                            Console.WriteLine($"Korisceni kljucevi: \n{nacin.UsedKey}");

                            PrintCommunicationList(communication);

                            //--- ODAVDE cemo razlikovati logiku shodno tome koji algoritam se koristi ---
                            #region Homofono sifrovanje

                            if (nacin.Algorithm == "HOMOFONO")
                            {
                                Homophonic homophonic = new Homophonic();
                                string decryptedMessage = homophonic.Decrypt(encryptedMessage).Trim();
                                Console.WriteLine($"\nDekriptovana poruka od klijenta \"{clientEndPoint}\": {decryptedMessage.ToLower()}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }

                                Console.Write("\nUnesite odgovor klijentu: ");
                                string response = Console.ReadLine();

                                string encryptingMessage = homophonic.Encrypt(response);
                                byte[] responseBytes = Encoding.UTF8.GetBytes(encryptingMessage);
                                serverSocket.SendTo(responseBytes, clientEndPoint);

                                if (response.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }
                            }
                            #endregion

                            #region Sifrovanje upotrebom bitova

                            if (nacin.Algorithm == "BITOVI")
                            {
                                Bitwise bitwise = new Bitwise();
                                string decryptedMessage = bitwise.Decrypt(encryptedMessage).Trim();
                                Console.WriteLine($"\nDekriptovana poruka od klijenta \"{clientEndPoint}\": {decryptedMessage}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }

                                Console.Write("\nUnesite odgovor klijentu: ");
                                string response = Console.ReadLine();

                                string encryptingMessage = bitwise.Encrypt(response);
                                byte[] responseBytes = Encoding.UTF8.GetBytes(encryptingMessage);
                                serverSocket.SendTo(responseBytes, clientEndPoint);

                                if (response.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }
                            }
                            #endregion

                            #region Viznerov algoritam

                            if (nacin.Algorithm == "VIZNER")
                            {
                                Vignere vignere = new Vignere();
                                string decryptedMessage = vignere.Decrypt(encryptedMessage).Trim();
                                Console.WriteLine($"\nDekriptovana poruka od klijenta \"{clientEndPoint}\": {decryptedMessage}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }

                                Console.Write("\nUnesite odgovor klijentu: ");
                                string response = Console.ReadLine();

                                string encryptingMessage = vignere.Encrypt(response);
                                byte[] responseBytes = Encoding.UTF8.GetBytes(encryptingMessage);
                                serverSocket.SendTo(responseBytes, clientEndPoint);

                                if (response.ToLower() == "kraj")
                                {
                                    Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                    break;
                                }
                            }
                            #endregion
                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine($"Doslo je do greske tokom prijema poruke.\n{ex}");
                        }
                    }
                    #endregion

                    #region Zatvaranje uticnice

                    serverSocket.Close();
                    Console.WriteLine("UDP Server zavrsio sa radom.");

                    #endregion
                }

                #endregion

                #region TCP

                else if (protocol == "TCP")
                {
                    #region Inicijalizacija i povezivanje

                    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50001);

                    serverSocket.Bind(serverEP);
                    serverSocket.Listen(2);

                    Console.WriteLine($"\nServer je stavljen u stanje osluskivanja i ocekuje komunikaciju na \"{serverEP}\"");

                    Socket acceptedSocket = serverSocket.Accept();

                    IPEndPoint clientEndPoint = acceptedSocket.RemoteEndPoint as IPEndPoint;
                    Console.WriteLine($"\nPovezao se novi klijent!");
                    Console.WriteLine($"IP adresa:{clientEndPoint.Address,-10}");
                    Console.WriteLine($"Port:{clientEndPoint.Port,-10}\n\n");
                    #endregion

                    #region Komunikacija

                    byte[] buffer = new byte[1024];
                    BinaryFormatter formatter = new BinaryFormatter();
                    List<NacinKomunikacije> communications = new List<NacinKomunikacije>();

                    while (true)
                    {
                        try
                        {
                            int numOfBytes = acceptedSocket.Receive(buffer);

                            if (numOfBytes == 0)
                            {
                                Console.WriteLine("\nKlijent je zavrsio sa radom");
                                break;
                            }

                            int separatorIndex = Array.IndexOf(buffer, (byte)'|');
                            if (separatorIndex == -1)
                            {
                                Console.WriteLine("Greska: Separator nije pronadjen.");
                                continue;
                            }

                            // Bajtovi za objekat
                            byte[] objectBytes = new byte[separatorIndex];
                            Array.Copy(buffer, 0, objectBytes, 0, separatorIndex);

                            // Bajtovi za poruku
                            int messageLength = numOfBytes - separatorIndex - 1;
                            byte[] messageBytes = new byte[messageLength];
                            Array.Copy(buffer, separatorIndex + 1, messageBytes, 0, messageLength);

                            // Deserijalizacija objekta
                            NacinKomunikacije nacin;
                            using (MemoryStream ms = new MemoryStream(objectBytes))
                            {
                                nacin = (NacinKomunikacije)formatter.Deserialize(ms);
                                nacin.ClientEndPoint = clientEndPoint;
                                communications.Add(nacin);
                            }

                            string encryptedMessage = Encoding.UTF8.GetString(messageBytes);
                            Console.WriteLine($"Primljena enkriptovana poruka: {encryptedMessage}");
                            Console.WriteLine($"Adresa posiljaoca: {nacin.ClientEndPoint}");
                            Console.WriteLine($"Algoritam: {nacin.Algorithm}");
                            Console.WriteLine($"Korisceni kljucevi: \n{nacin.UsedKey}");

                            PrintCommunicationList(communications);

                            //--- ODAVDE cemo razlikovati logiku shodno tome koji algoritam se koristi ---

                            #region Homofono sifrovanje

                            if (nacin.Algorithm == "HOMOFONO")
                            {
                                Homophonic homophonic = new Homophonic();
                                //string decryptedMessage = Encoding.UTF8.GetString(buffer, 0, numOfBytes);
                                string decryptedMessage = homophonic.Decrypt(encryptedMessage);
                                Console.WriteLine($"\nDekriptovana poruka od klijenta \"{clientEndPoint}\": {decryptedMessage.ToLower()}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa klijentom.");
                                    break;
                                }

                                Console.Write("\nUnesite odgovor klijentu: ");
                                string response = Console.ReadLine();

                                string encryptingMessage = homophonic.Encrypt(response);
                                numOfBytes = acceptedSocket.Send(Encoding.UTF8.GetBytes(encryptingMessage));

                                if (response.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa klijentom.");
                                    break;
                                }
                            }

                            #endregion

                            #region Sifrovanje upotrebom bitova

                            if (nacin.Algorithm == "BITOVI")
                            {
                                Bitwise bitwise = new Bitwise();
                                //string decryptedMessage = Encoding.UTF8.GetString(buffer, 0, numOfBytes);
                                string decryptedMessage = bitwise.Decrypt(encryptedMessage);
                                Console.WriteLine($"\nDekriptovana poruka od klijenta \"{clientEndPoint}\": {decryptedMessage}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa klijentom.");
                                    break;
                                }

                                Console.Write("\nUnesite odgovor klijentu: ");
                                string response = Console.ReadLine();

                                string encryptingMessage = bitwise.Encrypt(response);
                                numOfBytes = acceptedSocket.Send(Encoding.UTF8.GetBytes(encryptingMessage));

                                if (response.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa klijentom.");
                                    break;
                                }
                            }

                            #endregion

                            #region Vigenerovo sifrovanje

                            if (nacin.Algorithm == "VIZNER")
                            {
                                Vignere vignere = new Vignere();
             
                                string decryptedMessage = vignere.Decrypt(encryptedMessage);
                                Console.WriteLine($"\nDekriptovana poruka od klijenta \"{clientEndPoint}\": {decryptedMessage}");

                                if (decryptedMessage.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa klijentom.");
                                    break;
                                }

                                Console.Write("\nUnesite odgovor klijentu: ");
                                string response = Console.ReadLine();

                                string encryptingMessage = vignere.Encrypt(response);
                                numOfBytes = acceptedSocket.Send(Encoding.UTF8.GetBytes(encryptingMessage));

                                if (response.ToLower() == "kraj")
                                {
                                    Console.WriteLine("\nPrekinuta komunikacija sa klijentom.");
                                    break;
                                }
                            }

                            #endregion
                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine($"\nDoslo je do greske prilikom prijema poruke! \n{ex}");
                            break;
                        }
                    }
                    #endregion

                    #region Zatvaranje uticnice

                    Console.WriteLine("Server zavrsava sa radom.");
                    acceptedSocket.Close();
                    serverSocket.Close();

                    #endregion
                }
                #endregion

                CheckReconnection();

            }
        }
        #region Ispis liste komunikacija
        public static void PrintCommunicationList(List<NacinKomunikacije> communications)
        {
            Console.WriteLine("| {0,-25} | {1,-12} | {2,-97} |",
                "Client EndPoint", "Algorithm", "Used Key");
            Console.WriteLine(new string('-', 143));

            foreach (var communication in communications)
            {
                string clientEndpoint = communication.ClientEndPoint.ToString();
                string algorithm = communication.Algorithm;
                string usedKey = communication.UsedKey;

                if (usedKey.Contains("~ Sekundarni kljuc: "))
                {
                    var parts = usedKey.Split(new[] { "~ Sekundarni kljuc: " }, StringSplitOptions.None);
                    string primaryKey = parts[0].Trim();
                    string secondaryKey = parts.Length > 1 ? parts[1].Trim() : "";

                    Console.WriteLine("| {0,-25} | {1,-12} | {2,-50} |",
                        clientEndpoint, algorithm, primaryKey);

                    Console.WriteLine("| {0,-25} | {1,-12} | {2,-50} |",
                        "", "", "~ Sekundarni kljuc: " + secondaryKey);
                }
                else
                {
                    Console.WriteLine("| {0,-25} | {1,-12} | {2,-50} |",
                        clientEndpoint, algorithm, usedKey);
                }
            }

        }

        #endregion

        #region Provera unosa
        static string CheckValidInputProtocol()
        {
            Console.Write("Unesite protokol za rad servera (TCP ili UDP): ");
            string input = Console.ReadLine().Trim().ToUpper();

            while (input != "UDP" && input != "TCP")
            {
                Console.Write("\nGRESKA! Unesite protokol za rad servera (TCP ili UDP):  ");
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
