using AlmostFreeVeturilo.Logic.VeturiloApi;

namespace AlmostFreeVeturilo.Models.DatabaseModels
{
    public class StationOld
    {
        public StationOld()
        {
        }

        public StationOld(Place place)
        {
            Uid = place.uid;
            Lat = (float)place.lat;
            Lng = (float)place.lng;
            Name = place.name;
            Number = place.number;
        }

        public int Id { get; set; }
        public int Uid { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}
