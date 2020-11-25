using System;
using Google.Cloud.Speech.V1;

namespace WeatherLabServer
{
	internal class SpeechRecognizer
	{
		private readonly SpeechClient client;
		private readonly RecognitionConfig config;

		public SpeechRecognizer()
		{
            try
            {
                client = SpeechClient.Create();
			}
            catch (Exception e)
            {
				Console.WriteLine("SpeechLab server could not start speech recognizer.");
				Console.WriteLine("Perhaps the GOOGLE_APPLICATION_CREDENTIALS environment variable or the corresponding .json file is missing.\n" +
                                  "Correct GOOGLE_APPLICATION_CREDENTIALS environment variable is required to run WeatherLab server.\n\n" +
                                  "Full exception message:");
                Console.WriteLine(e.Message);
				Console.WriteLine("\nPress any key to stop the server...");
                Console.ReadKey();
                Environment.Exit(1);
            }
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