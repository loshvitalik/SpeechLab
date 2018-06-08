using System.Linq;
using System.Text;

namespace WeatherLabServer
{
	internal class SpeechParser
	{
		private readonly string[] commands = {"как"};
		private readonly Forecaster forecaster;
		private readonly string[] greetings = {"привет", "приветствую", "прив", "здравствуйте", "здравствуй"};

		private readonly string[] stopWords =
		{
			"в", "город", "городе", "сегодня", "завтра", "сейчас", "как", "какая", "что", "расскажи", "скажи", "покажи", "узнай",
			"а", "и", "у", "когда", "будет"
		};

		private readonly string[] weatherWords =
		{
			"погода", "погоду", "температура", "ветер", "солнце", "дождь", "снег", "ветрено", "солнечно", "дождливо", "снежно",
			"жарко", "тепло", "холодно"
		};

		public SpeechParser()
		{
			forecaster = new Forecaster("1b933923de5a5582bcf7788f67709a15");
			stopWords = stopWords.Concat(weatherWords).Concat(greetings).Concat(commands).ToArray();
		}

		public string[] ParsePhrase(string phrase)
		{
			if (phrase.Length == 0)
				return new[] {"NoText"};
			var header = "Text";
			var response = new StringBuilder();
			var words = phrase.ToLower().Split(' ');

			var greeting = greetings.FirstOrDefault(w => words.Contains(w));
			if (greeting != null)
			{
				header = "Answer";
				response.Append(char.ToUpper(greeting[0]) + greeting.Substring(1) + "! \n");
			}

			var hasCommand = commands.Any(w => words.Contains(w));
			if (hasCommand)
			{
				header = "Answer";
				for (var i = 0; i < words.Length; i++)
				{
					if (!commands.Contains(words[i])) continue;
					if (words[i] == "как" && i < words.Length - 1 && words[i + 1] == "дела")
					{
						response.Append("Дела — отлично! \n");
						i++;
					}

					if (words[i] == "как" && i < words.Length - 1 && words[i + 1] == "ты")
					{
						response.Append("У меня всё замечательно! \n");
						i++;
					}
				}
			}

			var isWeather = weatherWords.Any(w => words.Contains(w));
			if (isWeather)
			{
				header = "Answer";
				var city = "";
				var wordsToLook = words.Where(w => !stopWords.Contains(w)).ToArray();
				if (wordsToLook.Length == 0)
					city = "екатеринбурге";
				else
					for (var i = 0; i < wordsToLook.Length; i++)
						if (forecaster.Cities.ContainsKey(wordsToLook[i]))
						{
							city = wordsToLook[i];
							break;
						}
						else if (i > 0 && forecaster.Cities.ContainsKey(wordsToLook[i - 1] + " " + wordsToLook[i]))
						{
							city = wordsToLook[i - 1] + " " + wordsToLook[i];
							break;
						}

				response.Append(city == "" ? "Не удалось найти погоду для этого города :(" : forecaster.GetWeather(city));
			}

			return response.Length != 0 ? new[] {header, phrase, response.ToString()} : new[] {header, phrase};
		}
	}
}