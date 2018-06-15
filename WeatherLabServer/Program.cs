namespace WeatherLabServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server();
            server.Run();
        }
    }
}