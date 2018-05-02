namespace AFVTry.Models.DatabaseModels
{
    public class Connection_old
    {
        public int Id { get; set; }
        public Station StationFrom { get; set; } // lower Uid
        public Station StationTo { get; set; }// higher Uid
        public int Distance { get; set; }
        public int Time { get; set; }
    }
}