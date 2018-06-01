using System;
using System.IO;
using System.Windows;
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

			short peak = 0;
			var buffer = new WaveBuffer(e.Buffer);
			for (var i = 0; i < e.BytesRecorded / 4; i++)
			{
				var sample = buffer.ShortBuffer[i];
				if (sample < 0) sample = (short)-sample;
				if (sample > peak) peak = sample;
			}

			Application.Current.Dispatcher.BeginInvoke(new Action(delegate
			{
				var w = (MainWindow) Application.Current.MainWindow;
				w.levelMeter1.Value = peak / 70;
				w.levelMeter2.Value = peak / 70;
			}));
		}

		private void WaveSource_RecordingStopped(object sender, StoppedEventArgs e)
		{
			if (WaveSource != null)
			{
				WaveSource.Dispose();
				WaveSource = null;
				Application.Current.Dispatcher.BeginInvoke(new Action(delegate
				{
					var w = (MainWindow)Application.Current.MainWindow;
					w.levelMeter1.Value = 0;
					w.levelMeter2.Value = 0;
				}));
			}
		}
	}
}