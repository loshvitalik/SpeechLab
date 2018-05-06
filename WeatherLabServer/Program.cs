using System;
using System.IO;
using System.Threading;
using Google.Cloud.Speech.V1;
using NAudio.Wave;

namespace WeatherLabServer
{
	class Program
	{
		// просто пример записи звука и распознавания. Это должно быть раздельно клиент и сервер
		// на сервере должны остаться только последние строчки Main, с var speech до string result
		// NAudio нужно будет удалить с сервера!

		static WaveInEvent waveSource;
		static bool IsOver;
		static byte[] Speech;
		static readonly MemoryStream stream = new MemoryStream();

		static void Main(string[] args)
		{
			// record sound

			waveSource = new WaveInEvent
			{
				WaveFormat = new WaveFormat(44100, 1)
			};

			waveSource.DataAvailable += waveSource_DataAvailable;
			waveSource.RecordingStopped += waveSource_RecordingStopped;

			waveSource.StartRecording();

			var thread = new Thread(Delay);
			thread.Start();
			while (!IsOver)
				Thread.Sleep(10);
			waveSource.StopRecording();
			Speech = stream.ToArray();

			// speech recognition

			var speech = SpeechClient.Create();
			var response = speech.Recognize(new RecognitionConfig
			{
				Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
				SampleRateHertz = 44100,
				LanguageCode = "ru-RU"
			}, RecognitionAudio.FromBytes(Speech));
			string result = (response.Results.Count == 0)
				? "Error"
				: response.Results[0].Alternatives[0].Transcript;
			Console.WriteLine(result);
		}

		static void Delay()
		{
			Thread.Sleep(5000);
			IsOver = true;
		}

		static void waveSource_DataAvailable(object sender, WaveInEventArgs e)
		{
			stream.Write(e.Buffer, 0, e.BytesRecorded);
			stream.Flush();
		}

		static void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
		{
			if (waveSource != null)
			{
				waveSource.Dispose();
				waveSource = null;
			}
		}
	}
}