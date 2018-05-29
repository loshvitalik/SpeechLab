using System.Windows.Input;


namespace WeatherLab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
		private readonly Client client;
	    private AudioRecorder recorder;
        public MainWindow()
        {
            InitializeComponent();
	        client = new Client("tcp://127.0.0.1:2228");
        }

	    private void StartRecording(object sender, MouseButtonEventArgs e)
	    {
		    test.Content = "Запись начата";
			recorder = new AudioRecorder();
			recorder.WaveSource.StartRecording();
	    }

	    private void StopRecording(object sender, MouseButtonEventArgs e)
	    {
			test.Content = "Запись завершена";
			recorder.WaveSource.StopRecording();
			var response = client.Recognize(recorder.Stream.ToArray());
		    phrase.Content = "\" " + response.Item1 + " \"";
		    text.Content = response.Item2;
	    }
	}
}