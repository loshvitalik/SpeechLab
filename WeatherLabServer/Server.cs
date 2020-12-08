using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace WeatherLabServer
{
    public class Server
    {
        private readonly SpeechParser parser;
        private readonly SpeechRecognizer recognizer;
        private readonly RouterSocket server;
        private readonly string address = "@tcp://127.0.0.1:2228";

        public Server()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SpeechLab server is starting up...");
            try
            {

                server = new RouterSocket(address);
            }
            catch (NetMQException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("SpeechLab server could not start.");
                Console.WriteLine("Another instance of the server is already running or the IP address is in use by other application.\n" +
                                  "Check if IP address is correct. Current: " + address + "\n\n" +
                                  "Full exception message:");
                Console.WriteLine(e.Message);
                Console.WriteLine("\nPress any key to stop the server...");
                Console.ReadKey();
                Environment.Exit(2);
            }
            recognizer = new SpeechRecognizer();
            parser = new SpeechParser();
            Console.WriteLine("SpeechLab server started.");

            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Run()
        {
            while (true)
            {
                var request = server.ReceiveMultipartMessage();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Received a message from client at " + DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                var response = new NetMQMessage();
                var clientAddress = request[0];
                response.Append(clientAddress);

                var phrase = recognizer.Recognize(request[1].ToByteArray());
                var result = parser.ParsePhrase(phrase);
                server.SendMultipartMessage(CreateResponse(response, result));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Sent a message with header: " + response[1].ConvertToString(Encoding.UTF8) + " at " +
                                  DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static NetMQMessage CreateResponse(NetMQMessage message, params string[] data)
        {
            foreach (var d in data)
                message.Append(Encoding.UTF8.GetBytes(d));
            return message;
        }
    }
}