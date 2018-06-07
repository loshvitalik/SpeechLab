using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace WeatherLab
{
	internal class Client
	{
		private readonly DealerSocket client;

		public Client(string address)
		{
			client = new DealerSocket();
			client.Connect(address);
		}

		public Tuple<string, string> Recognize(byte[] phrase)
		{
			var request = new NetMQMessage();
			request.Append(phrase);
			client.SendMultipartMessage(request);
			var response = client.ReceiveMultipartMessage();
			return ParseResponse(response);
		}

		private Tuple<string, string> ParseResponse(NetMQMessage response)
		{
			var header = response[0].ConvertToString(Encoding.UTF8);
			if (header == "NoText")
				return new Tuple<string, string>("NoText", "Не удалось распознать Ваш голос :(");
			var phrase = response[1].ConvertToString(Encoding.UTF8);
			switch (header)
			{
				case "Text":
					return new Tuple<string, string>(phrase, "Вы сказали: " + phrase + "? Мне нечего на это ответить :)");
				case "NoWeather":
					return new Tuple<string, string>(phrase, "Не удалось найти погоду для этого города :(");
				case "Weather":
				case "Answer":
					return new Tuple<string, string>(phrase, response[2].ConvertToString(Encoding.UTF8));
				default:
					return new Tuple<string, string>(phrase, "Неизвестная операция :(");
			}
		}
	}
}