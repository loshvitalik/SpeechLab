using System;
using System.IO;
using System.Threading;
using Google.Cloud.Speech.V1;
using NAudio.Wave;

namespace WeatherLabServer
{
	class Program
	{
		// просто пример записи звука и распознавания. Это должно быть раздельно клиент и сервер
		// на сервере должны остаться только последние строчки Main, с var speech до string result
		// NAudio нужно будет удалить с сервера!


		static void Main(string[] args)
		{
		    var server = new Server();
            server.Start();

		}	
	}
}