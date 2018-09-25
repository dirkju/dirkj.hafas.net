using System.Runtime.Serialization;

namespace dirkj.hafas.net
{
    /// <summary>
    /// The location list contains stops/stations as a result of a stops nearby request.
    /// The data of every list entry can be used for further trip or departureBoard requests.
    /// </summary>
    [DataContract]
    public class LocationList
    {
        /// <summary>
        /// An array of StopLocations
        /// </summary>
        [DataMember(Name="StopLocation")]
        public StopLocation[] Items { get; set; }
    }
}
