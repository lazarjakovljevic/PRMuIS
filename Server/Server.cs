using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
                Console.WriteLine($"Izabrali ste protocol: {protocol}");

                #endregion

                #region UDP

                if (protocol == "UDP")
                {
                    #region Inicijalizacija i povezivanje

                    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50001);
                    serverSocket.Bind(serverEP);

                    Console.WriteLine($"Server je stavljen u stanje osluskivanja i ocekuje komunikaciju na \"{serverEP}\"");

                    EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

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
                                Console.WriteLine("Klijent je zavrsio sa radom");
                                break;
                            }

                            string message = Encoding.UTF8.GetString(buffer, 0, numOfBytes); //ubaciti dekript
                            Console.WriteLine($"Poruka od {clientEndPoint}: {message}");

                            if (message.ToLower() == "kraj")
                                break;

                            Console.Write("\nUnesite poruku: ");
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
                            Console.WriteLine($"Doslo je do greske tokom prijema poruke: \n{ex}");
                        }
                    }

                    #endregion

                    #region Zatvaranje uticnice

                    serverSocket.Close();
                    Console.WriteLine("UDP Server završio sa radom.");

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

                    Console.WriteLine($"Server je stavljen u stanje osluskivanja i ocekuje komunikaciju na {serverEP}");

                    Socket acceptedSocket = serverSocket.Accept();

                    IPEndPoint clientEndPoint = acceptedSocket.RemoteEndPoint as IPEndPoint;
                    Console.WriteLine($"Povezao se novi klijent! Njegova adresa: {clientEndPoint}");
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
                                Console.WriteLine("Klijent je zavrsio sa radom");
                                break;
                            }

                            string message = Encoding.UTF8.GetString(buffer, 0, numOfBytes);
                            Console.WriteLine($"Poruka od {clientEndPoint}: {message}");

                            if (message.ToLower() == "kraj")
                            {
                                Console.WriteLine("Prekinuta komunikacija sa klijentom.");
                                break;
                            }

                            Console.Write("\nUnesite poruku: ");
                            string response = Console.ReadLine();

                            numOfBytes = acceptedSocket.Send(Encoding.UTF8.GetBytes(response));

                            if (response.ToLower() == "kraj")
                            {
                                Console.WriteLine("Prekinuta komunikacija sa klijentom.");
                                break;
                            }
                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine($"Doslo je do greske: {ex}");
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

                if (answer == "ne")
                {
                    Console.WriteLine("\nServer zavrsava sa radom.");
                    break;
                }

                #endregion

            }
        }

        #region Provera unosa
        static string CheckValidInput()
        {
            Console.Write("Unesite zeljeni protocol za rad Servera (TCP ili UDP): ");
            string input = Console.ReadLine().Trim().ToUpper();

            while (input != "UDP" && input != "TCP")
            {
                Console.Write("\nGRESKA! Unesite zeljeni protocol za rad Servera (TCP ili UDP): ");
                input = Console.ReadLine().Trim().ToUpper();
            }
            return input;
        }
        #endregion
    }
}
