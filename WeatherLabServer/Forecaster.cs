using System.Collections.Generic;

namespace WeatherLabServer
{
	public class Forecaster
	{
		public Dictionary<string, int> Cities;

		public Forecaster(string key)
		{
			// Openweathermap = new openweathermap(key)
			Cities = new Dictionary<string, int>();
			// Cities заполняем из файла
		}

		public string GetWeather(int cityId)
		{
			// получаем json погоды, вытаскиваем температуру и отдаем красивую строку
			return @"{""Город"":" + cityId + "}";
		}
	}
}