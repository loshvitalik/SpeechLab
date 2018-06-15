using Google.Cloud.Speech.V1;

namespace WeatherLabServer
{
	internal class SpeechRecognizer
	{
		private readonly SpeechClient client;
		private readonly RecognitionConfig config;

		public SpeechRecognizer()
		{
			client = SpeechClient.Create();
			config = new RecognitionConfig()
			{
				Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
				SampleRateHertz = 44100,
				LanguageCode = "ru-Ru"
			};
		}

		public string Recognize(byte[] speech)
		{
			var response = client.Recognize(config, RecognitionAudio.FromBytes(speech));
			return response.Results.Count != 0 ? response.Results[0].Alternatives[0].Transcript : "";
		}
	}
}