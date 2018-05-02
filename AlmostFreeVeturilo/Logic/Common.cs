using System;

namespace AlmostFreeVeturilo.Logic
{
    public static class Common
    {
        public const int MaxMatrixRequest = 25;
        public const int MinBikesOnStation = 3;
        public const int StartStationsNumber = 7;
        public static TimeSpan CacheLivespan => new TimeSpan(0, 0, 30);
    }
}
