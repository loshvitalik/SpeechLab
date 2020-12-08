using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherLabServer
{
	public class Forecaster
	{
		private readonly string citylist = Path.Combine(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString(), "Resources\\citylist.txt");

		private readonly string key;
		public Dictionary<string, int> Cities;

		public Forecaster(string key)
		{
			this.key = key;
			if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Resources")))
				Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Resources"));
			if (!File.Exists(citylist))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Warning: City list file is empty. Creating new file with example city: Ekaterinburg");
                Console.ForegroundColor = ConsoleColor.White;
				File.Create(citylist).Close();
				File.AppendAllText(citylist, @"{""екатеринбург"":1486209,""екатеринбурге"":1486209,""екб"":1486209}", Encoding.Default);
			}

			Cities = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(citylist, Encoding.Default));
		}

		public string GetWeather(string city)
        {
            var forecast = "";
            Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Acquiring weather data...");
            try
            {
                forecast = new WebClient().DownloadString("http://api.openweathermap.org/data/2.5/" +
                                                          $"weather?appid={key}&id={Cities[city]}");
            }
            catch (WebException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("SpeechLab could not get weather information.");
                Console.WriteLine("The API key for OpenWeatherMap access is incorrect.\n\n" +
                                  "Full exception message:");
                Console.WriteLine(e.Message);
                Console.WriteLine("\nPress any key to stop the server...");
                Console.ReadKey();
                Environment.Exit(3);
			}
            Console.WriteLine("Weather data acquired");
			Console.ForegroundColor = ConsoleColor.White;
			var temp = (int) JObject.Parse(forecast)["main"]["temp"] - 273;
			var humidity = (int) JObject.Parse(forecast)["main"]["humidity"];
			var wind = (int) JObject.Parse(forecast)["wind"]["speed"];
			var clouds = (int) JObject.Parse(forecast)["clouds"]["all"];
			var cloudness = "Ясно";
			if (clouds > 30) cloudness = "Облачно";
			if (clouds > 60) cloudness = "Пасмурно";
			var builder = new StringBuilder();
			builder.Append("Сейчас в " + char.ToUpper(city[0]) + city.Substring(1) + ":\n");
			builder.Append("Температура: " + (temp > 0 ? "+" : "") + temp + "°C\n");
			builder.Append(cloudness + "\n");
			builder.Append("Скорость ветра: " + wind + " м/с\n");
			builder.Append("Влажность: " + humidity + "%\n");
			return builder.ToString();
		}
	}
}