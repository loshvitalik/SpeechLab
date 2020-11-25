using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace WeatherLabServer
{
	internal class Server
	{
		private readonly SpeechParser parser;
		private readonly SpeechRecognizer recognizer;
		private readonly RouterSocket server;

		public Server()
		{
            Console.WriteLine("SpeechLab server is starting up...");
			server = new RouterSocket("@tcp://127.0.0.1:2228");
			recognizer = new SpeechRecognizer();
			parser = new SpeechParser();
            Console.WriteLine("SpeechLab server started.");
		}

		public void Run()
		{
			while (true)
			{
				var request = server.ReceiveMultipartMessage();
				Console.WriteLine("Received a message from client at " + DateTime.Now);

				var response = new NetMQMessage();
				var clientAddress = request[0];
				response.Append(clientAddress);

				var phrase = recognizer.Recognize(request[1].ToByteArray());
				var result = parser.ParsePhrase(phrase);
				server.SendMultipartMessage(CreateResponse(response, result));
				Console.WriteLine("Sent a message with header: " + response[1].ConvertToString(Encoding.UTF8) + " at " +
				                  DateTime.Now);
			}
		}

		private NetMQMessage CreateResponse(NetMQMessage message, params string[] data)
		{
			foreach (var d in data)
				message.Append(Encoding.UTF8.GetBytes(d));
			return message;
		}
	}
}