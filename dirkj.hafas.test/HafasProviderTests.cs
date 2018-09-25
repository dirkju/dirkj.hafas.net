using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using dirkj.hafas.net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace dirkj.hafas.test
{
    [TestClass]
    public class HafasProviderTests
    {
        private const string responseContentXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<DepartureBoard xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""http://xmlopen.rejseplanen.dk/xml/rest/hafasRestDepartureBoard.xsd"">
    <Departure name=""Bus 37"" type=""BUS"" stop=""Amagerv�rket"" time=""23:10"" date=""18.09.18"" messages=""0"" finalStop=""Nordhavn St. (�stbanegade)"" direction=""Nordhavn St. via Kl�vermarksvej"">
        <JourneyDetailRef ref=""http://xmlopen.rejseplanen.dk/bin/rest.exe/journeyDetail?ref=465207%2F174138%2F101828%2F104159%2F86%3Fdate%3D18.09.18"" />
    </Departure>
    <Departure name=""Bus 37"" type=""BUS"" stop=""Amagerv�rket"" time=""05:50"" date=""19.09.18"" messages=""0"" finalStop=""Nordhavn St. (�stbanegade)"" direction=""Nordhavn St. via Kl�vermarksvej"">
        <JourneyDetailRef ref=""http://xmlopen.rejseplanen.dk/bin/rest.exe/journeyDetail?ref=356148%2F137775%2F27766%2F104833%2F86%3Fdate%3D19.09.18"" />
    </Departure>
</DepartureBoard>";

        private HafasProvider provider;

        [TestInitialize]
        public void Initialize()
        {
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
            var dpb = this.provider.GetDepartureBoard("45933",  DateTime.Parse("2018-05-26 07:02"));
            Assert.IsNotNull(dpb);
        }

        [TestMethod]
        [Ignore]
        public void GetDSBDepartureBoard()
        {
            var dsb = new HafasProvider(HafasEndpoints.DSB);

            var stationId = "45933";
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
