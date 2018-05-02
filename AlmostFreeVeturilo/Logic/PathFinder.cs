using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using AlmostFreeVeturilo.DataAccess;
using AlmostFreeVeturilo.Logic.GoogleApi;
using AlmostFreeVeturilo.Logic.VeturiloApi;
using AlmostFreeVeturilo.Models;
using AlmostFreeVeturilo.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

            var connectionMatrix = await GoogleProxy.Instance.GetConnectionMatrix(origins, destinations, false);
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

            var uids = usefulStations.Select(x => x.Uid).ToList();

            if (!uids.Contains(start.Uid))
                uids.Add(start.Uid);

            var edges = GetEdges(GetConnections(uids, end.Uid));
            //dijkstra
            //path



            return null;
        }

        private EdgeCollections GetEdges(IEnumerable<Connection> connections)
        {
            return null;
        }

        public class EdgeCollections
        {
            private Dictionary<int, Dictionary<int, float>> _weights = new Dictionary<int, Dictionary<int, float>>();

            public void AddEdge(int from, int to, float weight)
            {
                Adjust(ref from, ref to);
                if (!_weights.ContainsKey(from))
                    _weights[from] = new Dictionary<int, float>();
                _weights[from][to] = weight;
            }

            public float GetWeght(int from, int to)
            {
                Adjust(ref from, ref to);
                return _weights[from][to];
            }

            private void Adjust(ref int from, ref int to)
            {
                if (from > to)
                {
                    var x = from;
                    from = to;
                    to = x;
                }
            }

        }

        private IEnumerable<Connection> GetConnections(IEnumerable<int> uids, int endUid)
        {
            IEnumerable<Connection> connections = null;
            using (var db = new DataContext())
            {
                connections = db.Connections.Where(x =>
                    uids.Contains(x.StationFromUid) && uids.Contains(x.StationToUid) ||
                     x.StationToUid == endUid && x.StationFromUid == endUid);
            }
            return connections;
        }

        private async Task<PathPart> GetEndStation(float lat, float lng)
        {
            var places = await VeturiloProxy.Instance.GetVeturiloPlaces();
            var stations = places.Select(x => new PathPart(x.uid, (float)x.lat, (float)x.lng));
            var takenStations = stations.OrderBy(x => MathF.Pow(x.Lat - lat, 2) + MathF.Pow(x.Lng - lng, 2)).Take(Common.MaxMatrixRequest);

            var potentialEndStations = await FillDistances(lat, lng, takenStations);
            return potentialEndStations.OrderBy(x => x.Distance).First();
        }
    }
}
