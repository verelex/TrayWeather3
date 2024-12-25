
using System.Collections.Generic;

namespace TrayWeather3
{
    internal class TwOptions
    {
        public string? id1 { get; set; } // city id 1 mail.ru
        public string? id2 { get; set; } // city id 2 yandex
        public string? id3 { get; set; } // city id 3 gismeteo
        public string? id4 { get; set; } // city id 4 meteoprog
        public string? id5 { get; set; } // city id 5 accuweather
        public string? id6 { get; set; } // city id 6 meteovesti
        public string? id7 { get; set; } // city id 7 for RP5
        public string? id8 { get; set; } // city id 8 for openweathermap
        public string? rph { get; set; } // run per hour
        public string? dhi { get; set; } // default host item
        public string? icl { get; set; } // icon pack dir

        public TwOptions()
        {
            id1 = String.Empty;
            id2 = String.Empty;
            id3 = String.Empty;
            id4 = String.Empty;
            id5 = String.Empty;
            id6 = String.Empty;
            id7 = String.Empty;
            id8 = String.Empty;
            rph = String.Empty;
            dhi = String.Empty;
            icl = String.Empty;
        }

        public void SetAll(string i1, string i2, string i3, string i4, string i5, string i6, string i7, string i8, string r,string d, string il)
        {
            id1 = i1;
            id2 = i2;
            id3 = i3;
            id4 = i4;
            id5 = i5;
            id6 = i6;
            id7 = i7;
            id8 = i8;
            rph = r;
            dhi = d;
            icl = il;
        }

        public String GetElementByIndex(int index)
        {
            LinkedList<String> list = new([id1, id2, id3, id4, id5, id6, id7, id8, rph, dhi, icl]);
            return list.ElementAt(index);
        }
    }
}
