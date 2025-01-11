using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Client
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

                    Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 50001);
                    EndPoint serverResponseEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    #endregion

                    #region Komunikacija

                    while (true)
                    {
                        try
                        {
                            Console.Write("Unesite poruku za server (ili 'kraj' za izlaz): ");
                            string message = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(message)) continue;

                            //ubaciti kriptovanje

                            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                            clientSocket.SendTo(messageBytes, serverEndPoint);  //parametri?

                            if (message.ToLower() == "kraj")
                            {
                                Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                break;
                            }

                            byte[] buffer = new byte[1024];
                            int receivedBytes = clientSocket.ReceiveFrom(buffer, ref serverResponseEndPoint);

                            string response = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                            Console.WriteLine($"Server: {response}");
                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine($"Doslo je do greske tokom slanja poruke: \n{ex}");
                        }
                    }

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

                    IPEndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 50001);
                    byte[] buffer = new byte[1024];

                    try
                    {
                        clientSocket.Connect(serverEP);
                        Console.WriteLine("\nKlijent je uspesno povezan sa serverom!");
                    }

                    catch (SocketException ex)
                    {
                        Console.WriteLine($"\nGRESKA: Doslo je do greske prilikom povezivanja sa serverom:\n{ex}");
                        break;
                    }
                    
                    #endregion

                    #region Komunikacija
                    while (true)
                    {
                        try
                        {
                            Console.Write("\nUnesite poruku: ");
                            string message = Console.ReadLine();

                            if (message.ToLower() == "kraj")
                            {
                                Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                break;
                            }

                            int numOfBytes = clientSocket.Send(Encoding.UTF8.GetBytes(message));

                            numOfBytes = clientSocket.Receive(buffer);

                            if (numOfBytes == 0)
                            {
                                Console.WriteLine("Server je zavrsio sa radom");
                                break;
                            }

                            string response = Encoding.UTF8.GetString(buffer);

                            Console.WriteLine($"Server: {response}");

                            if (response.ToLower() == "kraj")
                            {
                                Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                break;
                            }

                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine($"Doslo je do greske tokom slanja:\n{ex}");
                            break;
                        }
                    }
                    #endregion

                    #region Zatvaranje uticnice

                    Console.WriteLine("UDP klijent zavrsava sa radom.");
                    clientSocket.Close();

                    #endregion
                }

                #endregion

                #region Ponovna komunikacija

                Console.WriteLine("\nDa li zelite ponovo da uspostavite komunikaciju sa serverom? (da/ne)");
                string answer = Console.ReadLine().Trim().ToLower();

                if (answer == "ne")
                {
                    Console.WriteLine("\nKlijent završava sa radom.");
                    break;
                }

                #endregion
            }
        }

        #region Provera unosa
        static string CheckValidInput()
        {
            Console.Write("Unesite zeljeni protocol za slanje poruke Serveru (TCP ili UDP): ");
            string input = Console.ReadLine().Trim().ToUpper();

            while (input != "UDP" && input != "TCP")
            {
                Console.Write("\nGRESKA! Unesite zeljeni protocol za slanje poruke Serveru (TCP ili UDP): ");
                input = Console.ReadLine().Trim().ToUpper();
            }
            return input;
        }
        #endregion
    }
}
