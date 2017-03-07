using System.Runtime.Serialization;

namespace Extensions.Models
{
    [DataContract]
    public class GetCountryAndCityResponse {

        [DataMember(Name = "statusCode")]
        public string Status { get; set; }
        [DataMember(Name = "statusMessage")]
        public string StatusMessage { get; set; }
        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; set; }
        [DataMember(Name = "countryCode")]
        public string CountryCode { get; set; }
        [DataMember(Name = "countryName")]
        public string CountryName { get; set; }
        [DataMember(Name = "regionName")]
        public string RegionName { get; set; }
        [DataMember(Name = "cityName")]
        public string CityName { get; set; }
        [DataMember(Name = "zipCode")]
        public string ZipCode { get; set; }
        [DataMember(Name = "latitude")]
        public string Latitude { get; set; }
        [DataMember(Name = "longitude")]
        public string Longitude { get; set; }
        [DataMember(Name = "timeZone")]
        public string TimeZone { get; set; }

        public override string ToString()
        {
            return $"{nameof(Status)}: {Status}, {nameof(StatusMessage)}: {StatusMessage}, {nameof(IpAddress)}: {IpAddress}, {nameof(CountryCode)}: {CountryCode}, {nameof(CountryName)}: {CountryName}, {nameof(RegionName)}: {RegionName}, {nameof(CityName)}: {CityName}, {nameof(ZipCode)}: {ZipCode}, {nameof(Latitude)}: {Latitude}, {nameof(Longitude)}: {Longitude}, {nameof(TimeZone)}: {TimeZone}";
        }
    }
}