using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherLabServer
{
	public class Forecaster
	{
		private readonly string citylist = Path.Combine(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString(), "Resources\\citylist.txt");
		public Dictionary<string, int> Cities;
	    private readonly string key;

		public Forecaster(string key)
		{
		    this.key = key;
			Cities = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(citylist, Encoding.Default));
		}

		public string GetWeather(string city)
		{
		    var forecast = new System.Net.WebClient().DownloadString(
			    "http://api.openweathermap.org/data/2.5/" + $"weather?appid={key}&id={Cities[city]}");
			var temp = (int) JObject.Parse(forecast)["main"]["temp"] - 273;
			var humidity = (int) JObject.Parse(forecast)["main"]["humidity"];
			var wind = (int) JObject.Parse(forecast)["wind"]["speed"];
			var clouds = (int) JObject.Parse(forecast)["clouds"]["all"];
			var cloudness = "Ясно";
			if (clouds > 30) cloudness = "Облачно";
			if (clouds > 60) cloudness = "Пасмурно";
			var builder = new StringBuilder();
			builder.Append("Сейчас в " + char.ToUpper(city[0]) + city.Substring(1) + ":\n");
			builder.Append("Температура: " + (temp > 0 ? '+' : '-') + temp + "°C\n");
			builder.Append(cloudness + "\n");
			builder.Append("Скорость ветра: " + wind + " м/с\n");
			builder.Append("Влажность: " + humidity + "%\n");
			return builder.ToString();
		}
	}
}