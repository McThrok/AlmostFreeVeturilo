using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AlmostFreeVeturilo.Logic.GoogleApi;
using Newtonsoft.Json;

namespace AlmostFreeVeturilo.Logic.VeturiloApi
{
    public class VeturiloProxy
    {
        private VeturiloData _dataCache;
        private DateTime _cacheTime = DateTime.MinValue;

        private static VeturiloProxy _instance;
        public static VeturiloProxy Instance => _instance ?? (_instance = new VeturiloProxy());
        private VeturiloProxy() { }

        public async Task<VeturiloData> GetVeturiloData()
        {
            if (DateTime.Now.Subtract(_cacheTime) < Common.CacheLivespan)
                return _dataCache;

            _cacheTime = DateTime.Now;

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://api.nextbike.net/maps/nextbike-official.json?city=210");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    _dataCache = JsonConvert.DeserializeObject<VeturiloData>(json);
                }
            }

            return _dataCache;
        }

        public async Task<List<Place>> GetVeturiloPlaces()
        {
            var data = await GetVeturiloData();
            return data.countries.First().cities.First().places;
        }
    }
}
