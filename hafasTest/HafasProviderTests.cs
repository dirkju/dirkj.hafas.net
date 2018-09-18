using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using hafas.net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace hafasTest
{
    [TestClass]
    public class HafasProviderTests
    {
        private const string responseContentXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<DepartureBoard xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""http://xmlopen.rejseplanen.dk/xml/rest/hafasRestDepartureBoard.xsd"">
    <Departure name=""Bus 332"" type=""BUS"" stop=""Solhøjpark (Solhøjgårdsvej)"" time=""21:42"" date=""26.05.18"" messages=""0"" finalStop=""Farum St."" direction=""Farum øst - Farum St. via Farum Midtpunkt"">
        <JourneyDetailRef ref=""http://xmlopen.rejseplanen.dk/bin/rest.exe/journeyDetail?ref=385635%2F146749%2F484208%2F113561%2F86%3Fdate%3D26.05.18"" />
    </Departure>
    <Departure name=""Bus 332"" type=""BUS"" stop=""Solhøjpark (Solhøjgårdsvej)"" time=""22:42"" date=""26.05.18"" messages=""0"" finalStop=""Farum St."" direction=""Farum øst - Farum St. via Farum Midtpunkt"">
        <JourneyDetailRef ref=""http://xmlopen.rejseplanen.dk/bin/rest.exe/journeyDetail?ref=743568%2F266060%2F587158%2F45726%2F86%3Fdate%3D26.05.18"" />
    </Departure>
</DepartureBoard>";
        private const string responseContentXml2 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<DepartureBoard xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""http://xmlopen.rejseplanen.dk/xml/rest/hafasRestDepartureBoard.xsd"">
    <Departure name=""Bus 332"" type=""BUS"" stop=""Solhøjpark (Solhøjgårdsvej)"" time=""21:42"" date=""26.05.18"" messages=""0"" finalStop=""Farum St."" direction=""Farum øst - Farum St. via Farum Midtpunkt"">
        <JourneyDetailRef ref=""http://xmlopen.rejseplanen.dk/bin/rest.exe/journeyDetail?ref=385635%2F146749%2F484208%2F113561%2F86%3Fdate%3D26.05.18"" />
    </Departure>
    <Departure name=""Bus 332"" type=""BUS"" stop=""Solhøjpark (Solhøjgårdsvej)"" time=""22:42"" date=""26.05.18"" messages=""0"" finalStop=""Farum St."" direction=""Farum øst - Farum St. via Farum Midtpunkt"">
        <JourneyDetailRef ref=""http://xmlopen.rejseplanen.dk/bin/rest.exe/journeyDetail?ref=743568%2F266060%2F587158%2F45726%2F86%3Fdate%3D26.05.18"" />
    </Departure>
</DepartureBoard>";
        private HafasProvider provider;

        [TestInitialize]
        public void Initialize()
        {
            var responseContentJson = 
@"{
    ""DepartureBoard"": {
        ""noNamespaceSchemaLocation"": ""http://xmlopen.rejseplanen.dk/xml/rest/hafasRestDepartureBoard.xsd"",
        ""Departure"": [
            {
                ""name"": ""Bus 334"",
                ""type"": ""BUS"",
                ""stop"": ""Stavnsholtstien (Paltholmvej)"",
                ""time"": ""00:14"",
                ""date"": ""20.05.18"",
                ""messages"": ""1"",
                ""finalStop"": ""Farum St."",
                ""direction"": ""Farum St."",
                ""JourneyDetailRef"": {
                    ""ref"": ""http://xmlopen.rejseplanen.dk/bin/rest.exe/journeyDetail?ref=640437%2F225164%2F944850%2F258946%2F86%3Fdate%3D20.05.18%26format%3Djson""
                }
            },
            {
                ""name"": ""Bus 334"",
                ""type"": ""BUS"",
                ""stop"": ""Stavnsholtstien (Paltholmvej)"",
                ""time"": ""07:47"",
                ""date"": ""20.05.18"",
                ""messages"": ""1"",
                ""finalStop"": ""Holte St."",
                ""direction"": ""Holte St."",
                ""JourneyDetailRef"": {
                    ""ref"": ""http://xmlopen.rejseplanen.dk/bin/rest.exe/journeyDetail?ref=638166%2F224440%2F674052%2F124304%2F86%3Fdate%3D20.05.18%26format%3Djson""
                }
            }
        ]
    }
}
";

            var msgHandlerMock = new Mock<HttpMessageHandler>();
            msgHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContentXml, Encoding.UTF8, "application/xml")
                }));

            this.provider = new HafasProvider(HafasEndpoints.DSB, msgHandlerMock.Object);
        }

        [TestMethod]
        public void DeserializeXML()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DepartureBoard));
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(responseContentXml);
            writer.Flush();
            stream.Position = 0;
            var reader = XmlReader.Create(stream);
            var dpb = (DepartureBoard)serializer.Deserialize(reader);
            Assert.IsTrue(dpb.Departure.Length > 1);
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            var dpb1 = new DepartureBoard
            {
                Departure = new []
                {
                     new Departure
                    {
                         name = "foo",
                         type = DepartureType.BUS
                    },
                     new Departure
                     {

                     }
                },

                error = "nope"
            };

            XmlSerializer serializer = new XmlSerializer(typeof(DepartureBoard));
            var ms = new MemoryStream();
            serializer.Serialize(ms, dpb1);
            ms.Position = 0;
            var dpb2 = (DepartureBoard)serializer.Deserialize(ms);
            Assert.IsTrue("foo" == dpb2.Departure[0].name);
        }

        [TestMethod]
        public void GetDepartureBoard()
        {
            var dpb = this.provider.GetDepartureBoard("9472",  DateTime.Parse("2018-05-26 07:02"));
            Assert.IsNotNull(dpb);
        }

        [TestMethod]
        [Ignore]
        public void GetDSBDepartureBoard()
        {
            var dsb = new HafasProvider(HafasEndpoints.DSB);

            var stationId = "9472";
            var dpb = dsb.GetDepartureBoard(stationId, DateTime.Now);
            Debug.Print(
                "{0} Departures; Next: {1} {2}",
                dpb.Departure.Length,
                dpb.Departure[0].name,
                dpb.Departure[0].time);

            Assert.IsTrue(dpb.Departure.Length > 0);
        }
    }
}
