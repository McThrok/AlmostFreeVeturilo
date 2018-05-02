using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AlmostFreeVeturilo.Logic.GoogleApi
{
    public class GoogleProxy
    {
        public async Task<ConnectionMatrix> GetConnectionMatrix(IEnumerable<(float lat, float lng)> origins, IEnumerable<(float lat, float lng)> destinations)
        {
            var apiKey = ApiKeyGetter.Instance.GetKey();

            ConnectionMatrix conMatrix = null;

            using (var client = new HttpClient())
            {
                var originsQuery = string.Join('|', origins.Select(x => $"{x.lat} {x.lng}"));
                var destinationsQuery = string.Join('|', destinations.Select(x => $"{x.lat} {x.lng}"));

                var response = await client.GetAsync($"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={originsQuery}&destinations={destinationsQuery}&mode=bicycling&key={apiKey}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    conMatrix = JsonConvert.DeserializeObject<ConnectionMatrix>(json);
                }
            }
            return conMatrix;
        }
    }
}
