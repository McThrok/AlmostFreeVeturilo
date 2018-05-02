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

            return await FillDistances(lat, lng, takenStations);
        }

        public async Task<IEnumerable<PathPart>> GetStationsWithBikes()
        {
            var places = await VeturiloProxy.Instance.GetVeturiloPlaces();
            return places.Where(x => x.bikes >= Common.MinBikesOnStation).Select(x => new PathPart(x.uid, (float)x.lat, (float)x.lng));
        }

        private async Task<List<PathPart>> FillDistances(float lat, float lng, IEnumerable<PathPart> parts)
        {
            var stations = parts.ToList();

            var origins = new List<(float lat, float lng)> { (lat, lng) };
            var destinations = stations.Select(x => (x.Lat, x.Lng));

            var connectionMatrix = await  GoogleProxy.Instance.GetConnectionMatrix(origins, destinations, false);
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


        public async Task<VeturiloPath> GetPath(PathPart start, float lat, float lng)
        {
            var end = await GetEndStation(lat, lng);

            var usefulStations = await GetStationsWithBikes();
            //take few closest
            //take last
            //

            return null;
        }

        public async Task<PathPart> GetEndStation(float lat, float lng)
        {
            var places = await VeturiloProxy.Instance.GetVeturiloPlaces();
            var stations = places.Select(x => new PathPart(x.uid, (float)x.lat, (float)x.lng));
            var takenStations = stations.OrderBy(x => MathF.Pow(x.Lat - lat, 2) + MathF.Pow(x.Lng - lng, 2)).Take(Common.MaxMatrixRequest);

            var potentialEndStations = await FillDistances(lat, lng, takenStations);
            return potentialEndStations.OrderBy(x => x.Distance).First();
        }
    }
}
