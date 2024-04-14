using System.Xml;

namespace TrayWeather3
{
    internal class XMLWorker
    {
        public XMLWorker() { }

        public TwOptions LoadConfig(string xmlFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // получим корневой элемент
            XmlElement? xRoot = doc.DocumentElement;
            TwOptions opt = new TwOptions();
            if (xRoot != null)
            {
                // обход всех узлов в корневом элементе
                foreach (XmlElement xnode in xRoot)
                {
                    if (xnode.Name == "cnm") // city name
                    {
                        opt.cnm = xnode.InnerText;
                    }
                    if (xnode.Name == "id1")   // city id 1 gismeteo
                    {
                        opt.id1 = xnode.InnerText;
                    }
                    if (xnode.Name == "id2")   // city id 2 accuweather
                    {
                        opt.id2 = xnode.InnerText;
                    }
                    if (xnode.Name == "id3")   // city id 3 meteovesti
                    {
                        opt.id3= xnode.InnerText;
                    }
                    if (xnode.Name == "id4")   // city id 4 rp5
                    {
                        opt.id4 = xnode.InnerText;
                    }
                    if (xnode.Name == "id5")   // city id 5 openweathermap
                    {
                        opt.id5 = xnode.InnerText;
                    }
                    if (xnode.Name == "rph") // run per hour
                    {
                        opt.rph = xnode.InnerText;
                    }
                    if (xnode.Name == "dhi") // default host item
                    {
                        opt.dhi = xnode.InnerText;
                    }
                    if (xnode.Name == "icl") // icon color
                    {
                        opt.icl = xnode.InnerText;
                    }
                }
            }
            return opt;
        }

        public void SaveConfig(string xmlFile, TwOptions opt)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                string XmlString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<opt>\r\n  " +
                    $"<cnm>{opt.cnm}</cnm>\r\n  " +
                    $"<id1>{opt.id1}</id1>\r\n  " +
                    $"<id2>{opt.id2}</id2>\r\n  " +
                    $"<id3>{opt.id3}</id3>\r\n  " +
                    $"<id4>{opt.id4}</id4>\r\n  " +
                    $"<id5>{opt.id5}</id5>\r\n  " +
                    $"<rph>{opt.rph}</rph>\r\n  " +
                    $"<dhi>{opt.dhi}</dhi>\r\n  " +
                    $"<icl>{opt.icl}</icl>\r\n</opt>";

                doc.LoadXml(XmlString);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true; // отступы
                // Save the document to a file and auto-indent the output.
                XmlWriter writer = XmlWriter.Create(xmlFile, settings);
                doc.Save(writer);
                writer.Close(); // закроем файл, иначе ошибка!
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void LoadConfig2(string xmlFile, out List<TwHosts> lth)
        {
            lth = new List<TwHosts>();
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // получим корневой элемент
            XmlElement? xRoot = doc.DocumentElement;
            if (xRoot != null)
            {
                // обход всех узлов в корневом элементе <opt>
                foreach (XmlElement xnode in xRoot)
                {
                    TwHosts twHosts = new TwHosts();
                    foreach (XmlNode xnode2 in xnode.ChildNodes)
                    {
                        if (xnode2.Name == "hst")
                        {
                            twHosts.hst = xnode2.InnerText;
                        }
                        if (xnode2.Name == "cls")
                        {
                            twHosts.cls = xnode2.InnerText;
                        }
                        if (xnode2.Name == "end")
                        {
                            twHosts.end = xnode2.InnerText;
                        }
                        if (xnode2.Name == "rsn")
                        {
                            twHosts.rsn = xnode2.InnerText;
                        }
                        if (xnode2.Name == "eit")
                        {
                            twHosts.eit = xnode2.InnerText;
                        }
                        if (xnode2.Name == "tva")
                        {
                            twHosts.tva = xnode2.InnerText;
                        }
                    }
                    lth.Add(twHosts);
                }
            }
        }
    }
}
