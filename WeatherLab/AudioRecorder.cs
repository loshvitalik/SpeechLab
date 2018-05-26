using System.IO;
using NAudio.Wave;

namespace WeatherLab
{
	internal class AudioRecorder
	{
		public MemoryStream Stream;
		public WaveInEvent WaveSource;

		public AudioRecorder()
		{
			WaveSource = new WaveInEvent
			{
				WaveFormat = new WaveFormat(44100, 1)
			};

			WaveSource.DataAvailable += WaveSource_DataAvailable;
			WaveSource.RecordingStopped += WaveSource_RecordingStopped;
			Stream = new MemoryStream();
		}

		private void WaveSource_DataAvailable(object sender, WaveInEventArgs e)
		{
			Stream.Write(e.Buffer, 0, e.BytesRecorded);
			Stream.Flush();
		}

		private void WaveSource_RecordingStopped(object sender, StoppedEventArgs e)
		{
			if (WaveSource != null)
			{
				WaveSource.Dispose();
				WaveSource = null;
			}
		}
	}
}