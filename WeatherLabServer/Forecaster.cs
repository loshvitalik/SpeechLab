using System.Collections.Generic;

namespace WeatherLabServer
{
	public class Forecaster
	{
		public Dictionary<string, string> Cities;
	    private readonly OpenWeatherAPI.OpenWeatherAPI openWeatherApi;
        public Forecaster(string key)
        {
            openWeatherApi = new OpenWeatherAPI.OpenWeatherAPI(key);
            Cities = new Dictionary<string, string>();
			Cities.Add("екатеринбург", "Yekaterinburg,ru");
			// Cities заполняем из файла
		}

		public string GetWeather(string city)
		{
			OpenWeatherAPI.Query query = openWeatherApi.query(Cities[city]);
			city = char.ToUpper(city[0]) + city.Substring(1);
            var temp = query.Main.Temperature.CelsiusCurrent;
            // получаем json погоды, вытаскиваем температуру и отдаем красивую строку
            return "Температура в городе " + city + " сейчас составляет " + temp + " градусов";
		}
	}
}