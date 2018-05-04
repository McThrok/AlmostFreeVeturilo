namespace AlmostFreeVeturilo.Models.DatabaseModels
{
    public class ConnectionOld
    {
        public int Id { get; set; }
        public StationOld StationFrom { get; set; } // lower Uid
        public StationOld StationTo { get; set; }// higher Uid
        public int Distance { get; set; }
        public int Time { get; set; }
    }
}