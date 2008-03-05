using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Nahravadlo
{
	public partial class formSettings : Form
	{
		public bool isCanceled = false;
		private Settings settings;

		public formSettings()
		{
			InitializeComponent();

			settings = Settings.getInstance();

			LoadData();
			isCanceled = false;
		}

		private void btnHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show(
				"Pokud má být nahrávání spuštìno na pozadí, tak aby nebylo vidìt okno VLC, pak je nutné zadat jméno a heslo existujícího uživatele ve Windows XP (Nejlépe jiného uživatele, než pod kterým jste aktuálnì pøihlášen. Heslo nesmí být prázdné!). Pokud nevyplníte uživatele a heslo, VLC se spustí pod právì pøihlášeným uživatelem a tento uživatel uvidí okno VLC vèetnì možnosti jeho ovládání (nahrávání se neprovede, pokud uživatel nebude pøihlášen!).\n\nPoznámka: Uživatelské jméno je možné zadávat vèetnì domény. Uživatel, pod kterým bude spuštìno VLC musí mít možnost tuto aplikaci spustit a musí mít možnost pøistupovat i k prostoru na disku, kam se budou ukládat soubory.\n\nJe výhodné pomocí ovládacích panelù založit nového uživatele, nejlépe s pravomocmi správce poèítaèe, nastavit mu heslo a následnì tohoto uživatele vyplnit do této aplikace.",
				Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void btnSelectVLC_Click(object sender, EventArgs e)
		{
			openFile.FileName = txtVLCPath.Text;
			openFile.ShowDialog();
			txtVLCPath.Text = openFile.FileName;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			isCanceled = true;
			Close();
		}

		private void btnSaveAndClose_Click(object sender, EventArgs e)
		{
			if (!File.Exists(txtVLCPath.Text) || txtVLCPath.Text.Trim().Length == 0 ||
			    !Directory.Exists(txtDefaultDirectory.Text) || txtDefaultDirectory.Text.Trim().Length == 0)
			{
				MessageBox.Show(
					"Prosím vyplòte správnì následující položky:\n\nCesta k VLC - musí obsahovat cestu k VLC vèetnì spustitelného souboru.\n\nVýchozí adresáø - musí obsahovat cestu k adresáøi, kam se budou ukládat nahrané poøady, pokud u nich nebude uvedena absolutní cesta.",
					Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				txtVLCPath.Text = settings.getString("nahravadlo/config/vlc", "");
				txtUsername.Text = settings.getString("nahravadlo/config/login/username", "");
				txtPassword.Text = settings.getString("nahravadlo/config/login/password", "");

				txtDefaultDirectory.Text = settings.getString("nahravadlo/config/defaultdirectory", @"C:\");

				chkUseMPEGTS.Checked = settings.getBool("nahravadlo/config/use_mpegts", false);
				numAddScheduleMinutes.Value = settings.getInt("nahravadlo/config/add_schedule_minutes", 0);

				Channel[] channels = new Channels(settings).getChannels();

				if (channels.Length == 0) channels = new Channels(settings).getDefaultChannels();

				lstChannel.Items.Clear();
				foreach(Channel channel in channels)
					lstChannel.Items.Add(channel);
			} catch {}
		}

		private void SaveData()
		{
			try
			{
				settings.setString("nahravadlo/config/vlc", txtVLCPath.Text);
				settings.setString("nahravadlo/config/defaultdirectory", txtDefaultDirectory.Text);
				settings.setBool("nahravadlo/config/use_mpegts", chkUseMPEGTS.Checked);

				settings.setString("nahravadlo/config/login/username", txtUsername.Text);
				settings.setString("nahravadlo/config/login/password", txtPassword.Text);

				settings.setInt("nahravadlo/config/add_schedule_minutes", (int) numAddScheduleMinutes.Value);

				Channel[] channel = new Channel[lstChannel.Items.Count];
				lstChannel.Items.CopyTo(channel, 0);

				new Channels(settings).setChannels(channel);

				settings.Save();
			} catch(Exception e)
			{
				MessageBox.Show("Nepovedlo se uložit nastavení programu.\n\nSystém hlásí: " + e.Message, Text, MessageBoxButtons.OK,
				                MessageBoxIcon.Error);
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
			MessageBox.Show(
				"MPEG TS (Transport Stream) se používá pro pøenášení MPEG záznamu v prostøedí, kde mohou vznikat chyby, napø: DVB-T, streamovaní po síti, atd. Narozdíl od toho se MPEG PS (Programm Stream) využívá v prostøedí, kde opravu chyb dokáže zajistit jiná technologie, napø. DVD, video na disku, atd.\n\nMPEG PS pøehraje jakýkoliv software pro sledovani videa. Pro pøehrání MPEG TS kontejneru, je již potøebný vìtšinou nìjaký plugin. Pro programy používající DirectShow, jako napøíklad Windows Media Player, BSPlayer, MV2Player lze použít Haali Media Splitter. Pøehrátí MPEG TS bez instalace pluginu umí tøeba VLC, nebo pøeportovaný MPlayer na Windows.\n\nPoznámka: VLC 0.7.x a pravdìpodobnì i nišší, má problémy s vytváøením MPEG PS kontejnerù (projevuje se to, že se nelze posouvat ve videu, pøípadnì pøi posunutí pøehrávaè spadne - pøíklad Windows Media Playeru). Proto pro použití MPEG PS kontejneru doporuèuji použít poslední verzi VLC, která tyto problémy nemá.",
				Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void lstChannel_SelectedIndexChanged(object sender, EventArgs e)
		{
			Channel channel = (Channel) lstChannel.SelectedItem;

			if (channel == null)
			{
				btnChannelSave.Enabled = false;
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
			} else
			{
				btnChannelSave.Enabled = true;
				btnChannelDelete.Enabled = true;

				txtChannelName.Enabled = true;
				txtChannelUri.Enabled = true;
				txtChannelId.Enabled = true;
			}

			btnChannelUp.Enabled = (lstChannel.SelectedIndex > 0);
			btnChannelDown.Enabled = (lstChannel.SelectedIndex < lstChannel.Items.Count - 1);

			txtChannelName.Text = channel.getName();
			txtChannelUri.Text = channel.getUri();
			txtChannelId.Text = channel.getId();
		}

		private void btnChannelDelete_Click(object sender, EventArgs e)
		{
			int selectedIndex = lstChannel.SelectedIndex;

			lstChannel.Items.Remove(lstChannel.SelectedItem);

			if (lstChannel.Items.Count > 0)
			{
				if (lstChannel.Items.Count > selectedIndex)
					lstChannel.SelectedIndex = selectedIndex;
				else if (lstChannel.Items.Count == selectedIndex)
					lstChannel.SelectedIndex = lstChannel.Items.Count - 1;
			}
		}

		private void btnChannelSave_Click(object sender, EventArgs e)
		{
			lstChannel.Items[lstChannel.SelectedIndex] = new Channel(txtChannelName.Text, txtChannelUri.Text, txtChannelId.Text);
		}

		private void btnChannelAdd_Click(object sender, EventArgs e)
		{
			lstChannel.SelectedIndex = lstChannel.Items.Add(new Channel("Název kanálu", "udp://@", ""));
		}

		private void btnChannelUp_Click(object sender, EventArgs e)
		{
			int index = lstChannel.SelectedIndex;
			Object temp = lstChannel.Items[index - 1];

			lstChannel.Items[index - 1] = lstChannel.Items[index];
			lstChannel.Items[index] = temp;

			lstChannel.SelectedIndex = index - 1;
		}

		private void btnChannelDown_Click(object sender, EventArgs e)
		{
			int index = lstChannel.SelectedIndex;
			Object temp = lstChannel.Items[index + 1];

			lstChannel.Items[index + 1] = lstChannel.Items[index];
			lstChannel.Items[index] = temp;

			lstChannel.SelectedIndex = index + 1;
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			if (importFile.ShowDialog() == DialogResult.OK)
			{
				Channels ch = new Channels(settings);
				Channel[] channels = ch.loadChannelsFromFile(importFile.FileName);
				ch.setChannels(channels);

				lstChannel.Items.Clear();
				foreach(Channel channel in channels)
					lstChannel.Items.Add(channel);
			}
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			if (exportFile.ShowDialog() == DialogResult.OK)
			{
				Channels channels = new Channels(settings);
				channels.saveChannelsToFile(exportFile.FileName, channels.getChannels());
			}
		}

		private void btnRegisterScheduleProtocol_Click(object sender, EventArgs e)
		{
			try
			{
				//misto HKCR pouzijeme HKCU\Software\Classes, kam lze zapisovat i bez administratorskych prav, napr ve Windows Vista
				RegistryKey scheduleKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\schedule");
				scheduleKey.SetValue(null, "URL:Schedule Protocol");
				scheduleKey.SetValue("URL Protocol", "");
				scheduleKey.CreateSubKey("DefaultIcon");
				scheduleKey.CreateSubKey("shell\\open\\command").SetValue(null,
				                                                          string.Format("\"{0}\" \"%1\"",
				                                                                        Process.GetCurrentProcess().MainModule.
				                                                                        	FileName));
				MessageBox.Show(this, "Protokol schedule byl úspìšnì zaregistrován.", Text, MessageBoxButtons.OK,
				                MessageBoxIcon.Information);
			} catch(Exception)
			{
				MessageBox.Show(this,
				                "Protokol schedule se nepovedlo zaregistrovat.\nPravdìpodobnì nemáte pro tuto operaci dostateèná práva.",
				                Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void btnScheduleHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show(
				"Protokol schedule slouží k pøedání všech parametrù pro vytvoøení nahrávání \nz webového prostøedí probramu Nahrávadlo. Tedy jedním kliknutím lze pøedvyplnit políèka \npro založení nového nahrávání. Více informací naleznete v nápovìdì programu.",
				Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}