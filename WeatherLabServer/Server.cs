using System;
using System.IO;
using System.Linq;
using System.Security;
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
		private readonly string weatherlist = Path.Combine(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString(), "Resources\\weatherlist.txt");
		private readonly string[] weatherWords;

		public Server()
		{
			if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Resources")))
				Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Resources"));
			if (!File.Exists(stoplist))
				File.Create(stoplist).Close();
			if (!File.Exists(weatherlist))
				File.Create(weatherlist).Close();
			server = new RouterSocket("@tcp://127.0.0.1:2228");
			recognizer = new SpeechRecognizer();
			forecaster = new Forecaster("1b933923de5a5582bcf7788f67709a15");
			stopWords = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(stoplist, Encoding.Default));
			weatherWords = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(weatherlist, Encoding.Default));
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
				response = ParsePhrase(response, phrase);
				server.SendMultipartMessage(response);
				Console.WriteLine("Sent a message with header: " + response[1].ConvertToString(Encoding.UTF8) + " at " +
				                  DateTime.Now);
			}
		}

		private NetMQMessage ParsePhrase(NetMQMessage response, string phrase)
		{
			var words = phrase.ToLower().Split(' ');
			if (phrase.Length == 0)
				return CreateResponse(response, "NoText");
			var isWeather = weatherWords.Any(w => words.Contains(w));
			if (!isWeather)
				return CreateResponse(response, "Text", phrase);
			var city = "";
			words = words.Where(w => !stopWords.Contains(w)).Where(w => !weatherWords.Contains(w)).ToArray();
			foreach (var w in words)
				if (forecaster.Cities.ContainsKey(w))
				{
					city = w;
					break;
				}

			if (city == "")
				for (var i = 0; i < words.Length - 1; i++)
					if (forecaster.Cities.ContainsKey(words[i] + " " + words[i + 1]))
					{
						city = words[i] + " " + words[i + 1];
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
	}
}