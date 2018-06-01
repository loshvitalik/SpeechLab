using System;
using System.IO;
using System.Linq;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace WeatherLabServer
{
	internal class Server
	{
		private readonly Forecaster forecaster;
		private readonly SpeechRecognizer recognizer;
		private readonly RouterSocket server;
		private readonly string stoplist = Path.Combine(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString(), "Resources\\stoplist.txt");
		private readonly string[] stopWords;

		public Server()
		{
			if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Resources")))
				Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Resources"));
			if (!File.Exists(stoplist))
				File.Create(stoplist).Close();
			server = new RouterSocket("@tcp://127.0.0.1:2228");
			recognizer = new SpeechRecognizer();
			forecaster = new Forecaster("1b933923de5a5582bcf7788f67709a15");
			stopWords = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(stoplist, Encoding.Default));
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
				message.Append(Encoding.UTF8.GetBytes(d));
			return message;
		}

		private void PrintFrames(string operationType, NetMQMessage message) // Test
		{
			for (var i = 0; i < message.FrameCount; i++)
				Console.WriteLine("{0} Socket : Frame[{1}]", operationType, i);
		}
	}
}