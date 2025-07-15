using Algorithms;
using Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        if (serverSocket.Poll(5 * 1000 * 1000, SelectMode.SelectRead))
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
                        else
                        {
                            Console.WriteLine("Jos uvek cekam...");
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
                    serverSocket.Listen(10);
                    serverSocket.Blocking = false;

                    Console.WriteLine($"\nServer je stavljen u stanje osluskivanja i ocekuje komunikaciju na \"{serverEP}\"\n");

                    List<Socket> clientSockets = new List<Socket>();
                    Dictionary<Socket, IPEndPoint> clientEndPoints = new Dictionary<Socket, IPEndPoint>();
                    List<NacinKomunikacije> komunikacije = new List<NacinKomunikacije>();
                    BinaryFormatter formatter = new BinaryFormatter();
                    byte[] buffer = new byte[1024];

                    #endregion

                    #region Komunikacija

                    while (true)
                    {
                        // pravimo novog klijenta
                        if (serverSocket.Poll(1000, SelectMode.SelectRead))
                        {
                            Socket newClient = serverSocket.Accept();
                            newClient.Blocking = false;
                            clientSockets.Add(newClient);

                            IPEndPoint EndPoint = newClient.RemoteEndPoint as IPEndPoint;
                            clientEndPoints[newClient] = EndPoint;

                            Console.WriteLine($"Novi klijent povezan: {EndPoint.Address}:{EndPoint.Port}\n");
                        }

                        // komunikacija sa svakim klijentom
                        foreach (var client in clientSockets.ToList()) // kopija liste zbog brisanja
                        {
                            try
                            {
                                if (client.Poll(1000, SelectMode.SelectRead))
                                {
                                    int numOfBytes = client.Receive(buffer);

                                    if (numOfBytes == 0)
                                    {
                                        Console.WriteLine($"Klijent {client.RemoteEndPoint} prekinuo vezu.");
                                        client.Close();
                                        clientSockets.Remove(client);
                                        clientEndPoints.Remove(client);
                                        continue;
                                    }

                                    int separatorIndex = Array.IndexOf(buffer, (byte)'|');
                                    if (separatorIndex == -1)
                                    {
                                        Console.WriteLine("Greska: Separator nije pronadjen.");
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
                                        nacin.ClientEndPoint = clientEndPoints[client];
                                        komunikacije.Add(nacin);
                                    }

                                    string encryptedMessage = Encoding.UTF8.GetString(messageBytes);
                                    Console.WriteLine($"Primljena enkriptovana poruka: {encryptedMessage}");
                                    Console.WriteLine($"Adresa posiljaoca: {nacin.ClientEndPoint}");
                                    Console.WriteLine($"Algoritam: {nacin.Algorithm}");
                                    Console.WriteLine($"Korisceni kljucevi: {nacin.UsedKey}");

                                    PrintCommunicationList(komunikacije);

                                    // --- ODAVDE cemo razlikovati logiku shodno tome koji algoritam se koristi ---
                                    string decryptedMessage = "";
                                    string response = "";

                                    if (nacin.Algorithm == "HOMOFONO")
                                    {
                                        Homophonic homophonic = new Homophonic();
                                        decryptedMessage = homophonic.Decrypt(encryptedMessage).Trim();

                                        Console.WriteLine($"\nDekriptovana poruka: {decryptedMessage}");

                                        if (decryptedMessage.ToLower() == "kraj")
                                        {
                                            Console.WriteLine("Klijent je zatvorio konekciju.");
                                            client.Close();
                                            clientSockets.Remove(client);
                                            clientEndPoints.Remove(client);
                                            continue;
                                        }

                                        Console.Write("\nUnesite odgovor klijentu: ");
                                        response = Console.ReadLine();

                                        string encryptedResponse = homophonic.Encrypt(response);
                                        client.Send(Encoding.UTF8.GetBytes(encryptedResponse));
                                    }

                                    else if (nacin.Algorithm == "BITOVI")
                                    {
                                        Bitwise bitwise = new Bitwise();
                                        decryptedMessage = bitwise.Decrypt(encryptedMessage).Trim();

                                        Console.WriteLine($"\nDekriptovana poruka: {decryptedMessage}");

                                        if (decryptedMessage.ToLower() == "kraj")
                                        {
                                            Console.WriteLine("Klijent je zatvorio konekciju.");
                                            client.Close();
                                            clientSockets.Remove(client);
                                            clientEndPoints.Remove(client);
                                            continue;
                                        }

                                        Console.Write("\nUnesite odgovor klijentu: ");
                                        response = Console.ReadLine();

                                        string encryptedResponse = bitwise.Encrypt(response);
                                        client.Send(Encoding.UTF8.GetBytes(encryptedResponse));
                                    }

                                    else if (nacin.Algorithm == "VIZNER")
                                    {
                                        Vignere vignere = new Vignere();
                                        decryptedMessage = vignere.Decrypt(encryptedMessage).Trim();

                                        Console.WriteLine($"\nDekriptovana poruka: {decryptedMessage}");

                                        if (decryptedMessage.ToLower() == "kraj")
                                        {
                                            Console.WriteLine("Klijent je zatvorio konekciju.");
                                            client.Close();
                                            clientSockets.Remove(client);
                                            clientEndPoints.Remove(client);
                                            continue;
                                        }

                                        Console.Write("\nUnesite odgovor klijentu: ");
                                        response = Console.ReadLine();

                                        string encryptedResponse = vignere.Encrypt(response);
                                        client.Send(Encoding.UTF8.GetBytes(encryptedResponse));
                                    }

                                    if (response.ToLower() == "kraj")
                                    {
                                        Console.WriteLine("Zatvaranje veze sa klijentom po zahtevu servera.");
                                        client.Close();
                                        clientSockets.Remove(client);
                                        clientEndPoints.Remove(client);
                                    }
                                }
                            }
                            catch (SocketException ex)
                            {
                                Console.WriteLine($"Greska u komunikaciji sa klijentom: {ex.Message}");
                                client.Close();
                                clientSockets.Remove(client);
                                clientEndPoints.Remove(client);
                            }
                            
                        }

                        
                    }
                    #endregion

                    #region Zatvaranje uticnice

                    Console.WriteLine("Server zavrsava sa radom.");
                    foreach (var client in clientSockets)
                    {
                        client.Close();
                    }
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
            Console.WriteLine("\n| {0,-25} | {1,-12} | {2,-97} |",
                "Client EndPoint", "Algoritam", "Korisceni kljuc");
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
                    Console.WriteLine("| {0,-25} | {1,-12} | {2,-97} |",
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
            Console.Write("\nDa li zelite ponovo da uspostavite komunikaciju sa klijentom? (da/ne): ");
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
