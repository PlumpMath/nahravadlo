using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Nahravadlo {
	public partial class formSettings : Form {
		public bool isCanceled = false;

		public formSettings() {
			InitializeComponent();
			LoadData();
			isCanceled = false;
		}

		private void btnHelp_Click(object sender, EventArgs e) {
			MessageBox.Show("Pokud m� b�t nahr�v�n� spu�t�no na pozad�, tak aby nebylo vid�t okno VLC, pak je nutn� zadat jm�no a heslo existuj�c�ho u�ivatele ve Windows XP (Nejl�pe jin�ho u�ivatele, ne� pod kter�m jste aktu�ln� p�ihl�en. Heslo nesm� b�t pr�zdn�!). Pokud nevypln�te u�ivatele a heslo, VLC se spust� pod pr�v� p�ihl�en�m u�ivatelem a tento u�ivatel uvid� okno VLC v�etn� mo�nosti jeho ovl�d�n� (nahr�v�n� se neprovede, pokud u�ivatel nebude p�ihl�en!).\n\nPozn�mka: U�ivatelsk� jm�no je mo�n� zad�vat v�etn� dom�ny. U�ivatel, pod kter�m bude spu�t�no VLC mus� m�t mo�nost tuto aplikaci spustit a mus� m�t mo�nost p�istupovat i k prostoru na disku, kam se budou ukl�dat soubory.\n\nJe v�hodn� pomoc� ovl�dac�ch panel� zalo�it nov�ho u�ivatele, nejl�pe s pravomocmi spr�vce po��ta�e, nastavit mu heslo a n�sledn� tohoto u�ivatele vyplnit do t�to aplikace.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void btnSelectVLC_Click(object sender, EventArgs e) {
			openFile.FileName = txtVLCPath.Text;
			openFile.ShowDialog();
			txtVLCPath.Text = openFile.FileName;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			isCanceled = true;
			Close();
		}

		private void btnSaveAndClose_Click(object sender, EventArgs e) {
			if (!File.Exists(txtVLCPath.Text) || txtVLCPath.Text.Trim().Length == 0 || !Directory.Exists(txtDefaultDirectory.Text) || txtDefaultDirectory.Text.Trim().Length == 0) {
				MessageBox.Show("Pros�m vypl�te spr�vn� n�sleduj�c� polo�ky:\n\nCesta k VLC - mus� obsahovat cestu k VLC v�etn� spustiteln�ho souboru.\n\nV�choz� adres�� - mus� obsahovat cestu k adres��i, kam se budou ukl�dat nahran� po�ady, pokud u nich nebude uvedena absolutn� cesta.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			SaveData();
			Close();
		}

		private void LoadData() {
			txtDefaultDirectory.Text = @"C:\";

			try {
				XmlDocument objXmlDoc = new XmlDocument();
				objXmlDoc.Load(Application.StartupPath + @"\config.xml");

				XmlElement objRootXmlElement = objXmlDoc.DocumentElement;

				if (objRootXmlElement.SelectSingleNode("config/vlc") != null)
					txtVLCPath.Text = objRootXmlElement.SelectSingleNode("config/vlc").InnerText;
				if (objRootXmlElement.SelectSingleNode("config/login/username") != null)
					txtUsername.Text = objRootXmlElement.SelectSingleNode("config/login/username").InnerText;
				if (objRootXmlElement.SelectSingleNode("config/login/password") != null)
					txtPassword.Text = objRootXmlElement.SelectSingleNode("config/login/password").InnerText;
				try
				{
					chkUseMPEGTS.Checked = Boolean.Parse(objRootXmlElement.SelectSingleNode("config/use_mpegts").InnerText);
				} catch (Exception)
				{
				}

				txtDefaultDirectory.Text = objRootXmlElement.SelectSingleNode("config/defaultdirectory").InnerText;
				objXmlDoc = null;
			} catch(Exception) {
			}
		}

		private void SaveData() {
			XmlDocument objXmlDoc = new XmlDocument();
			try {
				objXmlDoc.Load(Application.StartupPath + @"\config.xml");
			} catch(Exception) {
			}

			try {
				if (objXmlDoc.SelectNodes("//nahravadlo").Count == 0)
					objXmlDoc.AppendChild(objXmlDoc.CreateElement("nahravadlo"));

				if (objXmlDoc.SelectNodes("//nahravadlo/config").Count == 0)
					objXmlDoc.SelectSingleNode("//nahravadlo").AppendChild(objXmlDoc.CreateElement("config"));
				if (objXmlDoc.SelectNodes("//nahravadlo/config/vlc").Count == 0)
					objXmlDoc.SelectSingleNode("//nahravadlo/config").AppendChild(objXmlDoc.CreateElement("vlc"));			
				if (objXmlDoc.SelectNodes("//nahravadlo/config/defaultdirectory").Count == 0)
					objXmlDoc.SelectSingleNode("//nahravadlo/config").AppendChild(objXmlDoc.CreateElement("defaultdirectory"));
				if (objXmlDoc.SelectNodes("//nahravadlo/config/use_mpegts").Count == 0)
					objXmlDoc.SelectSingleNode("//nahravadlo/config").AppendChild(objXmlDoc.CreateElement("use_mpegts"));

				if (objXmlDoc.SelectNodes("//nahravadlo/config/login").Count == 0)
					objXmlDoc.SelectSingleNode("//nahravadlo/config").AppendChild(objXmlDoc.CreateElement("login"));
				if (objXmlDoc.SelectNodes("//nahravadlo/config/login/username").Count == 0)
					objXmlDoc.SelectSingleNode("//nahravadlo/config/login").AppendChild(objXmlDoc.CreateElement("username"));
				if (objXmlDoc.SelectNodes("//nahravadlo/config/login/password").Count == 0)
					objXmlDoc.SelectSingleNode("//nahravadlo/config/login").AppendChild(objXmlDoc.CreateElement("password"));

				if (objXmlDoc.SelectNodes("//nahravadlo/programy").Count == 0)
					objXmlDoc.SelectSingleNode("//nahravadlo").AppendChild(objXmlDoc.CreateElement("programy"));

				if (objXmlDoc.SelectNodes("//nahravadlo/programy/program").Count == 0) {
					String[,] programs = {{"�T 1", "udp://@239.192.1.20:1234"}, {"�T 2", "udp://@239.192.1.21:1234"}, {"Nova", "udp://@239.192.1.22:1234"}, {"Prima", "udp://@239.192.1.23:1234"}, {"�T 24", "udp://@239.192.1.24:1234"}, {"�T 4 Sport", "udp://@239.192.1.25:1234"}};
					for (int i = 0; i < programs.GetLength(0); i++) {
						XmlNode node = objXmlDoc.SelectSingleNode("//nahravadlo/programy").AppendChild(objXmlDoc.CreateElement("program"));
						node.AppendChild(objXmlDoc.CreateElement("nazev")).AppendChild(objXmlDoc.CreateTextNode(programs[i, 0]));
						node.AppendChild(objXmlDoc.CreateElement("uri")).AppendChild(objXmlDoc.CreateTextNode(programs[i, 1]));
					}
				}

				if (!objXmlDoc.OuterXml.Contains("<?xml"))
					objXmlDoc.InsertBefore(objXmlDoc.CreateXmlDeclaration("1.0", "utf-8", null), objXmlDoc.DocumentElement);

				objXmlDoc.SelectSingleNode("//nahravadlo/config/vlc").InnerText = txtVLCPath.Text;
				objXmlDoc.SelectSingleNode("//nahravadlo/config/defaultdirectory").InnerText = txtDefaultDirectory.Text;
				objXmlDoc.SelectSingleNode("//nahravadlo/config/use_mpegts").InnerText = chkUseMPEGTS.Checked.ToString();

				objXmlDoc.SelectSingleNode("//nahravadlo/config/login/username").InnerText = txtUsername.Text;
				objXmlDoc.SelectSingleNode("//nahravadlo/config/login/password").InnerText = txtPassword.Text;

				objXmlDoc.Save(Application.StartupPath + @"\config.xml");
			} catch(Exception e) {
				MessageBox.Show("Nepovedlo se ulo�it nastaven� programu.\n\nSyst�m hl�s�: " + e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnBrowse_Click(object sender, EventArgs e) {
			folderBrowser.SelectedPath = txtDefaultDirectory.Text;
			folderBrowser.ShowDialog();
			txtDefaultDirectory.Text = folderBrowser.SelectedPath;
		}

		private void btnContainerHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show("MPEG TS (Transport Stream) se pou��v� pro p�en�en� MPEG z�znamu v prost�ed�, kde mohou vznikat chyby, nap�: DVB-T, streamovan� po s�ti, atd. Narozd�l od toho se MPEG PS (Programm Stream) vyu��v� v prost�ed�, kde opravu chyb dok�e zajistit jin� technologie, nap�. DVD, video na disku, atd.\n\nMPEG PS p�ehraje jak�koliv software pro sledovani videa. Pro p�ehr�n� MPEG TS kontejneru, je ji� pot�ebn� v�t�inou n�jak� plugin. Pro programy pou��vaj�c� DirectShow, jako nap��klad Windows Media Player, BSPlayer, MV2Player lze pou��t Haali Media Splitter. P�ehr�t� MPEG TS bez instalace pluginu um� t�eba VLC, nebo p�eportovan� MPlayer na Windows.\n\nPozn�mka: VLC 0.7.x a pravd�podobn� i ni���, m� probl�my s vytv��en�m MPEG PS kontejner� (projevuje se to, �e se nelze posouvat ve videu, p��padn� p�i posunut� p�ehr�va� spadne - p��klad Windows Media Playeru). Proto pro pou�it� MPEG PS kontejneru doporu�uji pou��t posledn� verzi VLC, kter� tyto probl�my nem�.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

	}
}