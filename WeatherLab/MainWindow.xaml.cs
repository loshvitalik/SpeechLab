using System.Windows.Input;


namespace WeatherLab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

	    private void StartRecording(object sender, MouseButtonEventArgs e)
	    {
		    test.Content = "Запись начата";
	    }

	    private void StopRecording(object sender, MouseButtonEventArgs e)
	    {
			test.Content = "Запись завершена";
		}
    }
}