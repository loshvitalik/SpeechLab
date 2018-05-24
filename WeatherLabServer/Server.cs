using System;
using NetMQ;
using NetMQ.Sockets;

namespace WeatherLabServer
{
    internal class Server
    {
        private readonly RouterSocket server;

        public Server()
        {
            server = new RouterSocket("@tcp://10.97.129.111:2228");
        }

        public void Start()
        {
            while (true)
            {
                var clientMessage = server.ReceiveMultipartMessage();
                PrintFrames("Server reciving", clientMessage);
                var clientAddress = clientMessage[0];
                var messageToClient = new NetMQMessage();
                messageToClient.Append(clientAddress);
                messageToClient.Append("Head: text");
                messageToClient.Append("Some text");
                server.SendMultipartMessage(messageToClient);
            }
        }

        private void PrintFrames(string operationType, NetMQMessage message)
        {
            for (var i = 0; i < message.FrameCount; i++)
                Console.WriteLine("{0} Socket : Frame[{1}] = {2}", operationType, i,
                    message[i].ConvertToString());
        }
    }
}