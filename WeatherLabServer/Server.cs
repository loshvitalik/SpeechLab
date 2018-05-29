using System;
using System.Linq;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace WeatherLabServer
{
	internal class Server
	{
		private readonly Forecaster forecaster;
		private readonly SpeechRecognizer recognizer;
		private readonly RouterSocket server;
		private readonly string[] stopWords = {"погода", "в"}; // можно и из файла при создании

		public Server()
		{
			server = new RouterSocket("@tcp://127.0.0.1:2228");
			recognizer = new SpeechRecognizer();
			forecaster = new Forecaster("APIkey");
		}

		public void Run()
		{
			while (true)
			{
				var request = server.ReceiveMultipartMessage();

				//
				Console.WriteLine("Received a message at " + DateTime.Now);
				PrintFrames("Server receiving", request);
				//

				var response = new NetMQMessage();
				var clientAddress = request[0];
				response.Append(clientAddress);

				var phrase = recognizer.Recognize(request[1].ToByteArray());
				response = ParsePhrase(response, phrase);
				server.SendMultipartMessage(response);

				//
				Console.WriteLine("Sent a message at " + DateTime.Now);
				Console.WriteLine();
				//
			}
		}

		private NetMQMessage ParsePhrase(NetMQMessage response, string phrase)
		{
			var words = phrase.ToLower().Split(' ');
			if (phrase.Length == 0)
				return CreateResponse(response, "NoText");
			if (!words.Contains("погода"))
				return CreateResponse(response, "Text", phrase);
			var city = "";
			foreach (var w in words.Where(w => !stopWords.Contains(w)))
				if (forecaster.Cities.ContainsKey(w))
				{
					city = w;
					break;
				}
			if (city == "")
				return CreateResponse(response, "NoWeather", phrase);
			return CreateResponse(response, "Weather", phrase,
				forecaster.GetWeather(city));
		}

		private NetMQMessage CreateResponse(NetMQMessage message, params string[] data)
		{
			foreach (var d in data)
				message.Append(d);
			return message;
		}

		private void PrintFrames(string operationType, NetMQMessage message) // Test
		{
			for (var i = 0; i < message.FrameCount; i++)
				Console.WriteLine("{0} Socket : Frame[{1}]", operationType, i);
		}
	}
}