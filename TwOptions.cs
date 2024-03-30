
namespace TrayWeather3
{
    internal class TwOptions
    {
        public string? key { get; set; }
        public string? q { get; set; }
        public string? rph { get; set; }
        public string? dhi { get; set; }

        public TwOptions()
        {
            key = String.Empty;
            q = String.Empty;
            rph = String.Empty;
            dhi = String.Empty;
        }

        public void SetAll(string k, string qq, string r, string d)
        {
            key = k;
            q = qq;
            rph = r;
            dhi = d;
        }
    }
}
