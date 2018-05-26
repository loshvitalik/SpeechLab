using Google.Cloud.Speech.V1;

namespace WeatherLabServer
{
	internal class SpeechRecognizer
	{
		public static string Recognize(byte[] speech)
		{
			var client = SpeechClient.Create();
			var config = new RecognitionConfig
			{
				Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
				SampleRateHertz = 44100,
				LanguageCode = "ru-Ru"
			};
			var response = client.Recognize(config, RecognitionAudio.FromBytes(speech));
			if (response.Results.Count != 0)
				return "ru:" + response.Results[0].Alternatives[0].Transcript;
			config.LanguageCode = "en";
			response = client.Recognize(config, RecognitionAudio.FromBytes(speech));
			if (response.Results.Count != 0)
				return "en:" + response.Results[0].Alternatives[0].Transcript;
			return "Error";
		}
	}
}