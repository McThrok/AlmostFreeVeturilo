namespace AlmostFreeVeturilo.Models
{
    public class PathPart
    {
        public float Distance { get; set; }
        public float Time { get; set; }
        public float EstimatedCost { get; set; }
        public Station Station { get; set; }

        public PathPart()
        {
        }

        public PathPart(float time, float distance, Station station)
        {
            Time = time;
            Distance = distance;
            Station = station;
        }
    }
}
