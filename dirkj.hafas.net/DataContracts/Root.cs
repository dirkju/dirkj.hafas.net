using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace dirkj.hafas.net
{
    [DataContract(Name = "root")]
    public class Root
    {
        [DataMember(Name = "departureboard")]
        public DepartureBoard departureBoard { get; set; }
    }
}
