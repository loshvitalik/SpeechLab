using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace WeatherLabServer
{
    class Server
    {
        private RouterSocket server;//10.97.129.111

        public Server()
        {
            server = new RouterSocket("@tcp://10.97.129.111:2228");

        }

        public void Start()
        {
            while (true)
            {
                var clientMessage = server.ReceiveMultipartMessage();
                PrintFrames("Server reciving",clientMessage);
                //foreach (var b in messege)
                //{
                //    Console.WriteLine(b);
                //}
                //Console.WriteLine(messege);
                //server.SendMultipartBytes(new byte[5]);
                if (clientMessage.FrameCount == 3)
                {
                    var clientAddress = clientMessage[0];
                    var clientOriginalMessage = clientMessage[2].ConvertToString();
                    string response = string.Format("{0} back from server {1}",
                        clientOriginalMessage, DateTime.Now.ToLongTimeString());
                    var messageToClient = new NetMQMessage();
                    messageToClient.Append(clientAddress);
                    messageToClient.AppendEmptyFrame();
                    messageToClient.Append(response);
                    server.SendMultipartMessage(messageToClient);
                }

            }
     
        }
        void PrintFrames(string operationType, NetMQMessage message)
        {
            for (int i = 0; i < message.FrameCount; i++)
            {
                Console.WriteLine("{0} Socket : Frame[{1}] = {2}", operationType, i,
                    message[i].ConvertToString());
            }
        }
    }
}
