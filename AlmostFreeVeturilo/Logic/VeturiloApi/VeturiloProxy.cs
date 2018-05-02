using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AFVTry.Models.RequestModels;
using Newtonsoft.Json;

namespace AFVTry.Logic
{
    public class VeturiloProxy
    {
        public async Task<VeturiloData> GetVeturiloData()
        {
            VeturiloData data = null;

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://api.nextbike.net/maps/nextbike-official.json?city=210");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<VeturiloData>(json);
                }
            }

            return data;
        }
    }
}
