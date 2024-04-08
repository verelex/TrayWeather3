
namespace TrayWeather3
{
    internal class TwOptions
    {
        public string? cnm { get; set; } // city name
        public string? id1 { get; set; } // city id 1 gismeteo
        public string? id2 { get; set; } // city id 1 accuweather
        public string? id3 { get; set; } // city id 1 meteovesti
        public string? idn { get; set; } // city id for RP5
        public string? rph { get; set; } // run per hour
        public string? dhi { get; set; } // default host item
        public string? icl { get; set; } // icon pack dir

        public TwOptions()
        {
            cnm = String.Empty;
            id1 = String.Empty;
            id2 = String.Empty;
            id3 = String.Empty;
            idn = String.Empty;
            rph = String.Empty;
            dhi = String.Empty;
            icl = String.Empty;
        }

        public void SetAll(string c, string i1, string i2, string i3, string id,string r,string d, string il)
        {
            cnm = c;
            id1 = i1;
            id2 = i2;
            id3 = i3;
            idn = id;
            rph = r;
            dhi = d;
            icl = il;
        }
    }
}
