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
                        new Channel("ÈT 1", "rtp://@233.10.47.81:1234", "ct1.ceskatelevize.cz"),
						new Channel("ÈT 2", "rtp://@233.10.47.82:1234", "ct2.ceskatelevize.cz"),
						new Channel("Nova", "rtp://@233.11.36.88:1234", "nova.nova.cz"),
						new Channel("Nova Cinema", "rtp://@233.11.36.89:1234", "cinema.nova.cz"),
						new Channel("Prima", "rtp://@233.11.36.90:1234", "prima.iprima.cz"),
						new Channel("Prima Cool", "rtp://@233.11.36.91:1234", "primacool.prima-cool.cz"),
						new Channel("TV Barandov", "rtp://@233.11.36.92:1234", "barandov.barrandov.tv"),
						new Channel("ÈT 24", "rtp://@233.10.47.83:1234", "ct24.ct24.cz"),
						new Channel("ÈT 4 Sport", "rtp://@233.10.47.84:1234", "ct4sport.ct24.cz"),
						new Channel("Óèko", "rtp://@233.10.47.66:1234", "ocko.idnes.cz"),
						new Channel("JOJ", "rtp://@233.10.47.13:1234", "joj.joj.sk"),
						new Channel("JOJ Plus", "rtp://@233.10.47.22:1234", "jojplus.joj.sk"),
						new Channel("Markíza", "rtp://@233.10.47.70:1234", "markiza.markiza.sk"),
						new Channel("TV Doma", "rtp://@233.10.47.92:1234", "doma.markiza.sk"),
						new Channel("STV1", "rtp://@233.10.47.71:1234", "stv1.stv.sk"),
						new Channel("STV2", "rtp://@233.10.47.72:1234", "stv2.stv.cz"),
						new Channel("STV3", "rtp://@233.10.47.76:1234", "stv3.stv.sk"),
						new Channel("TA3", "rtp://@233.10.47.11:1234", "ta3.ta3.com"),
						new Channel("TV Noe", "rtp://@233.10.47.85:1234", "tvnoe.tvnoe.cz"),
						new Channel("Public TV", "rtp://@233.11.36.98:1234", "publictv.publictv.cz"),
						new Channel("Z1", "rtp://@233.11.36.99:1234", "z1.z1tv.cz"),
						new Channel("Eurosport", "rtp://@233.10.47.78:1234", "eurosport.eurosport.com"),
						new Channel("Das erste", "rtp://@233.10.47.12:1234", "daserste.daserste.de"),
						new Channel("ZDF", "rtp://@233.10.47.17:1234", "zdf.zdf.de"),
						new Channel("ZDF neo", "rtp://@233.10.47.20:1234", "neo.zdf.de"),
						new Channel("ZDF theater", "rtp://@233.10.47.24:1234", "zdftheater.theaterkanal.de"),
						new Channel("ZDF info", "rtp://@233.10.47.23:1234", "infokanal.zdf.de"),
						new Channel("BR", "rtp://@233.10.47.15:1234", "br.br-online.de"),
						new Channel("BR alfa", "rtp://@233.10.47.38:1234", "alpha.br-online.de"),
						new Channel("RTL", "rtp://@233.10.47.33:1234", "rtl.rtl.de"),
						new Channel("RTL2", "rtp://@233.10.47.34:1234", "rtl2.rtl.de"),
						new Channel("SuperRTL", "rtp://@233.10.47.35:1234", "superrtl.rtl.de"),
						new Channel("SAT1", "rtp://@233.10.47.27:1234", "sat1.sat1.de"),
						new Channel("3sat", "rtp://@233.10.47.18:1234", "3sat.3sat.de"),
						new Channel("Pro 7", "rtp://@233.10.47.28:1234", "pro7.prosieben.de"),
						new Channel("Kabel Eins", "rtp://@233.10.47.29:1234", "kabel1.kabeleins.de"),
						new Channel("HR TV", "rtp://@233.10.47.21:1234", "hrtv.hr-online.de"),
						new Channel("WDR", "rtp://@233.10.47.25:1234", "wdr.wdr.de"),
						new Channel("VOX", "rtp://@233.10.47.36:1234", "vox.vox.de"),
						new Channel("SWR BW", "rtp://@233.10.47.75:1234", "swr.swr.de"),
						new Channel("MTV", "rtp://@233.10.47.68:1234", "mtv.mtv.tv"),
						new Channel("KI.KA", "rtp://@233.10.47.19:1234", "ki.ka.kika.de"),
						new Channel("Nick/Viva", "rtp://@233.10.47.77:1234", "viva.viva.tv"),
						new Channel("NT1", "rtp://@233.10.47.69:1234", "nt1.nt1.tv"),
						new Channel("N24", "rtp://@233.10.47.30:1234", "n24.n24.de"),
						new Channel("Euronews", "rtp://@233.10.47.67:1234", "euronews.euronews.net"),
						new Channel("ÈT1 HD (H264)", "rtp://@233.11.36.120:1234", ""),
						new Channel("Nova HD (H264)", "rtp://@233.11.36.121:1234", ""),
						new Channel("Prima HD (H264)", "rtp://@233.11.36.95:1234", ""),
						new Channel("Das erste HD (H264)", "rtp://@233.10.47.94:1234", ""),
						new Channel("ZDF HD (H264)", "rtp://@233.10.47.95:1234", ""),
						new Channel("Arte HD (H264)", "rtp://@233.10.47.96:1234", ""),
						new Channel("Anixe HD (H264)", "rtp://@233.10.47.97:1234", ""),
						new Channel("Servus TV HD (H264)", "rtp://@233.10.47.98:1234", ""),
						new Channel("WDR - Eins Festival HD (H264)", "rtp://@233.10.47.64:1234", "einsfestival.einsfestival.de"),
						new Channel("Astra promo (H264)", "rtp://@233.10.47.65:1234", ""),
						new Channel("O2 info", "rtp://@233.11.36.122:1234", "")
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