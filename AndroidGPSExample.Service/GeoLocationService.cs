using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AndroidGPSExample.Domain;
using AndroidGPSExample.Domain.Contracts;
using Newtonsoft.Json;

namespace AndroidGPSExample.Service
{
    public class GeoLocationService : IGeoLocationService
    {
        private const string SERVICE_BASE_URL = "http://192.168.1.8";
        private const string SERVICE_ACTION = "/AndroidGPSExample.API/api/geolocation";

        public async Task SendGeoLocation(GeoLocation geolocation)
        {
            await SendData(GetRequestParams(geolocation));
        }

        private async Task SendData(StringContent stringContent)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(SERVICE_BASE_URL);
                HttpResponseMessage response = await client.PostAsync(SERVICE_ACTION, stringContent);
            }
        }

        private StringContent GetRequestParams<T>(T request) =>
            new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

    }
}