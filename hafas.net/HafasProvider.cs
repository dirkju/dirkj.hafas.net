using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace hafas.net
{
    public class HafasProvider
    {
        private readonly HttpClient client;
        private readonly HafasEndpoint endpoint;

        public HafasProvider(HafasEndpoint endpoint) : this(endpoint, new HttpClientHandler())
        {
        }

        public HafasProvider(string baseUrl, string dateFormat, string timeFormat, string apiVersion)
        : this(new HafasEndpoint{ BaseUrl = baseUrl, DateFormat = dateFormat, TimeFormat = timeFormat, ApiVersion = apiVersion})
        {
        }

        public HafasProvider(HafasEndpoint endpoint, HttpMessageHandler msgHandler)
        {
            this.endpoint = endpoint;
            this.client = new HttpClient(msgHandler)
            {
                BaseAddress = new Uri(this.endpoint.BaseUrl)
            };
        }

        public DepartureBoard GetDepartureBoard(string stationId, DateTime dateTime)
        {
            var query = $"departureBoard?id={stationId}&date={dateTime.ToString(this.endpoint.DateFormat)}&time={dateTime.ToString(this.endpoint.TimeFormat)}";
            var dpb = GetRESTXml(query);
            return dpb;
        }

        private Root GetRESTJson(string relativeUrl)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var body = client.GetStreamAsync(relativeUrl + "&format=json").Result;
            var json = new StreamReader(body).ReadToEnd();
            return JsonConvert.DeserializeObject<Root>(json);
        }

        private DepartureBoard GetRESTXml(string relativeUrl)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            XmlSerializer serializer = new XmlSerializer(typeof(DepartureBoard));
            var responseStream = client.GetStreamAsync(relativeUrl).Result;
            XmlReader reader = XmlReader.Create(responseStream);
            var departureBoard = (DepartureBoard)serializer.Deserialize(reader);
            reader.Close();
            return departureBoard;
        }
    }
}
