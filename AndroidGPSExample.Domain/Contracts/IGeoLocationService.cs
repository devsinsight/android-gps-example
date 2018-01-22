using System.Threading.Tasks;

namespace AndroidGPSExample.Domain.Contracts
{
    public interface IGeoLocationService
    {
        Task SendGeoLocation(GeoLocation geolocation);
    }
}