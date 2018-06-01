using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlmostFreeVeturilo.DataAccess;
using AlmostFreeVeturilo.Logic.VeturiloApi;
using AlmostFreeVeturilo.Models;
using AlmostFreeVeturilo.Models.DatabaseModels;

namespace AlmostFreeVeturilo.Logic.PathFinding
{
    public class PathFinder : BaseFinder
    {
        public async Task<VeturiloPath> GetPath(int startUid, float lat, float lng, int minBikes, float timeFactor)
        {
            var end = await GetEndStation(lat, lng);

            var usefulStations = (await GetStations(minBikes, new[] { startUid, end.Station.Uid })).ToList();

            var edges = GetEdges(GetConnections(usefulStations.Select(x => x.Uid)), timeFactor);
            var path = new Graph(edges).GetShortestPath(startUid, end.Station.Uid);

            var stationList = path.uids.Select(uid => usefulStations.First(x => x.Uid == uid)).ToList();

            return new VeturiloPath(stationList, path.cost);
        }

        private EdgeCollection GetEdges(IEnumerable<Connection> connections, float timeFactor)
        {
            var result = new EdgeCollection();

            foreach (var connection in connections)
            {
                var time = (connection.Time + Common.ChangeBikeTime) * timeFactor;
                result.AddEdge(connection.StationFromUid, connection.StationToUid, GetCost(time));
            }

            return result;
        }
        private float GetCost(float time)
        {
            var cost = 0f;
            time -= Common.FreeVeturiloTime;

            float firstThreshold = 3600 - Common.FreeVeturiloTime;
            if (time > 0 && time < firstThreshold)
            {
                cost += Math.Min(time, firstThreshold) / firstThreshold;
                time -= firstThreshold;
            }

            float secondThreshold = 3600;
            if (time > 0 && time < secondThreshold)
            {
                cost += 3 * Math.Min(time, secondThreshold) / secondThreshold;
                time -= secondThreshold;
            }

            if (time > 0 && time < secondThreshold)
            {
                cost += 5 * Math.Min(time, secondThreshold) / secondThreshold;
                time -= secondThreshold;
            }

            if (time > 0)
            {
                cost += 7 * time / secondThreshold;
            }

            cost += Common.ChangeBikePenalty;

            return cost;
        }
        private List<Connection> GetConnections(IEnumerable<int> uids)
        {
            List<Connection> connections = null;
            using (var db = new DataContext())
            {
                connections = db.Connections
                    .Where(x => uids.Contains(x.StationFromUid) && uids.Contains(x.StationToUid))
                    .ToList();
            }
            return connections;
        }
        private async Task<PathPart> GetEndStation(float lat, float lng)
        {
            var places = await VeturiloProxy.Instance.GetVeturiloPlaces();
            var stations = places.Select(x => new Station(x.uid, (float)x.lat, (float)x.lng));
            var takenStations = stations.OrderBy(x => MathF.Pow(x.Lat - lat, 2) + MathF.Pow(x.Lng - lng, 2)).Take(Common.MaxMatrixRequest).ToList();

            var potentialEndStations = await GetPathParts(lat, lng, takenStations);
            return potentialEndStations.OrderBy(x => x.Distance).First();
        }


    }
}
