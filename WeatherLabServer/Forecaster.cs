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
		private readonly string citylist = Path.Combine(Environment.CurrentDirectory, "data\\citylist.txt");
		public Dictionary<string, string> Cities;
	    private String key;

		public Forecaster(string key)
		{
		    this.key = key;;
			Cities = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(citylist, Encoding.Default));
		}

		public string GetWeather(string city)
		{
			
		    var forecast = new System.Net.WebClient().DownloadString(String.Format("http://api.openweathermap.org/data/2.5/"+ "weather?appid={0}&q={1}",key,Cities[city]));
		    var temp = (double)JObject.Parse(forecast)["main"]["temp"]-273;
		    city = char.ToUpper(city[0]) + city.Substring(1);
			return "Температура в городе " + city + " сейчас составляет " + temp + " градусов";
		}
	}
}