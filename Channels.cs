using System;
using System.Xml;

namespace Nahravadlo
{
    internal class Channels
    {
        private readonly Settings option;

        public Channels(Settings option)
        {
            this.option = option;
        }

        public Channel[] getChannels()
        {
            XmlNodeList nodes = option.getNodes("nahravadlo/programy/program");
            if (nodes.Count == 0)
                nodes = option.getNodes("nahravadlo/channels/channel");

            if (nodes.Count == 0) return new Channel[0];

            var ret = new Channel[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                string name = "", uri = "", id = "";
                XmlNode node = nodes[i];

                //back compatibility
                if (node.SelectNodes("nazev").Count != 0) name = node.SelectSingleNode("nazev").InnerText;

                if (node.SelectNodes("name").Count != 0) name = node.SelectSingleNode("name").InnerText;
                if (node.SelectNodes("uri").Count != 0) uri = node.SelectSingleNode("uri").InnerText;
                if (node.SelectNodes("id").Count != 0) id = node.SelectSingleNode("id").InnerText;

                ret[i] = new Channel(name, uri, id);
            }

            return ret;
        }

        public Channel getChannelFromUri(string uri)
        {
            Channel[] ch = getChannels();

            for (int i = 0; i < ch.Length; i++)
                if (ch[i].Uri == uri) return ch[i];
            return null;
        }

        public Channel getChannelFromId(string id)
        {
            Channel[] ch = getChannels();
            id = id.ToLower();

            for (int i = 0; i < ch.Length; i++)
            {
                string[] chIds = ch[i].Id.Split(',');
                foreach (string chId in chIds)
                {
                    if (chId.Trim().ToLower().CompareTo(id) == 0)
                        return ch[i];
                }
            }
            return null;
        }

        public void setChannels(Channel[] channels)
        {
            //delete old nodes
            XmlNodeList nodes = option.getNodes("nahravadlo/programy");
            if (nodes.Count != 0)
            {
                XmlNode n = option.getNode("nahravadlo/programy");
                n.ParentNode.RemoveChild(n);
            }

            nodes = option.getNodes("nahravadlo/channels");
            if (nodes.Count != 0)
            {
                XmlNode n = option.getNode("nahravadlo/channels");
                n.ParentNode.RemoveChild(n);
            }

            XmlNode node = option.getNode("nahravadlo");
            node = node.AppendChild(option.getXMLDocument().CreateElement("channels"));

            for (int i = 0; i < channels.Length; i++)
            {
                XmlNode chan = node.AppendChild(option.getXMLDocument().CreateElement("channel"));

                if (channels[i].Name.Length > 0)
                    chan.AppendChild(option.getXMLDocument().CreateElement("name")).InnerText = channels[i].Name;
                if (channels[i].Uri.Length > 0)
                    chan.AppendChild(option.getXMLDocument().CreateElement("uri")).InnerText = channels[i].Uri;
                if (channels[i].Id.Length > 0)
                    chan.AppendChild(option.getXMLDocument().CreateElement("id")).InnerText = channels[i].Id;
            }
        }

        public Channel[] loadChannelsFromFile(string filename)
        {
            var imp = new XmlDocument();
            try
            {
                imp.Load(filename);
                string nspath = imp.DocumentElement.GetAttribute("xmlns");
                var ns = new XmlNamespaceManager(imp.NameTable);
                ns.AddNamespace(string.Empty, nspath);
                ns.AddNamespace("xspf", nspath);

                XmlNodeList nodes = imp.SelectNodes("xspf:playlist/xspf:trackList/xspf:track", ns);
                if (nodes != null)
                {
                    var ret = new Channel[nodes.Count];
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        string id = "";

                        string uri = nodes[i].SelectSingleNode("xspf:location", ns).InnerText;
                        string name = uri;
                        if (nodes[i].SelectNodes("xspf:title", ns).Count != 0)
                            name = nodes[i].SelectSingleNode("xspf:title", ns).InnerText;
                        if (nodes[i].SelectNodes("xspf:identifier", ns).Count != 0)
                            id = nodes[i].SelectSingleNode("xspf:identifier", ns).InnerText;

                        ret[i] = new Channel(name, uri, id);
                    }
                    return ret;
                }
                return new Channel[0];
            }
            catch
            {
                throw new ChannelFormatException();
            }
        }

        public void saveChannelsToFile(string filename, Channel[] channels)
        {
            var exp = new XmlDocument();

            XmlNode node = exp.AppendChild(exp.CreateElement("playlist"));

            node.Attributes.Append(exp.CreateAttribute("version")).InnerText = "0";
            node.Attributes.Append(exp.CreateAttribute("xmlns")).InnerText = "http://xspf.org/ns/0/";

            node = node.AppendChild(exp.CreateElement("trackList"));

            for (int i = 0; i < channels.Length; i++)
            {
                XmlNode track = node.AppendChild(exp.CreateElement("track"));

                track.AppendChild(exp.CreateElement("location")).InnerText = channels[i].Uri;
                if (channels[i].Id.Length > 0)
                    track.AppendChild(exp.CreateElement("identifier")).InnerText = channels[i].Id;
                track.AppendChild(exp.CreateElement("title")).InnerText = channels[i].Name;
            }

            if (!exp.OuterXml.Contains("<?xml"))
                exp.InsertBefore(exp.CreateXmlDeclaration("1.0", "utf-8", null), exp.DocumentElement);

            exp.Save(filename);
        }

        public Channel[] getDefaultChannels()
        {
            return
                new[]
                    {
                        new Channel("ÈT1", "udp://@239.192.1.20:1234", "ct1.ceskatelevize.cz"),
                        new Channel("ÈT2", "udp://@239.192.1.21:1234", "ct2.ceskatelevize.cz"),
                        new Channel("Nova", "udp://@239.192.1.22:1234", "nova.nova.cz"),
                        new Channel("Prima", "udp://@239.192.1.23:1234", "prima.iprima.cz"),
                        new Channel("ÈT4 Sport", "udp://@239.192.1.24:1234", "ct4sport.ct24.cz"),
                        new Channel("ÈT24", "udp://@239.192.1.25:1234", "ct24.ct24.cz"),
                        new Channel("TA3", "udp://@233.10.47.11:1234", "ta3.ta3.com")
                    };
        }
    }

    internal class Channel
    {
        public Channel(string name, string uri, string id)
        {
            Name = name;
            Uri = uri;
            Id = id;
        }

        public string Name { get; set; }

        public string Uri { get; set; }

        public string Id { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    internal class ChannelFormatException : Exception
    {
        public ChannelFormatException() : base("Soubor nemá správný formát.") {}
    }
}