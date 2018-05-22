using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oski.Hotel;

namespace Tavisca.Hotel.Engine.Performance
{
    public class SearchCriteria
    {
        public static Bounds JFK = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 40.641311, Longitude = -73.778139 }, RadiusInKm = 50 } };
        public static Bounds Paris = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 49.009691, Longitude = 2.5479 }, RadiusInKm = 50 } };
        public static Bounds Beijing = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 40.079857, Longitude = 116.603112 }, RadiusInKm = 50 } };
        public static Bounds Atlanta = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 33.640728, Longitude = -84.4277 }, RadiusInKm = 50 } };
        public static Bounds LosAngeles = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 33.941589, Longitude = -118.40853 }, RadiusInKm = 50 } };
        public static Bounds Chicago = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 41.974162, Longitude = -87.907321 }, RadiusInKm = 50 } };
        public static Bounds Dallas = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 32.899809, Longitude = -97.040335 }, RadiusInKm = 50 } };
        public static Bounds Denver = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 39.856096, Longitude = -104.673738 }, RadiusInKm = 50 } };
        public static Bounds SanFrancisco = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 37.621313, Longitude = -122.378955 }, RadiusInKm = 50 } };
        public static Bounds Charlotte = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 35.214403, Longitude = -80.947315 }, RadiusInKm = 50 } };
        public static Bounds LAS = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 36.1699, Longitude = 115.1398 }, RadiusInKm = 50 } };
        public static Bounds Phoenix = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 33.437269, Longitude = -112.007788 }, RadiusInKm = 50 } };
        public static Bounds Vancouver = new Bounds { Circle = new Circle { Center = new GeoCode { Latitude = 49.282729, Longitude = -123.120738 }, RadiusInKm = 50 } };
    }
}
