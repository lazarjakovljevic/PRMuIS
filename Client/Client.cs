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
            #region Inicijalizacija i povezivanje
            string protocol = CheckValidInput();
            Console.WriteLine($"Izabrali ste protocol: {protocol}");

            if (protocol == "UDP")
            {
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 50001);
                EndPoint serverResponseEndPoint = new IPEndPoint(IPAddress.Any, 0);

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

                        if (message.ToLower() == "kraj") break;

                        byte[] buffer = new byte[1024];
                        int receivedBytes = clientSocket.ReceiveFrom(buffer, ref serverResponseEndPoint);

                        string response = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                        Console.WriteLine($"Odgovor od servera: {response}");
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine($"Doslo je do greske tokom slanja poruke: \n{ex}");
                    }
                }
                #region Zatvaranje UDP uticnice

                Console.WriteLine("UDP Client zavrsava sa radom.");
                clientSocket.Close();
                Console.ReadKey();

                #endregion
            }
            else if (protocol == "TCP")
            {
                //TODO
            }
            #endregion
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
