using System.Collections.Generic;

namespace AlmostFreeVeturilo.Logic.VeturiloApi
{
    public class SouthWest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class NorthEast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Bounds
    {
        public SouthWest south_west { get; set; }
        public NorthEast north_east { get; set; }
    }

    public class Place
    {
        public int uid { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public bool bike { get; set; }
        public string name { get; set; }
        public object address { get; set; }
        public bool spot { get; set; }
        public int number { get; set; }
        public int bikes { get; set; }
        public int bike_racks { get; set; }
        public int free_racks { get; set; }
        public bool maintenance { get; set; }
        public string terminal_type { get; set; }
        public List<object> bike_list { get; set; }
        public object bike_numbers { get; set; }
        public object bike_types { get; set; }
        public string place_type { get; set; }
        public bool rack_locks { get; set; }
    }

    public class City
    {
        public int uid { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int zoom { get; set; }
        public string maps_icon { get; set; }
        public string alias { get; set; }
        public bool @break { get; set; }
        public string name { get; set; }
        public string num_places { get; set; }
        public string refresh_rate { get; set; }
        public Bounds bounds { get; set; }
        public int booked_bikes { get; set; }
        public int set_point_bikes { get; set; }
        public int available_bikes { get; set; }
        public List<Place> places { get; set; }
    }

    public class Country
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int zoom { get; set; }
        public string name { get; set; }
        public string hotline { get; set; }
        public string domain { get; set; }
        public string language { get; set; }
        public string email { get; set; }
        public string timezone { get; set; }
        public string currency { get; set; }
        public string country_calling_code { get; set; }
        public string system_operator_address { get; set; }
        public string country { get; set; }
        public string country_name { get; set; }
        public string terms { get; set; }
        public string policy { get; set; }
        public string website { get; set; }
        public bool show_bike_types { get; set; }
        public bool show_bike_type_groups { get; set; }
        public bool show_free_racks { get; set; }
        public int booked_bikes { get; set; }
        public int set_point_bikes { get; set; }
        public int available_bikes { get; set; }
        public bool capped_available_bikes { get; set; }
        public List<City> cities { get; set; }
    }

    public class VeturiloData
    {
        public List<Country> countries { get; set; }
    }
}
