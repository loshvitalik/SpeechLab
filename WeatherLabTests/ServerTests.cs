using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherLab;
using WeatherLabServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetMQ;

namespace WeatherLabTests
{
    [TestClass]
    public class ServerTests
    {
        private readonly SpeechParser parser = new SpeechParser();
        // тестовый шаблон сообщения
        private readonly string address = "tcp://127.0.0.1:2228";
        private readonly NetMQMessage message = new NetMQMessage();

        [TestInitialize]
        public void Init()
        {
            message.Append(address);
        }

        // NetMQMessage CreateResponse(NetMQMessage, params string[])

        // тест сбора ответного сообщения с заголовком и содержимым
        [TestMethod]
        public void Server_ResponseCreation()
        {
            var content = new[] {"Header test", "Content test"};
            var result = Server.CreateResponse(message, content);
            Assert.AreEqual(3, result.FrameCount);
            Assert.AreEqual(address, result[0].ConvertToString());
            Assert.AreEqual("Header test", result[1].ConvertToString());
            Assert.AreEqual("Content test", result[2].ConvertToString());
        }

        // string[] ParsePhrase(string)

        // тест разбора запроса без найденного текста
        [TestMethod]
        public void Parser_NoText()
        {
            var text = "";
            var result = parser.ParsePhrase(text);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("NoText", result[0]);
        }

        // тест разбора запроса с простым текстом
        [TestMethod]
        public void Parser_PlainText()
        {
            var text = "random test phrase";
            var result = parser.ParsePhrase(text);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Text", result[0]);
            Assert.AreEqual(text, result[1]);
        }

        // тест разбора запроса с приветствием
        [TestMethod]
        public void Parser_PlainTextWithGreeting()
        {
            var text = "привет другая часть фразы"; // localizable
            var result = parser.ParsePhrase(text);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("Answer", result[0]);
            Assert.AreEqual(text, result[1]);
            Assert.AreEqual("Привет! \n", result[2]); // localizable
        }

        // тест разбора запроса с командой
        [TestMethod]
        public void Parser_Question()
        {
            var text = "слушай как дела у тебя"; // localizable
            var result = parser.ParsePhrase(text);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("Answer", result[0]);
            Assert.AreEqual(text, result[1]);
            Assert.AreEqual("Дела — отлично! \n", result[2]); // localizable
        }

        // проверка наличии всех данных о погоде в ответ на запрос погоды
        [TestMethod]
        public void Parser_WeatherData()
        {
            var text = "погода"; // localizable
            var result = parser.ParsePhrase(text);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("Answer", result[0]);
            Assert.AreEqual(text, result[1]);
            Assert.IsTrue(result[2].Contains("Температура") && result[2].Contains("Скорость ветра") && result[2].Contains("Влажность")); // localizable
        }

        // проверка выдачи информации о погоде в городе по умолчанию (Екатеринбург) в ответ на запрос погоды без указания города
        [TestMethod]
        public void Parser_WeatherDefaultCity()
        {
            var text = "погода"; // localizable
            var result = parser.ParsePhrase(text);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("Answer", result[0]);
            Assert.AreEqual(text, result[1]);
            Assert.IsTrue(result[2].Contains("Екатеринбурге")); // localizable
        }

        // проверка выдачи информации о погоде в заданном городе в ответ на запрос погоды с указанием города
        [TestMethod]
        public void Parser_WeatherGivenCity()
        {
            var text = "погода в москве"; // localizable
            var result = parser.ParsePhrase(text);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("Answer", result[0]);
            Assert.AreEqual(text, result[1]);
            Assert.IsTrue(result[2].Contains("Москве")); // localizable
        }

        // проверка выдачи ошибки в ответ на запрос погоды с указанием города, отсутствующего в словаре
        [TestMethod]
        public void Parser_WeatherunknownCity()
        {
            var text = "погода в неизвестном городе"; // localizable
            var result = parser.ParsePhrase(text);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("Answer", result[0]);
            Assert.AreEqual(text, result[1]);
            Assert.AreEqual("Не удалось найти погоду для этого города :(", result[2]); // localizable
        }
    }
}