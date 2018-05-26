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

		public string Recognize(byte[] speech)
		{
			var messageToServer = new NetMQMessage();
			messageToServer.Append(speech);
			client.SendMultipartMessage(messageToServer);
			var response = client.ReceiveMultipartMessage();
			return response.ToString();
		}
	}
}