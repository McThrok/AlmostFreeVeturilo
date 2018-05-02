using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using AFVTry.Models;
using AFVTry.Models.DatabaseModels;

namespace AFVTry.DataAccess
{
    public class DataLoader
    {
        public void LoadData(string xml)
        {
            var stations = Deserialize<List<Station>>(xml);

            using (var db = new DataContext())
            {
                //foreach (var station in db.Connections)
                //    db.Connections.Remove(station);
                
                foreach (var station in db.Stations)
                    db.Stations.Remove(station);

                foreach (var station in stations)
                    db.Stations.Add(station);

                db.SaveChanges();
            }
        }

        private T Deserialize<T>(string xml)
        {
            T result = default(T);
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (var reader = new StringReader(xml))
            {
                result = (T)serializer.Deserialize(reader);
            }

            return result;
        }

        private string Serialize<T>(T station)
        {
            var xsSubmit = new XmlSerializer(typeof(T));
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, station);
                    xml = sww.ToString();
                }
            }

            return xml;
        }
    }
}
