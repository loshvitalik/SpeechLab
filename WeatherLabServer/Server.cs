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

        public void Run()
        {
            while (true)
            {
                var clientMessage = server.ReceiveMultipartMessage();
                Console.WriteLine("Received a message at " + DateTime.Now.ToLongTimeString());
                PrintFrames("Server receiving", clientMessage);
				// раскомментить
	            //var response = SpeechRecognizer.Recognize(clientMessage[1].ToByteArray());
                var clientAddress = clientMessage[0];
                var messageToClient = new NetMQMessage();
                messageToClient.Append(clientAddress);
                messageToClient.Append("Text");
				// раскомментить
				//messageToClient.Append(response);
				// закомментить 1 строку
                messageToClient.Append("Meessage content");
                server.SendMultipartMessage(messageToClient);
                Console.WriteLine("Sent a message at " + DateTime.Now.ToLongTimeString());
                Console.WriteLine();
            }
        }

        private void PrintFrames(string operationType, NetMQMessage message) // Test
        {
            for (var i = 0; i < message.FrameCount; i++)
                Console.WriteLine("{0} Socket : Frame[{1}] = {2}", operationType, i,
                    message[i].ConvertToString());
        }
    }
}