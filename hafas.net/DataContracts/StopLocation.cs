using System.Runtime.Serialization;

namespace hafas.net
{
    /// <summary>
    /// StopLocation specifies  a stop/station in a result of a location request. It contains an output name and an id.
    /// </summary>
    [DataContract]
    public class StopLocation
    {
        /// <summary>
        /// Contains the output name of this stop or station
        /// </summary>
        [DataMember(Name="name")]
        public string name { get; set; }

        /// <summary>
        /// The WGS84 x coordinate as integer (multiplied by 1,000,000)
        /// </summary>
        [DataMember(Name="x")]
        public int x { get; set; }

        /// <summary>
        /// The WGS84 y coordinate as integer (multiplied by 1,000,000)
        /// </summary>
        [DataMember(Name="y")]
        public int y { get; set; }

        /// <summary>
        /// As the crow flies distance from coordinate in meter.
        /// </summary>
        [DataMember(Name="distance")]
        public int distance { get; set; }

        /// <summary>
        /// This ID can either be used as originId or destId to perform a trip request or to call a departure board.
        /// </summary>
        [DataMember(Name="id")]
        public string id { get; set; }
    }
}
