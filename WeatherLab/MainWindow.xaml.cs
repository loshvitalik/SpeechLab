using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WeatherLab
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly Client client;
		private AudioRecorder recorder;
		private bool wasResponded;

		public MainWindow()
		{
			InitializeComponent();
			client = new Client("tcp://127.0.0.1:2228");
		}

		private void StartRecording(object sender, MouseButtonEventArgs e)
		{
			recorder = new AudioRecorder();
			recorder.WaveSource.StartRecording();
			spoiler.Visibility = Visibility.Hidden;
			mic.Source = wasResponded
				? new BitmapImage(new Uri("Resources/mic_rec1.png", UriKind.Relative))
				: new BitmapImage(new Uri("Resources/mic_rec0.png", UriKind.Relative));
		}

		private void StopRecording(object sender, MouseButtonEventArgs e)
		{
			recorder.WaveSource.StopRecording();
			mic.Source = new BitmapImage(new Uri("Resources/mic.png", UriKind.Relative));
			PrintResult();
		}

		private void PrintResult()
		{
			if (recorder.Stream == null || recorder.Stream.Length < 50000)
			{
				wasResponded = false;
				spoiler.Visibility = Visibility.Visible;
				spoiler.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
				mic.Source = new BitmapImage(new Uri("Resources/mic.png", UriKind.Relative));
			}
			else
			{
				var response = client.Recognize(recorder.Stream.ToArray());
                if (response.Item1 != "NoText") wasResponded = true;
				phrase.Text = response.Item1 == "NoText" ? "" : "\"" + response.Item1 + "\"";
				text.Text = response.Item2;
				mic.Source = wasResponded
                    ? new BitmapImage(new Uri("Resources/mic_done.png", UriKind.Relative))
                    : new BitmapImage(new Uri("Resources/mic.png", UriKind.Relative));
			}
		}

		private void ShowAboutWindow(object sender, MouseButtonEventArgs e)
		{
			new Alert("О программе \"Speech Lab\"",
				"Speech Lab v. 3.0\n© 2020 loshvitalik, UrFU").Show();
		}
	}
}