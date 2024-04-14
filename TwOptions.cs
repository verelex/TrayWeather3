
namespace TrayWeather3
{
    internal class TwOptions
    {
        public string? cnm { get; set; } // city name
        public string? id1 { get; set; } // city id 1 gismeteo
        public string? id2 { get; set; } // city id 1 accuweather
        public string? id3 { get; set; } // city id 1 meteovesti
        public string? id4 { get; set; } // city id 4 for RP5
        public string? id5 { get; set; } // city id 5 for openweathermap
        public string? rph { get; set; } // run per hour
        public string? dhi { get; set; } // default host item
        public string? icl { get; set; } // icon pack dir

        public TwOptions()
        {
            cnm = String.Empty;
            id1 = String.Empty;
            id2 = String.Empty;
            id3 = String.Empty;
            id4 = String.Empty;
            id5 = String.Empty;
            rph = String.Empty;
            dhi = String.Empty;
            icl = String.Empty;
        }

        public void SetAll(string c, string i1, string i2, string i3, string i4, string i5,string r,string d, string il)
        {
            cnm = c;
            id1 = i1;
            id2 = i2;
            id3 = i3;
            id4 = i4;
            id5 = i5;
            rph = r;
            dhi = d;
            icl = il;
        }
    }
}
