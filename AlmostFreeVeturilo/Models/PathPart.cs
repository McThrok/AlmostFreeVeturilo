namespace AlmostFreeVeturilo.Models
{
    public class PathPart
    {
        public int Uid { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public int Bikes { get; set; }
        public float Distance { get; set; }
        public float Time { get; set; }

        public PathPart()
        {
        }

        public PathPart(int uid, float lat, float lng)
        {
            Uid = uid;
            Lat = lat;
            Lng = lng;
        }
    }
}
