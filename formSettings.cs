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
			MessageBox.Show("Pokud má být nahrávání spuštìno na pozadí, tak aby nebylo vidìt okno VLC, pak je nutné zadat jméno a heslo existujícího uživatele ve Windows XP (Nejlépe jiného uživatele, než pod kterým jste aktuálnì pøihlášen. Heslo nesmí být prázdné!). Pokud nevyplníte uživatele a heslo, VLC se spustí pod právì pøihlášeným uživatelem a tento uživatel uvidí okno VLC vèetnì možnosti jeho ovládání (nahrávání se neprovede, pokud uživatel nebude pøihlášen!).\n\nPoznámka: Uživatelské jméno je možné zadávat vèetnì domény. Uživatel, pod kterým bude spuštìno VLC musí mít možnost tuto aplikaci spustit a musí mít možnost pøistupovat i k prostoru na disku, kam se budou ukládat soubory.\n\nJe výhodné pomocí ovládacích panelù založit nového uživatele, nejlépe s pravomocmi správce poèítaèe, nastavit mu heslo a následnì tohoto uživatele vyplnit do této aplikace.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
				MessageBox.Show("Prosím vyplòte správnì následující položky:\n\nCesta k VLC - musí obsahovat cestu k VLC vèetnì spustitelného souboru.\n\nVýchozí adresáø - musí obsahovat cestu k adresáøi, kam se budou ukládat nahrané poøady, pokud u nich nebude uvedena absolutní cesta.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show("Nepovedlo se uložit nastavení programu.\n\nSystém hlásí: " + e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			MessageBox.Show("MPEG TS (Transport Stream) se používá pro pøenášení MPEG záznamu v prostøedí, kde mohou vznikat chyby, napø: DVB-T, streamovaní po síti, atd. Narozdíl od toho se MPEG PS (Programm Stream) využívá v prostøedí, kde opravu chyb dokáže zajistit jiná technologie, napø. DVD, video na disku, atd.\n\nMPEG PS pøehraje jakýkoliv software pro sledovani videa. Pro pøehrání MPEG TS kontejneru, je již potøebný vìtšinou nìjaký plugin. Pro programy používající DirectShow, jako napøíklad Windows Media Player, BSPlayer, MV2Player lze použít Haali Media Splitter. Pøehrátí MPEG TS bez instalace pluginu umí tøeba VLC, nebo pøeportovaný MPlayer na Windows.\n\nPoznámka: VLC 0.7.x a pravdìpodobnì i nišší, má problémy s vytváøením MPEG PS kontejnerù (projevuje se to, že se nelze posouvat ve videu, pøípadnì pøi posunutí pøehrávaè spadne - pøíklad Windows Media Playeru). Proto pro použití MPEG PS kontejneru doporuèuji použít poslední verzi VLC, která tyto problémy nemá.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
			lstChannel.SelectedIndex = lstChannel.Items.Add(new Channel("Název kanálu", "udp://@", ""));
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
				MessageBox.Show(this, "Protokol schedule byl úspìšnì zaregistrován.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception)
			{
				MessageBox.Show(this, "Protokol schedule se nepovedlo zaregistrovat.\nPravdìpodobnì nemáte pro tuto operaci dostateèná práva.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			CheckScheduleProtocolRegistration();
		}

		private void btnScheduleHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Protokol schedule slouží k pøedání všech parametrù pro vytvoøení nahrávání \nz webového prostøedí probramu Nahrávadlo. Tedy jedním kliknutím lze pøedvyplnit políèka \npro založení nového nahrávání. Více informací naleznete v nápovìdì programu.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
				MessageBox.Show(this, "Protokol schedule byl úspìšnì odregistrován.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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