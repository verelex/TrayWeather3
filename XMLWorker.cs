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
                    if (xnode.Name == "key")
                    {
                        opt.key = xnode.InnerText;
                    }
                    if (xnode.Name == "q")
                    {
                        opt.q = xnode.InnerText;
                    }
                    if (xnode.Name == "rph")
                    {
                        opt.rph = xnode.InnerText;
                    }
                    if (xnode.Name == "dhi")
                    {
                        opt.dhi = xnode.InnerText;
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
                doc.LoadXml($"<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<opt>\r\n    <key>{opt.key}</key>\r\n    <q>{opt.q}</q>\r\n    <rph>{opt.rph}</rph>\r\n    <dhi>{opt.dhi}</dhi>\r\n</opt>");
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                // Save the document to a file and auto-indent the output.
                XmlWriter writer = XmlWriter.Create(xmlFile, settings);
                doc.Save(writer);
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
                        if (xnode2.Name == "trm")
                        {
                            twHosts.trm = xnode2.InnerText;
                        }
                    }
                    lth.Add(twHosts);
                }
            }
        }
    }
}
