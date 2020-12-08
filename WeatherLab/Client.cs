using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace WeatherLab
{
    public class Client
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
            var response = new NetMQMessage();
            var msgReceived = client.TryReceiveMultipartMessage(TimeSpan.FromSeconds(5), ref response);
            return msgReceived ? ParseResponse(response) : new Tuple<string, string>("NoText", "Не удалось подключиться к серверу :(");
        }

        public Tuple<string, string> ParseResponse(NetMQMessage response)
        {
            var header = response[0].ConvertToString(Encoding.UTF8);
            if (header == "NoText")
                return new Tuple<string, string>("NoText", "Не удалось распознать Ваш голос :(");
            var phrase = response[1].ConvertToString(Encoding.UTF8);
            switch (header)
            {
                case "Text":
                    return new Tuple<string, string>(phrase,
                        "Вы сказали: " + phrase + "? Мне нечего на это ответить :)");
                case "Answer":
                    return new Tuple<string, string>(phrase, response[2].ConvertToString(Encoding.UTF8));
                default:
                    return new Tuple<string, string>(phrase, "Неизвестная операция :(");
            }
        }
    }
}