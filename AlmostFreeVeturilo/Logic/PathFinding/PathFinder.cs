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

            var usefulStations = await GetStations(true);
            var takenStations = usefulStations.OrderBy(x => MathF.Pow(x.Lat - lat, 2) + MathF.Pow(x.Lng - lng, 2)).Take(Common.MaxMatrixRequest);

            return await FillDistances(lat, lng, takenStations);
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

        public async Task<List<PathPart>> GetPath(int startUid, float lat, float lng)
        {
            var end = await GetEndStation(lat, lng);

            var usefulStations = (await GetStations(true, new[] { startUid, end.Uid })).ToList();

            var edges = GetEdges(GetConnections(usefulStations.Select(x => x.Uid), end.Uid));
            var path = new Graph(edges).GetShortestPath(startUid, end.Uid);

            var veturiloPath = path.Take(path.Count - 1).Select(uid => usefulStations.First(x => x.Uid == uid)).ToList();
            veturiloPath.Add(end);

            return veturiloPath;
        }
        private EdgeCollection GetEdges(IEnumerable<Connection> connections)
        {
            var result = new EdgeCollection();

            foreach (var connection in connections)
            {
                var weight = MathF.Max(0, connection.Time + Common.ChangeBikePenalty - Common.FreeVeturiloTime) + Common.ChangeBikePenalty;
                result.AddEdge(connection.StationFromUid, connection.StationToUid, weight);
            }

            return result;
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

        private async Task<IEnumerable<PathPart>> GetStations(bool withBikes, IEnumerable<int> forcedUids = null)
        {
            var places = await VeturiloProxy.Instance.GetVeturiloPlaces();
            var result = places.Where
                (x => !withBikes || x.bikes >= Common.MinBikesOnStation
                                 || forcedUids == null || forcedUids.Contains(x.uid))
                .Select(x => new PathPart(x.uid, (float)x.lat, (float)x.lng));
            return result;
        }

    }
}
