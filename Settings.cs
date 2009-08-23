using System;
using System.Xml;

namespace Nahravadlo
{
	public class Settings
	{
		public static string DefaultFilename = "config.xml";
		private static Settings _settings;

		private readonly string _filename = DefaultFilename;
		private readonly XmlDocument _xmlDoc;

		private Settings(string filename)
		{
			_xmlDoc = new XmlDocument();
			Load(filename);
		}

		private Settings() : this(DefaultFilename) {}

		public static Settings GetInstance()
		{
			if (_settings == null)
				_settings = new Settings();
			return _settings;
		}

		public void Load(string filename)
		{
			try
			{
				_xmlDoc.Load(filename);
			}
			catch {}
		}

		public void Load()
		{
			Load(_filename);
		}

		public void Save(string filename)
		{
			if (!_xmlDoc.OuterXml.Contains("<?xml"))
				_xmlDoc.InsertBefore(_xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null), _xmlDoc.DocumentElement);
			_xmlDoc.Save(filename);
		}

		public void Save()
		{
			Save(_filename);
		}

		public string GetString(string name, string defaultValue)
		{
			try
			{
				return _xmlDoc.SelectSingleNode(name).InnerText;
			}
			catch
			{
				return defaultValue;
			}
		}

		public int GetInt(string name, int defaultValue)
		{
			try
			{
				return int.Parse(_xmlDoc.SelectSingleNode(name).InnerText);
			}
			catch
			{
				return defaultValue;
			}
		}

		public bool GetBool(string name, bool defaultValue)
		{
			try
			{
				return bool.Parse(_xmlDoc.SelectSingleNode(name).InnerText);
			}
			catch
			{
				return defaultValue;
			}
		}

		public float GetFloat(string name, float defaultValue)
		{
			try
			{
				return float.Parse(_xmlDoc.SelectSingleNode(name).InnerText);
			}
			catch
			{
				return defaultValue;
			}
		}

		public void SetString(string name, string value)
		{
			try
			{
				CreateSelectNodes(_xmlDoc, name).InnerText = value;
			}
			catch {}
		}

		public void SetInt(string name, int value)
		{
			try
			{
				CreateSelectNodes(_xmlDoc, name).InnerText = value.ToString();
			}
			catch {}
		}

		public void SetBool(string name, bool value)
		{
			try
			{
				CreateSelectNodes(_xmlDoc, name).InnerText = value.ToString();
			}
			catch {}
		}

		public void SetFloat(string name, float value)
		{
			try
			{
				CreateSelectNodes(_xmlDoc, name).InnerText = value.ToString();
			}
			catch {}
		}

		public XmlNode CreateSelectNodes(string path)
		{
			return CreateSelectNodes(_xmlDoc, path);
		}

		public XmlNode CreateSelectNodes(XmlDocument xmlDoc, string path)
		{
			if (path.Substring(0, 2) == "//")
				path = path.Substring(2);

			if (xmlDoc.SelectNodes(path).Count != 0)
				return xmlDoc.SelectSingleNode(path);

			if (path.IndexOf("/") == -1)
				return xmlDoc.AppendChild(xmlDoc.CreateElement(path));

			var pathItems = path.Split('/');
			XmlNode node = xmlDoc;
			for (var i = 0; i < pathItems.Length; i++)
				node = node.SelectNodes(pathItems[i]).Count != 0 ? node.SelectSingleNode(pathItems[i]) : node.AppendChild(xmlDoc.CreateElement(pathItems[i]));
			return node;
		}

		public XmlNode GetNode(string name)
		{
			return _xmlDoc.SelectSingleNode(name);
		}

		public XmlNodeList GetNodes(string name)
		{
			return _xmlDoc.SelectNodes(name);
		}

		public XmlDocument GetXmlDocument()
		{
			return _xmlDoc;
		}
	}
}