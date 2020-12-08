using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using NetMQ;
using WeatherLab;

namespace WeatherLabTests
{
    [TestClass]
    public class ClientTests
    {
        private readonly Client defaultClient = new Client("tcp://127.0.0.1:2228"); // подключение по умолчанию

        private readonly byte[] defaultAudio = File.ReadAllBytes(Path.Combine(
            Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                .ToString()).ToString(), "Resources\\test.wav")); // тестовый файл с речью

        // Tuple<string, string> Recognize(byte[])

        // тест ответа при отсутствии ответа от сервера
        [TestMethod]
        public void Recognize_NoAnswer()
        {
            var client = new Client("tcp://0.0.0.0:2228");
            var response = client.Recognize(defaultAudio);
            Assert.AreEqual("NoText", response.Item1);
            Assert.AreEqual("Не удалось подключиться к серверу :(", response.Item2); // localizable
        }

        // тест ответа при наличии ответа от сервера
        [TestMethod]
        public void Recognize_HasResponse()
        {
            var response = defaultClient.Recognize(defaultAudio);
            Assert.AreNotEqual("NoText", response.Item1);
        }

        // Tuple<string, string> ParseResponse(NetMQMessage)

        // тест разбора ответа без распознанного текста
        [TestMethod]
        public void Parse_NoText()
        {
            var header = "NoText";
            var message = new NetMQMessage();
            message.Append(Encoding.UTF8.GetBytes(header));
            var result = defaultClient.ParseResponse(message);
            Assert.AreEqual(header, result.Item1);
            Assert.AreEqual("Не удалось распознать Ваш голос :(", result.Item2); // localizable
        }

        // тест разбора ответа с простым распознанным текстом
        [TestMethod]
        public void Parse_Text()
        {
            var phrase = "Random test text for test";
            var message = new NetMQMessage();
            message.Append(Encoding.UTF8.GetBytes("Text"));
            message.Append(Encoding.UTF8.GetBytes(phrase));
            var result = defaultClient.ParseResponse(message);
            Assert.AreEqual(phrase, result.Item1);
        }

        // тест разбора ответа с предопределенным ответом на вопрос
        [TestMethod]
        public void Parse_Answer()
        {
            var phrase = "Random test question for test";
            var answer = "Other random test answer for new test";
            var message = new NetMQMessage();
            message.Append(Encoding.UTF8.GetBytes("Answer"));
            message.Append(Encoding.UTF8.GetBytes(phrase));
            message.Append(answer);
            var result = defaultClient.ParseResponse(message);
            Assert.AreEqual(phrase, result.Item1);
            Assert.AreEqual(answer, result.Item2);
        }

        // тест разбора ответа с неизвестной операцией
        [TestMethod]
        public void Parse_Error()
        {
            var header = "EMERGENCY";
            var phrase = "DO NOT READ THIS";
            var message = new NetMQMessage();
            message.Append(Encoding.UTF8.GetBytes(header));
            message.Append(Encoding.UTF8.GetBytes(phrase));
            var result = defaultClient.ParseResponse(message);
            Assert.AreEqual(phrase, result.Item1);
            Assert.AreEqual("Неизвестная операция :(", result.Item2); // localizable
        }
    }
}