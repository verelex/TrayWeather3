
namespace TrayWeather3
{
    internal class TwHosts
    {
        public string? hst { get; set; } // host = web site addr (url)
        public string? cls { get; set; } // html class for find value
        public string? end { get; set; } // end of url (adds to end of hst^)
        public string? rsn { get; set; } // remote server name
        public string? eit { get; set; } // element index innerText
        public string? tva { get; set; } // trim values array

        public TwHosts()
        {
            hst = String.Empty;
            cls = String.Empty;
            end = String.Empty;
            rsn = String.Empty;
            eit = String.Empty;
            tva = String.Empty;
        }
    }

}
