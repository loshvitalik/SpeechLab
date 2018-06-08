using System.Windows;
using System.Windows.Input;

namespace WeatherLab
{
	/// <summary>
	/// Логика взаимодействия для Alert.xaml
	/// </summary>
	public partial class Alert
	{
		public Alert(string title, string content)
		{
			InitializeComponent();
			Title = title;
			text.Text = content;
		}

		private void CloseWindow(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
	}
}