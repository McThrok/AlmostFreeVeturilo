namespace AFVTry.Models.DatabaseModels
{
    public class Connection
    {
        public int Id { get; set; }
        public int StationFromUid { get; set; } // lower Uid
        public int StationToUid { get; set; }// higher Uid
        public int Distance { get; set; }
        public int Time { get; set; }
    }
}