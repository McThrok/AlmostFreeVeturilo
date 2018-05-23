using System;
using AlmostFreeVeturilo.Logic.VeturiloApi;

namespace AlmostFreeVeturilo.Models
{
    public class Station
    {
        public int Uid { get; set; }
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public int Bikes { get; set; }

        public Station()
        {
        }

        public Station(int uid, float lat, float lng)
        {
            Uid = uid;
            Lat = lat;
            Lng = lng;
        }

        public Station(int uid, float lat, float lng, int bikes, string name) : this(uid, lat, lng)
        {
            Bikes = bikes;
            Name = name;
        }


        public Station(Place place) : this(place.uid, (float)place.lat, (float)place.lng, place.bikes, place.name)
        {
        }
    }
}
