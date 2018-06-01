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
		public Dictionary<string, string> Cities;
	    private readonly string key;

		public Forecaster(string key)
		{
		    this.key = key;
			Cities = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(citylist, Encoding.Default));
		}

		public string GetWeather(string city)
		{
		    var forecast = new System.Net.WebClient().DownloadString(
			    "http://api.openweathermap.org/data/2.5/" + $"weather?appid={key}&q={Cities[city]}");
		    var temp = (int)JObject.Parse(forecast)["main"]["temp"]-273;
		    city = char.ToUpper(city[0]) + city.Substring(1);
			char sign = temp > 0 ? '+' : '-';
			return "Температура в г. " + city + " сейчас составляет " + sign + temp + "°C";
		}
	}
}