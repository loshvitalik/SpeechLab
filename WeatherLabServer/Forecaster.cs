using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace WeatherLabServer
{
	public class Forecaster
	{
		private readonly OpenWeatherAPI.OpenWeatherAPI openWeatherApi;
		private readonly string citylist = Path.Combine(Environment.CurrentDirectory, "data\\citylist.txt");
		public Dictionary<string, string> Cities;

		public Forecaster(string key)
		{
			openWeatherApi = new OpenWeatherAPI.OpenWeatherAPI(key);
			Cities = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(citylist, Encoding.Default));
		}

		public string GetWeather(string city)
		{
			var query = openWeatherApi.query(Cities[city]);
			city = char.ToUpper(city[0]) + city.Substring(1);
			var temp = query.Main.Temperature.CelsiusCurrent;
			return "Температура в городе " + city + " сейчас составляет " + temp + " градусов";
		}
	}
}