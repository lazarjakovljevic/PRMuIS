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
            #region Inicijalizacija i povezivanje

            string protocol = CheckValidInput();
            Console.WriteLine($"Izabrali ste protocol: {protocol}");

            if(protocol == "UDP")
            {
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50001);
                serverSocket.Bind(serverEP);

                Console.WriteLine($"Server je stavljen u stanje osluskivanja i ocekuje komunikaciju na \"{serverEP}\"");

                EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int receivedBytes = serverSocket.ReceiveFrom(buffer, ref clientEndPoint);
                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes); //ubaciti dekript
                    Console.WriteLine($"Poruka od {clientEndPoint}: {message}");

                    if (message.ToLower() == "kraj") break;

                    string response = $"Server odgovor, poruka: {message} mora biti kriptovana"; //ubaciti kript
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    serverSocket.SendTo(responseBytes, clientEndPoint);
                }
            }
            else if(protocol == "TCP")
            {


            }

            #endregion



            Console.ReadKey();
        }

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

    }
}
