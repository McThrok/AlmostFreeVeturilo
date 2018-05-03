using System;

namespace AlmostFreeVeturilo.Logic
{
    public static class Common
    {
        public const int MaxMatrixRequest = 25;
        public const int MinBikesOnStation = 3;
        public const int StartStationsNumber = 7;
        public const int FreeVeturiloTime = 1200;
        public const int ChangeBikeTime = 60;
        public const int ChangeBikePenalty = 1;
        public static TimeSpan CacheLivespan => new TimeSpan(0, 0, 30);
    }
}
