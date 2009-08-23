using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Nahravadlo
{
	public partial class FormSettings : Form
	{
		private readonly Settings _settings;
		public bool IsCanceled;

		public FormSettings()
		{
			InitializeComponent();

			_settings = Settings.GetInstance();

			LoadData();
			IsCanceled = false;
		}

		private void btnHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Pokud m� b�t nahr�v�n� spu�t�no na pozad�, tak aby nebylo vid�t okno VLC, pak je nutn� zadat jm�no a heslo existuj�c�ho u�ivatele ve Windows XP (Nejl�pe jin�ho u�ivatele, ne� pod kter�m jste aktu�ln� p�ihl�en. Heslo nesm� b�t pr�zdn�!). Pokud nevypln�te u�ivatele a heslo, VLC se spust� pod pr�v� p�ihl�en�m u�ivatelem a tento u�ivatel uvid� okno VLC v�etn� mo�nosti jeho ovl�d�n� (nahr�v�n� se neprovede, pokud u�ivatel nebude p�ihl�en!).\n\nPozn�mka: U�ivatelsk� jm�no je mo�n� zad�vat v�etn� dom�ny. U�ivatel, pod kter�m bude spu�t�no VLC mus� m�t mo�nost tuto aplikaci spustit a mus� m�t mo�nost p�istupovat i k prostoru na disku, kam se budou ukl�dat soubory.\n\nJe v�hodn� pomoc� ovl�dac�ch panel� zalo�it nov�ho u�ivatele, nejl�pe s pravomocmi spr�vce po��ta�e, nastavit mu heslo a n�sledn� tohoto u�ivatele vyplnit do t�to aplikace.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void btnSelectVLC_Click(object sender, EventArgs e)
		{
			openFile.FileName = txtVLCPath.Text;
			openFile.ShowDialog();
			txtVLCPath.Text = openFile.FileName;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			IsCanceled = true;
			Close();
		}

		private void btnSaveAndClose_Click(object sender, EventArgs e)
		{
			if (!File.Exists(txtVLCPath.Text) || txtVLCPath.Text.Trim().Length == 0 || !Directory.Exists(txtDefaultDirectory.Text) || txtDefaultDirectory.Text.Trim().Length == 0)
			{
				MessageBox.Show("Pros�m vypl�te spr�vn� n�sleduj�c� polo�ky:\n\nCesta k VLC - mus� obsahovat cestu k VLC v�etn� spustiteln�ho souboru.\n\nV�choz� adres�� - mus� obsahovat cestu k adres��i, kam se budou ukl�dat nahran� po�ady, pokud u nich nebude uvedena absolutn� cesta.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
				return;
			}
			SaveData();
			Close();
		}

		private void LoadData()
		{
			try
			{
				txtVLCPath.Text = _settings.GetString("nahravadlo/config/vlc", "");
				txtDefaultDirectory.Text = _settings.GetString("nahravadlo/config/defaultdirectory", @"C:\");
				chkUseMPEGTS.Checked = _settings.GetBool("nahravadlo/config/use_mpegts", false);

				txtUsername.Text = _settings.GetString("nahravadlo/config/login/username", "");
				txtPassword.Text = _settings.GetString("nahravadlo/config/login/password", "");

				numAddScheduleMinutes.Value = _settings.GetInt("nahravadlo/config/add_schedule_minutes", 0);

				txtFilenameMask.Text = _settings.GetString("nahravadlo/config/filename/mask", "%N.mpg");
				chkFilenameWithoutDiacritics.Checked = _settings.GetBool("nahravadlo/config/filename/without_diacritics", false);
				chkFilenameLowerCase.Checked = _settings.GetBool("nahravadlo/config/filename/lower_case", false);
				var spaceReplacement = _settings.GetInt("nahravadlo/config/filename/space_replacement", 0);

				if (spaceReplacement < 0 || spaceReplacement > 3)
					spaceReplacement = 0;
				cmbSpacesReplacement.SelectedIndex = spaceReplacement;

				var channels = new Channels(_settings).Get();

				if (channels.Length == 0)
					channels = new Channels(_settings).DefaultChannels;

				lstChannel.Items.Clear();
				foreach (var channel in channels)
					lstChannel.Items.Add(channel);

				CheckScheduleProtocolRegistration();
			}
			catch {}
		}

		private void SaveData()
		{
			try
			{
				_settings.SetString("nahravadlo/config/vlc", txtVLCPath.Text);
				_settings.SetString("nahravadlo/config/defaultdirectory", txtDefaultDirectory.Text);
				_settings.SetBool("nahravadlo/config/use_mpegts", chkUseMPEGTS.Checked);

				_settings.SetString("nahravadlo/config/login/username", txtUsername.Text);
				_settings.SetString("nahravadlo/config/login/password", txtPassword.Text);

				_settings.SetInt("nahravadlo/config/add_schedule_minutes", (int) numAddScheduleMinutes.Value);

				_settings.SetString("nahravadlo/config/filename/mask", txtFilenameMask.Text);
				_settings.SetBool("nahravadlo/config/filename/without_diacritics", chkFilenameWithoutDiacritics.Checked);
				_settings.SetBool("nahravadlo/config/filename/lower_case", chkFilenameLowerCase.Checked);
				_settings.SetInt("nahravadlo/config/filename/space_replacement", cmbSpacesReplacement.SelectedIndex);

				var channel = new Channel[lstChannel.Items.Count];
				lstChannel.Items.CopyTo(channel, 0);

				new Channels(_settings).Set(channel);

				_settings.Save();
			}
			catch (Exception e)
			{
				MessageBox.Show("Nepovedlo se ulo�it nastaven� programu.\n\nSyst�m hl�s�: " + e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			folderBrowser.SelectedPath = txtDefaultDirectory.Text;
			folderBrowser.ShowDialog();
			txtDefaultDirectory.Text = folderBrowser.SelectedPath;
		}

		private void btnContainerHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show("MPEG TS (Transport Stream) se pou��v� pro p�en�en� MPEG z�znamu v prost�ed�, kde mohou vznikat chyby, nap�: DVB-T, streamovan� po s�ti, atd. Narozd�l od toho se MPEG PS (Programm Stream) vyu��v� v prost�ed�, kde opravu chyb dok�e zajistit jin� technologie, nap�. DVD, video na disku, atd.\n\nMPEG PS p�ehraje jak�koliv software pro sledovani videa. Pro p�ehr�n� MPEG TS kontejneru, je ji� pot�ebn� v�t�inou n�jak� plugin. Pro programy pou��vaj�c� DirectShow, jako nap��klad Windows Media Player, BSPlayer, MV2Player lze pou��t Haali Media Splitter. P�ehr�t� MPEG TS bez instalace pluginu um� t�eba VLC, nebo p�eportovan� MPlayer na Windows.\n\nPozn�mka: VLC 0.7.x a pravd�podobn� i ni���, m� probl�my s vytv��en�m MPEG PS kontejner� (projevuje se to, �e se nelze posouvat ve videu, p��padn� p�i posunut� p�ehr�va� spadne - p��klad Windows Media Playeru). Proto pro pou�it� MPEG PS kontejneru doporu�uji pou��t posledn� verzi VLC, kter� tyto probl�my nem�.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void lstChannel_SelectedIndexChanged(object sender, EventArgs e)
		{
			var channel = (Channel) lstChannel.SelectedItem;

			if (channel == null)
			{
				//btnChannelSave.Enabled = false;}}
				btnChannelDelete.Enabled = false;

				btnChannelUp.Enabled = false;
				btnChannelDown.Enabled = false;

				txtChannelName.Text = "";
				txtChannelUri.Text = "";
				txtChannelId.Text = "";

				txtChannelName.Enabled = false;
				txtChannelUri.Enabled = false;
				txtChannelId.Enabled = false;
				return;
			}

			//btnChannelSave.Enabled = true;
			btnChannelDelete.Enabled = true;

			txtChannelName.Enabled = true;
			txtChannelUri.Enabled = true;
			txtChannelId.Enabled = true;

			btnChannelUp.Enabled = (lstChannel.SelectedIndex > 0);
			btnChannelDown.Enabled = (lstChannel.SelectedIndex < lstChannel.Items.Count - 1);

			txtChannelName.Text = channel.Name;
			txtChannelUri.Text = channel.Uri;
			txtChannelId.Text = channel.Id;
		}

		private void btnChannelDelete_Click(object sender, EventArgs e)
		{
			var selectedIndex = lstChannel.SelectedIndex;

			lstChannel.Items.Remove(lstChannel.SelectedItem);

			if (lstChannel.Items.Count <= 0)
				return;

			if (lstChannel.Items.Count > selectedIndex)
				lstChannel.SelectedIndex = selectedIndex;
			else if (lstChannel.Items.Count == selectedIndex)
				lstChannel.SelectedIndex = lstChannel.Items.Count - 1;
		}

	    private void btnChannelAdd_Click(object sender, EventArgs e)
		{
			lstChannel.SelectedIndex = lstChannel.Items.Add(new Channel("N�zev kan�lu", "udp://@", ""));
		}

		private void btnChannelUp_Click(object sender, EventArgs e)
		{
			var index = lstChannel.SelectedIndex;
			var temp = lstChannel.Items[index - 1];

			lstChannel.Items[index - 1] = lstChannel.Items[index];
			lstChannel.Items[index] = temp;

			lstChannel.SelectedIndex = index - 1;
		}

		private void btnChannelDown_Click(object sender, EventArgs e)
		{
			var index = lstChannel.SelectedIndex;
			var temp = lstChannel.Items[index + 1];

			lstChannel.Items[index + 1] = lstChannel.Items[index];
			lstChannel.Items[index] = temp;

			lstChannel.SelectedIndex = index + 1;
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			if (importFile.ShowDialog() != DialogResult.OK)
				return;

			var ch = new Channels(_settings);
			var channels = ch.LoadFromFile(importFile.FileName);
			ch.Set(channels);

			lstChannel.Items.Clear();
			foreach (var channel in channels)
				lstChannel.Items.Add(channel);
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			if (exportFile.ShowDialog() != DialogResult.OK)
				return;

			var channels = new Channels(_settings);
			channels.SaveToFile(exportFile.FileName, channels.Get());
		}

		private void btnRegisterScheduleProtocol_Click(object sender, EventArgs e)
		{
			try
			{
				//misto HKCR pouzijeme HKCU\Software\Classes, kam lze zapisovat i bez administratorskych prav, napr ve Windows Vista
				using (var scheduleKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\schedule"))
				{
					if (scheduleKey != null)
					{
						scheduleKey.SetValue(null, "URL:Schedule Protocol");

						scheduleKey.SetValue("URL Protocol", "");
						using (var subKey = scheduleKey.CreateSubKey("DefaultIcon"))
						{
                            if (subKey != null) subKey.SetValue(null, string.Format("{0},1", Process.GetCurrentProcess().MainModule.FileName));
						}
					    using (var subKey = scheduleKey.CreateSubKey("shell\\open\\command"))
					    {
					        if (subKey != null) subKey.SetValue(null, string.Format("\"{0}\" \"%1\"", Process.GetCurrentProcess().MainModule.FileName));
					    }
					}
				}
				MessageBox.Show(this, "Protokol schedule byl �sp�n� zaregistrov�n.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception)
			{
				MessageBox.Show(this, "Protokol schedule se nepovedlo zaregistrovat.\nPravd�podobn� nem�te pro tuto operaci dostate�n� pr�va.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			CheckScheduleProtocolRegistration();
		}

		private void btnScheduleHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Protokol schedule slou�� k p�ed�n� v�ech parametr� pro vytvo�en� nahr�v�n� \nz webov�ho prost�ed� probramu Nahr�vadlo. Tedy jedn�m kliknut�m lze p�edvyplnit pol��ka \npro zalo�en� nov�ho nahr�v�n�. V�ce informac� naleznete v n�pov�d� programu.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void btnUnregisterScheduleProtocol_Click(object sender, EventArgs e)
		{
			try
			{
				using (var key = Registry.CurrentUser.OpenSubKey("Software\\Classes", RegistryKeyPermissionCheck.ReadWriteSubTree))
				{
					if (key != null)
						key.DeleteSubKeyTree("schedule");
				}
				MessageBox.Show(this, "Protokol schedule byl �sp�n� odregistrov�n.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			CheckScheduleProtocolRegistration();
		}

		private void CheckScheduleProtocolRegistration()
		{
			try
			{
				using (var key = Registry.CurrentUser.OpenSubKey("Software\\Classes\\schedule"))
				{
					if (key != null)
					{
						btnUnregisterScheduleProtocol.Enabled = true;
						return;
					}
				}
			}
			catch {}
			btnUnregisterScheduleProtocol.Enabled = false;
		}

        private void txtChannelName_TextChanged(object sender, EventArgs e)
        {
            if (lstChannel.SelectedItem != null)
            {
                ((Channel) lstChannel.SelectedItem).Name = txtChannelName.Text;
                //obnoveni nazvu stanice v listboxu
                lstChannel.DisplayMember = "";
                lstChannel.DisplayMember = "Name";
            }
        }

        private void txtChannelUri_TextChanged(object sender, EventArgs e)
        {
            if (lstChannel.SelectedItem != null)
                ((Channel) lstChannel.SelectedItem).Uri = txtChannelUri.Text;
        }

        private void txtChannelId_TextChanged(object sender, EventArgs e)
        {
            if (lstChannel.SelectedItem != null)
                ((Channel) lstChannel.SelectedItem).Id = txtChannelId.Text;
        }
	}
}