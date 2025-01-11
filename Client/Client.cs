﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
                Console.WriteLine($"Izabrani protokol: {protocol}");

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
                            Console.Write("Unesite poruku ('kraj' za izlaz): ");
                            string message = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(message))
                                continue;

                            
                            //ubaciti kriptovanje

                            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                            clientSocket.SendTo(messageBytes, serverEndPoint);


                            if (message.ToLower() == "kraj")
                            {
                                Console.WriteLine("Prekinuta komunikacija sa serverom.");
                                break;
                            }

                            byte[] buffer = new byte[1024];
                            int receivedBytes = clientSocket.ReceiveFrom(buffer, ref serverResponseEndPoint);

                            string response = Encoding.UTF8.GetString(buffer, 0, receivedBytes).Trim();
                            Console.WriteLine($"Poruka od servera: {response}");

                            if (response.ToLower() == "kraj")
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
                    while (true)
                    {
                        try
                        {
                            Console.Write("\nUnesite poruku ('kraj' za izlaz): ");
                            string message = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(message))
                                continue;

                            int numOfBytes = clientSocket.Send(Encoding.UTF8.GetBytes(message));
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

                            string response = Encoding.UTF8.GetString(buffer, 0, numOfBytes);
                            Console.WriteLine($"Poruka od servera: {response}");

                            if (response.ToLower() == "kraj")
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
                    #endregion

                    #region Zatvaranje uticnice

                    Console.WriteLine("UDP klijent zavrsava sa radom.");
                    clientSocket.Close();

                    #endregion
                }

                #endregion

                #region Ponovna komunikacija

                Console.Write("\nDa li zelite ponovo da uspostavite komunikaciju sa serverom? (da/ne): ");
                string answer = Console.ReadLine().Trim().ToLower();

                while (answer != "da" && answer != "ne")
                {
                    Console.Write("\nGRESKA! Unesite samo 'da' ili 'ne': ");
                    answer = Console.ReadLine().Trim().ToLower();
                }

                if (answer == "ne")
                {
                    Console.WriteLine("\nKlijent zavrsava sa radom.");
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
                Console.Write("\nGRESKA! Unesite protokol za rad servera (TCP ili UDP): ");
                input = Console.ReadLine().Trim().ToUpper();
            }
            return input;
        }
        #endregion
    }
}
