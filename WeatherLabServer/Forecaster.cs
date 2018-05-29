using System.Collections.Generic;
using OpenWeatherAPI;

namespace WeatherLabServer
{
	public class Forecaster
	{
		public Dictionary<string, int> Cities;
        // private OpenWeatherAPI.OpenWeatherAPI openWeatherApi;
	    private OpenWeatherAPI.OpenWeatherAPI openWeatherAPI;
        public Forecaster(string key)
        {
            openWeatherAPI = new OpenWeatherAPI.OpenWeatherAPI(key);
            //openWeatherApi = new OpenWeatherAPI.OpenWeatherAPI(key);
            // Openweathermap = new openweathermap(key)
            Cities = new Dictionary<string, int>();
			// Cities заполняем из файла
		}

		public string GetWeather(string city)
		{
		    
		    OpenWeatherAPI.Query query = openWeatherAPI.query(city);
            double temperature =query.Main.Temperature.CelsiusCurrent;
            // получаем json погоды, вытаскиваем температуру и отдаем красивую строку
            return @"{""ID города"":" + Cities[city] + "}";
		}
	}
}