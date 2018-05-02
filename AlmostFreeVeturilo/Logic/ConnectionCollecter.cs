using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AFVTry.DataAccess;
using AFVTry.Models;
using AFVTry.Models.DatabaseModels;
using AFVTry.Models.RequestModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AFVTry.Logic
{
    public class ConnectionCollecter
    {
        public async Task LoadData()
        {
            var key = ApiKeyGetter.Instance.GetKey();

            using (var db = new DataContext())
            {
                while (true)
                {
                    var input = ChooseInputData(db);

                    var matrix = await new GoogleProxy().GetConnectionMatrix(input.origins.Select(x => (x.Lat, x.Lng)),input.destinations.Select(x => (x.Lat, x.Lng)));
                    var connections = SaveData(matrix, input.origins, input.destinations);

                    if (connections.Count == 0)
                        return;

                    db.Connections.AddRange(connections);
                    db.SaveChanges();
                }
            }

        }

        private (List<Station> origins, List<Station> destinations) ChooseInputData(DataContext db)
        {
            var stations = db.Stations.ToList();
            var isConnectionQuery = db.Stations.Select(
                   a => db.Stations.Select(
                       b => ((a.Uid < b.Uid) ? new bool?(db.Connections.Any(con => con.StationFrom.Id == a.Id && con.StationTo.Id == b.Id)) : null)
                       ).ToList()
                   ).ToList();

            var origins = new List<Station>();
            var destinations = new List<Station>();

            int[] count = { 0, 0, 0 };
            foreach (var row in isConnectionQuery)
            {
                foreach (var value in row)
                {
                    if (value == true)
                        count[0]++;
                    if (value == false)
                        count[1]++;
                    if (value == null)
                        count[2]++;
                }
            }

            for (int i = 0; i < isConnectionQuery.Count; i++)
            {
                var row = isConnectionQuery[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j].HasValue && !row[j].Value)
                    {
                        if (origins.Count == 0)
                            origins.Add(stations[i]);

                        if (destinations.Count < Common.MaxMatrixRequest)
                            destinations.Add(stations[j]);
                        else
                            break;
                    }
                }

                if (origins.Count != 0)
                    break;
            }

            return (origins, destinations);
        }
        private List<Connection_old> SaveData(ConnectionMatrix matrix, List<Station> origins, List<Station> destinations)
        {
            var connections = new List<Connection_old>();

            if (matrix.status != "OK")
                return connections;

            for (int i = 0; i < origins.Count; i++)
            {
                for (int j = 0; j < destinations.Count; j++)
                {
                    var el = matrix.rows[i].elements[j];
                    connections.Add(new Connection_old
                    {
                        Distance = el.distance.value,
                        Time = el.duration.value,
                        StationFrom = origins[i],
                        StationTo = destinations[j]
                    });
                }
            }

            return connections;
        }
    }
}

