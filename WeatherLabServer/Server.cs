using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace WeatherLabServer
{
    class Server
    {
        private RouterSocket server;

        Server()
        {
            server = new RouterSocket("@tcp://127.0.0.1:5556");
        }

        public void Start()
        {
            while (true)
            {
                var clientMessehe = server.ReceiveMultipartMessage();
            }
        }
    }
}
