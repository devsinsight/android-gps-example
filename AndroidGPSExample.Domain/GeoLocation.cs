namespace AndroidGPSExample.Domain
{
    public class GeoLocation
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoLocation(double latitude, double longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}