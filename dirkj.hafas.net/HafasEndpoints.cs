namespace dirkj.hafas.net
{
    public static class HafasEndpoints
    {
        public static HafasEndpoint DSB = new HafasEndpoint
        {
            BaseUrl = "http://xmlopen.rejseplanen.dk/bin/rest.exe/",
            DateFormat = "dd.MM.yyyy",
            TimeFormat = "HH:mm"
        };

        public static HafasEndpoint Vasttrafik = new HafasEndpoint
        {
            BaseUrl = "https://api.vasttrafik.se/bin/rest.exe/v2",
            TokenUrl = "https://api.vasttrafik.se/token",
            DateFormat = "yyyy-MM-dd",
            TimeFormat = "HH:mm"
        };
    }
}
