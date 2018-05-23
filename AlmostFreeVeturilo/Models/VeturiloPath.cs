using System.Collections.Generic;

namespace AlmostFreeVeturilo.Models
{
    public class VeturiloPath
    {
        public List<Station> Stations { get; set; }
        public float Cost { get; set; }

        public VeturiloPath(List<Station> stations, float cost)
        {
            Cost = cost;
            Stations = stations;
        }
    }
}
