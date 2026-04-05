using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHX.Models.Configuration
{
    public class GoogleConfig
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://maps.googleapis.com/maps/api/";
        public string DirectionsEndpoint { get; set; } = "directions/json";
        public string GeocodeEndpoint { get; set; } = "geocode/json";
        public string DistanceMatrixEndpoint { get; set; } = "distancematrix/json";
        public string PlacesEndpoint { get; set; } = "place/nearbysearch/json";
    }
}
