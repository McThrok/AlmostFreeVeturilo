using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlmostFreeVeturilo.Logic.GoogleApi;
using AlmostFreeVeturilo.Logic.VeturiloApi;
using AlmostFreeVeturilo.Models;

namespace AlmostFreeVeturilo.Logic
{
    public class PathFinder
    {
        public async Task<List<PathPart>> GetStartStations(float lat, float lng)
        {
            var usefulStations = await GetStationsWithBikes();

            return MockGetDistances(usefulStations.OrderBy(x => MathF.Pow(x.Lat - lat, 2) + MathF.Pow(x.Lng - lng, 2)).Take(Common.MaxMatrixRequest));
        }

        private List<PathPart> MockGetDistances(IEnumerable<PathPart> parts)
        {
            var stations = parts.ToList();

            var mock = new ConnectionMatrix();
            if (mock.status != "OK")
                return null;


            for (int i = 0; i < stations.Count; i++)
            {
                var el = mock.rows[0].elements[i];

                stations[i].Distance = el.distance.value;
                stations[i].Time = el.duration.value;
            }

            return stations.OrderBy(x => x.Distance).ToList();
        }

        private async Task<IEnumerable<PathPart>> GetStationsWithBikes()
        {
            var data = await new VeturiloProxy().GetVeturiloData();
            var places = data.countries.First().cities.First().places;

            return places.Where(x => x.bikes >= Common.MinBikesOnStation).Select(x => new PathPart(x.uid, (float)x.lat, (float)x.lng));
        }

        public VeturiloPath GetPath(int uid, float lat, float lng)
        {
            //take first
            //take few closest
            //take last
            //

            return null;
        }
    }
}
