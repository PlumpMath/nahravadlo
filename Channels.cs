using System;
using System.Xml;

namespace Nahravadlo
{
	internal class Channels
	{
		private readonly Settings _settings;

		public Channels(Settings settings)
		{
			_settings = settings;
		}

		public Channel[] Get()
		{
			var nodes = _settings.GetNodes("nahravadlo/programy/program");
			if (nodes.Count == 0)
				nodes = _settings.GetNodes("nahravadlo/channels/channel");

			if (nodes.Count == 0)
				return new Channel[0];

			var ret = new Channel[nodes.Count];
		    var i = 0;
			foreach (XmlNode node in nodes) 
            {
				string name = "", uri = "", id = "";
				
				//back compatibility
				if (node.SelectNodes("nazev").Count != 0)
					name = node.SelectSingleNode("nazev").InnerText;

				if (node.SelectNodes("name").Count != 0)
					name = node.SelectSingleNode("name").InnerText;
				if (node.SelectNodes("uri").Count != 0)
					uri = node.SelectSingleNode("uri").InnerText;
				if (node.SelectNodes("id").Count != 0)
					id = node.SelectSingleNode("id").InnerText;

				ret[i] = new Channel(name, uri, id);
                i++;
			}

			return ret;
		}

		public Channel ChannelFromUri(string uri)
		{
			var ch = Get();

			for (var i = 0; i < ch.Length; i++)
			{
				if (ch[i].Uri == uri)
					return ch[i];
			}
			return null;
		}

		public Channel ChannelFromId(string id)
		{
			var ch = Get();
			id = id.ToLower();

			for (var i = 0; i < ch.Length; i++)
			{
				var chIds = ch[i].Id.Split(',');
				foreach (var chId in chIds)
				{
					if (chId.Trim().ToLower().CompareTo(id) == 0)
						return ch[i];
				}
			}
			return null;
		}

		public void Set(Channel[] channels)
		{
			//delete old nodes
			var nodes = _settings.GetNodes("nahravadlo/programy");
			if (nodes.Count != 0)
			{
				var n = _settings.GetNode("nahravadlo/programy");
				n.ParentNode.RemoveChild(n);
			}

			nodes = _settings.GetNodes("nahravadlo/channels");
			if (nodes.Count != 0)
			{
				var n = _settings.GetNode("nahravadlo/channels");
				n.ParentNode.RemoveChild(n);
			}

			var node = _settings.CreateSelectNodes("nahravadlo/channels");

			for (var i = 0; i < channels.Length; i++)
			{
				var chan = node.AppendChild(_settings.GetXmlDocument().CreateElement("channel"));

				if (channels[i].Name.Length > 0)
					chan.AppendChild(_settings.GetXmlDocument().CreateElement("name")).InnerText = channels[i].Name;
				if (channels[i].Uri.Length > 0)
					chan.AppendChild(_settings.GetXmlDocument().CreateElement("uri")).InnerText = channels[i].Uri;
				if (channels[i].Id.Length > 0)
					chan.AppendChild(_settings.GetXmlDocument().CreateElement("id")).InnerText = channels[i].Id;
			}
		}

		public Channel[] LoadFromFile(string filename)
		{
			var imp = new XmlDocument();
			try
			{
				imp.Load(filename);
                if (imp.DocumentElement == null)
                    return new Channel[0];

				var nspath = imp.DocumentElement.GetAttribute("xmlns");
				var ns = new XmlNamespaceManager(imp.NameTable);
				ns.AddNamespace(string.Empty, nspath);
				ns.AddNamespace("xspf", nspath);

				var nodes = imp.SelectNodes("xspf:playlist/xspf:trackList/xspf:track", ns);
				if (nodes != null)
				{
					var ret = new Channel[nodes.Count];
					for (var i = 0; i < nodes.Count; i++)
					{
						var id = "";

						var uri = nodes[i].SelectSingleNode("xspf:location", ns).InnerText;
						var name = uri;
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

		public void SaveToFile(string filename, Channel[] channels)
		{
			var exp = new XmlDocument();

			var node = exp.AppendChild(exp.CreateElement("playlist"));

			node.Attributes.Append(exp.CreateAttribute("version")).InnerText = "0";
			node.Attributes.Append(exp.CreateAttribute("xmlns")).InnerText = "http://xspf.org/ns/0/";

			node = node.AppendChild(exp.CreateElement("trackList"));

			for (var i = 0; i < channels.Length; i++)
			{
				var track = node.AppendChild(exp.CreateElement("track"));

				track.AppendChild(exp.CreateElement("location")).InnerText = channels[i].Uri;
				if (channels[i].Id.Length > 0)
					track.AppendChild(exp.CreateElement("identifier")).InnerText = channels[i].Id;
				track.AppendChild(exp.CreateElement("title")).InnerText = channels[i].Name;
			}

			if (!exp.OuterXml.Contains("<?xml"))
				exp.InsertBefore(exp.CreateXmlDeclaration("1.0", "utf-8", null), exp.DocumentElement);

			exp.Save(filename);
		}

		public Channel[] DefaultChannels
		{
            get
            {
                return new[]
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
	}

	internal class Channel
	{
		
		public Channel(string name, string uri, string id)
		{
			Name = name;
			Uri = uri;
			Id = id;
		}

        public string Name
        {
            get; set;
        }

        public string Uri
        {
            get; set;
        }

        public string Id
        {
            get; set;
        }

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