using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlmostFreeVeturilo.Models;

namespace AlmostFreeVeturilo.Logic.PathFinding
{
    public class StartStationsFinder : BaseFinder
    {
        public async Task<List<PathPart>> GetStartStations(float lat, float lng)
        {

            var usefulStations = await GetStations(true);
            var takenStations = usefulStations.OrderBy(x => MathF.Pow(x.Lat - lat, 2) + MathF.Pow(x.Lng - lng, 2)).Take(Common.MaxMatrixRequest).ToList();
            var stationWithDistances = await GetPathParts(lat, lng, takenStations);
            return stationWithDistances.OrderBy(x => x.Distance).Take(Common.StartStationsNumber).ToList();
        }
    }
}
