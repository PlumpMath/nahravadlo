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
			MessageBox.Show("Pokud má být nahrávání spuštìno na pozadí, tak aby nebylo vidìt okno VLC, pak je nutné zadat jméno a heslo existujícího uživatele ve Windows XP (Nejlépe jiného uživatele, než pod kterým jste aktuálnì pøihlášen. Heslo nesmí být prázdné!). Pokud nevyplníte uživatele a heslo, VLC se spustí pod právì pøihlášeným uživatelem a tento uživatel uvidí okno VLC vèetnì možnosti jeho ovládání (nahrávání se neprovede, pokud uživatel nebude pøihlášen!).\n\nPoznámka: Uživatelské jméno je možné zadávat vèetnì domény. Uživatel, pod kterým bude spuštìno VLC musí mít možnost tuto aplikaci spustit a musí mít možnost pøistupovat i k prostoru na disku, kam se budou ukládat soubory.\n\nJe výhodné pomocí ovládacích panelù založit nového uživatele, nejlépe s pravomocmi správce poèítaèe, nastavit mu heslo a následnì tohoto uživatele vyplnit do této aplikace.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
				MessageBox.Show("Prosím vyplòte správnì následující položky:\n\nCesta k VLC - musí obsahovat cestu k VLC vèetnì spustitelného souboru.\n\nVýchozí adresáø - musí obsahovat cestu k adresáøi, kam se budou ukládat nahrané poøady, pokud u nich nebude uvedena absolutní cesta.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
					String[,] programs = {{"ÈT 1", "udp://@239.192.1.20:1234"}, {"ÈT 2", "udp://@239.192.1.21:1234"}, {"Nova", "udp://@239.192.1.22:1234"}, {"Prima", "udp://@239.192.1.23:1234"}, {"ÈT 24", "udp://@239.192.1.24:1234"}, {"ÈT 4 Sport", "udp://@239.192.1.25:1234"}};
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
				MessageBox.Show("Nepovedlo se uložit nastavení programu.\n\nSystém hlásí: " + e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnBrowse_Click(object sender, EventArgs e) {
			folderBrowser.SelectedPath = txtDefaultDirectory.Text;
			folderBrowser.ShowDialog();
			txtDefaultDirectory.Text = folderBrowser.SelectedPath;
		}

		private void btnContainerHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show("MPEG TS (Transport Stream) se používá pro pøenášení MPEG záznamu v prostøedí, kde mohou vznikat chyby, napø: DVB-T, streamovaní po síti, atd. Narozdíl od toho se MPEG PS (Programm Stream) využívá v prostøedí, kde opravu chyb dokáže zajistit jiná technologie, napø. DVD, video na disku, atd.\n\nMPEG PS pøehraje jakýkoliv software pro sledovani videa. Pro pøehrání MPEG TS kontejneru, je již potøebný vìtšinou nìjaký plugin. Pro programy používající DirectShow, jako napøíklad Windows Media Player, BSPlayer, MV2Player lze použít Haali Media Splitter. Pøehrátí MPEG TS bez instalace pluginu umí tøeba VLC, nebo pøeportovaný MPlayer na Windows.\n\nPoznámka: VLC 0.7.x a pravdìpodobnì i nišší, má problémy s vytváøením MPEG PS kontejnerù (projevuje se to, že se nelze posouvat ve videu, pøípadnì pøi posunutí pøehrávaè spadne - pøíklad Windows Media Playeru). Proto pro použití MPEG PS kontejneru doporuèuji použít poslední verzi VLC, která tyto problémy nemá.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

	}
}