using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Server
    {
        static void Main(string[] args)
        {
            while (true)
            {
                #region Odabir protokola

                string protocol = CheckValidInput();
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

                    Console.WriteLine($"\nServer je stavljen u stanje osluskivanja i ocekuje komunikaciju na \"{serverEP}\"");

                    #endregion

                    #region Komunikacija

                    byte[] buffer = new byte[1024];

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

                            string message = Encoding.UTF8.GetString(buffer, 0, numOfBytes).Trim(); //ubaciti dekript
                            Console.WriteLine($"\nPoruka od \"{clientEndPoint}\": {message}");

                            if (message.ToLower() == "kraj")
                            {
                                Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                break;
                            }

                            Console.Write("\nUnesi poruku: ");
                            string response = Console.ReadLine();

                            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                            serverSocket.SendTo(responseBytes, clientEndPoint);

                            if (response.ToLower() == "kraj")
                            {
                                Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                break;
                            }
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
                    Console.WriteLine($"Port:{clientEndPoint.Port,-10}");
                    #endregion

                    #region Komunikacija
                    byte[] buffer = new byte[1024];

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

                            string message = Encoding.UTF8.GetString(buffer, 0, numOfBytes);
                            Console.WriteLine($"\nPoruka od {clientEndPoint}: {message}");

                            if (message.ToLower() == "kraj")
                            {
                                Console.WriteLine("\nPrekinuta komunikacija sa klijentom.");
                                break;
                            }

                            Console.Write("\nUnesi poruku: ");
                            string response = Console.ReadLine();

                            numOfBytes = acceptedSocket.Send(Encoding.UTF8.GetBytes(response));

                            if (response.ToLower() == "kraj")
                            {
                                Console.WriteLine("\nPrekinuta komunikacija sa klijentom.");
                                break;
                            }
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

                #region Ponovna komunikacija

                Console.WriteLine("\nDa li zelite ponovo da uspostavite komunikaciju sa klijentom? (da/ne)");
                string answer = Console.ReadLine().Trim().ToLower();

                while (answer != "da" && answer != "ne")
                {
                    Console.Write("\nGRESKA! Unesite samo 'da' ili 'ne': ");
                    answer = Console.ReadLine().Trim().ToLower();
                }

                if (answer == "ne")
                {
                    Console.WriteLine("\nServer zavrsava sa radom.");
                    break;
                }

                Console.Clear();

                #endregion

            }
        }

        #region Provera unosa
        static string CheckValidInput()
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
        #endregion
    }
}
