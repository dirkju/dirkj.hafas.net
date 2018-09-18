using System;
using System.Collections.Generic;
using System.Text;

namespace hafas.net
{
    public static class HafasEndpoints
    {
        public static HafasEndpoint DSB = new HafasEndpoint{
            BaseUrl = "http://xmlopen.rejseplanen.dk/bin/rest.exe/",
            DateFormat = "dd.MM.yyyy",
            TimeFormat = "HH:mm"
        };
    }
}
