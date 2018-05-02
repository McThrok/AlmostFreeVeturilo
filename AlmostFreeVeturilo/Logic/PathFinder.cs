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

            var takenStations = usefulStations.OrderBy(x => MathF.Pow(x.Lat - lat, 2) + MathF.Pow(x.Lng - lng, 2)).Take(Common.MaxMatrixRequest);
            return await MockGetDistances(lat, lng, takenStations);
        }

        private async Task<List<PathPart>> MockGetDistances(float lat, float lng, IEnumerable<PathPart> parts)
        {
            var stations = parts.ToList();

            var origins = new List<(float lat, float lng)> { (lat, lng) };
            var destinations = stations.Select(x => (x.Lat, x.Lng));

            var connectionMatrix = await new GoogleProxy().GetConnectionMatrix(origins, destinations);
            if (connectionMatrix.status != "OK")
                return null;

            for (int i = 0; i < stations.Count; i++)
            {
                var el = connectionMatrix.rows[0].elements[i];

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
