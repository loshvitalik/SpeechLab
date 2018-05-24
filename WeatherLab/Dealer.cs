using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherLab
{
    static class Dealer
    {
        public static void Recognise(byte[] voice)
        {
            var client = new DealerSocket();
            client.Connect("tcp://10.97.129.111:2228");

        }
    }
}
