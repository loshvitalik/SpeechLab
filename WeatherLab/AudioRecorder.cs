using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using NAudio.Wave;

namespace WeatherLab
{
	internal class AudioRecorder
	{
		private const int avgWidth = 5;
		private readonly Queue<int> peaks;
		private int peaksSum;
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
			peaks = new Queue<int>();
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
				if (sample < 0) sample = (short) -sample;
				if (sample > peak) peak = sample;
			}

			CalculatePeak(peak / 70);
		}

		private void WaveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (WaveSource == null) return;
            WaveSource.Dispose();
            WaveSource = null;
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                var w = (MainWindow) Application.Current.MainWindow;
                w.levelMeter.Value = 0;
            }));
        }

		private void CalculatePeak(int peak)
		{
			peaks.Enqueue(peak);
			peaksSum += peak;
			if (peaks.Count > avgWidth)
				peaksSum -= peaks.Dequeue();
			if (Application.Current == null) return;
			Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                var w = (MainWindow) Application.Current.MainWindow;
                if (w != null) w.levelMeter.Value = peaksSum / peaks.Count;
            }));
		}
	}
}