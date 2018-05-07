using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;

namespace WeatherLabServer
{
    class SpeechRecognition
    {
        public static String Recognise(byte[] Speech)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 44100,
                LanguageCode = "ru-RU"
            }, RecognitionAudio.FromBytes(Speech));
            string result = (response.Results.Count == 0)
                ? "Error"
                : response.Results[0].Alternatives[0].Transcript;
            return result;
        }
    }
}
