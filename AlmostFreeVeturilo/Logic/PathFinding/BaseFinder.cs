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
    public abstract class BaseFinder
    {
        protected async Task<List<PathPart>> GetPathParts(float lat, float lng, List<Station> stations)
        {

            var origins = new List<(float lat, float lng)> { (lat, lng) };
            var destinations = stations.Select(x => (x.Lat, x.Lng));

            var connectionMatrix = await GoogleProxy.Instance.GetConnectionMatrix(origins, destinations, false);
            if (connectionMatrix.status != "OK")
                return null;

            var result = stations.Select((s, i) =>
            {
                var el = connectionMatrix.rows[0].elements[i];
                return new PathPart(el.duration.value, el.distance.value, s);
            }).ToList();

            return result;
        }
        protected async Task<IEnumerable<Station>> GetStations(bool withBikes, IEnumerable<int> forcedUids = null)
        {
            var places = await VeturiloProxy.Instance.GetVeturiloPlaces();
            var result = places.Where
                (x => !withBikes || x.bikes >= Common.MinBikesOnStation
                                 || forcedUids != null && forcedUids.Contains(x.uid))
                .Select(x => new Station(x.uid, (float)x.lat, (float)x.lng, x.bikes));
            return result;
        }

    }
}
