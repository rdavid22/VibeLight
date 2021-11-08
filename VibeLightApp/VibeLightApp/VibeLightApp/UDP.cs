using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace VibelightApp
{
    public class UDP
    {
        private readonly Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private readonly int Port = 4210;


        public UDP()
        {
            Socket.EnableBroadcast = true;
        }

        public async Task SendAsync(string ip, string message)
        {
            await Task.Run(() =>
            {
                byte[] send_buffer = Encoding.ASCII.GetBytes(message);
                Socket.SendTo(send_buffer, new IPEndPoint(IPAddress.Parse(ip), Port));
            });
        }

        public async Task<string> BroadcastAsyncAndGetResponse(string message, int ListenTimeout)
        {
            string responses = "";
            await Task.Run(() =>
            {
                byte[] send_buffer = Encoding.ASCII.GetBytes(message);

                Socket.SendTo(send_buffer, new IPEndPoint(IPAddress.Broadcast, Port));

                try
                {
                    for (int i = 0; i < 100; i++) // legfeljebb 100 uzenet és ListenTimeout nyi ido utan kilep
                    {
                        using (UdpClient udpClient = new UdpClient())
                        {
                            udpClient.Client.ReceiveTimeout = ListenTimeout; // legutobbi uzenet utan legfeljebb mennyit varjon

                            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
                            IPEndPoint endpoint = new IPEndPoint(0, 0);
                            byte[] recv_buffer = udpClient.Receive(ref endpoint);
                            responses += Encoding.UTF8.GetString(recv_buffer) + '\n';
                        }
                    }
                }
                catch (Exception EX) { Console.WriteLine(EX); }
            });
            return responses;
        }
        public string BroadcastAndGetResponse(string message, int ListenTimeout)
        {
            string responses = "";

            byte[] send_buffer = Encoding.ASCII.GetBytes(message);

            Socket.SendTo(send_buffer, new IPEndPoint(IPAddress.Broadcast, Port));

            try
            {
                for (int i = 0; i < 100; i++) // legfeljebb 100 uzenet és ListenTimeout nyi ido utan kilep
                {
                    using (UdpClient udpClient = new UdpClient())
                    {
                        udpClient.Client.ReceiveTimeout = ListenTimeout; // legutobbi uzenet utan legfeljebb mennyit varjon

                        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
                        IPEndPoint endpoint = new IPEndPoint(0, 0);
                        byte[] recv_buffer = udpClient.Receive(ref endpoint);
                        responses += Encoding.UTF8.GetString(recv_buffer) + '\n';
                    }
                }
            }
            catch (Exception EX) { Console.WriteLine(EX); }

            return responses;
        }

        public async Task<bool> ValidationSendAsync(string ip, string message, int RecieveTimeout)
        {
            bool MessageArrived = false;

            await Task.Run(() =>
            {
                byte[] send_buffer = Encoding.ASCII.GetBytes(message);

                Socket.SendTo(send_buffer, new IPEndPoint(IPAddress.Parse(ip), Port));

                try
                {
                    using (UdpClient udpClient = new UdpClient())
                    {
                        udpClient.Client.ReceiveTimeout = RecieveTimeout;

                        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
                        IPEndPoint endpoint = new IPEndPoint(0, 0);
                        byte[] recv_buffer = udpClient.Receive(ref endpoint);

                        MessageArrived = true;
                    }
                }
                catch (Exception EX)
                {
                    Console.WriteLine(EX);
                    MessageArrived = false;
                }
            });

            return MessageArrived;
        }
    }
}
