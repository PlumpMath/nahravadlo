using System;
using System.ComponentModel;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Windows.Forms;
using Nahravadlo.Schedule;

namespace Nahravadlo
{
	public partial class formMain : Form
	{
		public static ComboBox comboChannels;
		public static string defaultDirectory;
		public static Schedules SCHEDULES;
		public static bool useMpegTS;
		public static string vlc;

		private bool filenameLowerCase;
		private string filenameMask = "%N.mpg";
		private int filenameSpaceReplacement;
		private bool filenameWithoutDiacritics;

		private bool forceClose;
		private string password;
		private Settings setting;
		private string username;

		public formMain()
		{
			InitializeComponent();

			var ver = Application.ProductVersion.Split('.');
			Text = String.Format("Nahrávadlo {0}.{1}.{2} by Arcao", ver[0], ver[1], ver[2]);

			LoadConfig();
			comboChannels = cmbProgram;

			TestUACElevation(true);

			//if (UAC.IsVistaOrHigher() && !String.IsNullOrEmpty(username)

			SCHEDULES = new Schedules(vlc, defaultDirectory);
		}

		public formMain(String[] args) : this()
		{
			try
			{
				var url = args[0];
				//pokud se zavrel nastavovaci dialog bez ulozeni, ukoncime funkci
				if (forceClose)
					return;

				if (url == null)
					return;
				var uri = new Uri(url);
				var channelId = uri.Host;
				var programmName = Uri.UnescapeDataString(uri.AbsolutePath).Substring(1);
				var qItems = HttpUtility.ParseQueryString(uri.Query);

				var f = new formQuickAdd(channelId, programmName, Utils.ParseISO8601DateTime(qItems["start"]), Utils.ParseISO8601DateTime(qItems["stop"])) {Text = string.Format("{0} - Rychlé nahrávání", Text)};
				var res = f.ShowDialog(this);

				if (Equals(res, DialogResult.Abort) || Equals(res, DialogResult.OK))
					forceClose = true; {}
			}
			catch {}
		}

		private void TestUACElevation(bool passArgs)
		{
			if (!UAC.IsVistaOrHigher())
				return;
			if (UAC.IsAdmin())
				return;

			if (String.IsNullOrEmpty(username))
				return;

			try
			{
				using (var current = WindowsIdentity.GetCurrent())
				{
					var account = new NTAccount(username);
					var sid = account.Translate(typeof (SecurityIdentifier));

					if (!current.User.Equals(sid))
					{
#if (!DEBUG)
						UAC.RestartElevated(passArgs);
						forceClose = true;
#endif
					}
				}
			}
			catch (IdentityNotMappedException) {}
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			//pokud potrebujeme nahle ukoncit, ukoncime
			if (forceClose)
			{
				Close();
				return;
			}

			RefreshList();
			dteBegin.Value = DateTime.Now;
			dteEnd.Value = dteBegin.Value.AddMinutes(1);
			timer.Enabled = true;
		}

		public void RefreshList()
		{
			lst.Items.Clear();

			foreach (var name in SCHEDULES.GetAllNames())
				lst.Items.Add(name);
		}

		private void lst_SelectedIndexChanged(object sender, EventArgs e)
		{
			var itemName = (string) lst.SelectedItem;
			if (String.IsNullOrEmpty(itemName))
				return;
			try
			{
				var job = SCHEDULES.Get(itemName);
				txtName.Text = itemName;

				if (job.Start != DateTime.MinValue)
					dteBegin.Value = job.Start;
				numLength.Value = job.Length < 1 ? 1 : job.Length;

				cmbProgram.SelectedIndex = getChannelIndexFromUri(job.Uri);
				txtFilename.Text = job.Filename;

				txtStatus.Text = job.StatusText;
				btnStopRecording.Enabled = (job.Status == JobStatus.Running);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				lst.Items.Remove(itemName);
			}
		}

		public int getChannelIndexFromUri(string uri)
		{
			foreach (var item in cmbProgram.Items)
			{
				if (((Channel) item).Uri.CompareTo(uri) == 0)
					return cmbProgram.FindString(item.ToString());
			}
			return -1;
		}

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			try
			{
				using (var job = SCHEDULES.Create(txtName.Text))
				{
					job.Start = dteBegin.Value;
					job.Uri = ((Channel) cmbProgram.SelectedItem).Uri;
					job.Filename = txtFilename.Text;

					job.UseMPEGTS = useMpegTS;

					job.Length = (int) numLength.Value;
					job.Save(username, password);
				}

				lst.SelectedIndex = lst.Items.Add(txtName.Text);

				cmdSave.Enabled = SCHEDULES.Exist(txtName.Text);

				cmdDelete.Enabled = cmdSave.Enabled;

				if (txtName.Text.Length == 0 || txtFilename.Text.Length == 0 || SCHEDULES.Exist(txtName.Text))
					cmdAdd.Enabled = false;
				else
					cmdAdd.Enabled = true;
			}
			catch (Exception ex)
			{
                MessageBox.Show(ex.ToString());
				MessageBox.Show("Nepovedlo se pøidat nahrávání.\n\nUjistìte se, že název nahrávání neobsahuje následující znaky:\n/ \\ : * ? \" < > |", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void LoadConfig()
		{
			//zjistime cestu k profilu s nastavenim
			var appSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nahravadlo");
			//vytvorime adresar
			Directory.CreateDirectory(appSettingsPath);

			try
			{
				//nastavime cestu k souboru s nastavenim
				Settings.default_filename = Path.Combine(appSettingsPath, "config.xml");

				try
				{
					// pokud mame soubor config.xml u aplikace a neexistuje tento soubor v profilu s natavenim, zkopirujeme ho
					// a pokusime se ho smazat u aplikace
					if (!File.Exists(Settings.default_filename) && File.Exists(Path.Combine(Application.StartupPath, "config.xml")))
					{
						File.Copy(Path.Combine(Application.StartupPath, "config.xml"), Settings.default_filename);
						File.Delete(Path.Combine(Application.StartupPath, "config.xml"));
					}
				}
				catch {}

				//pokud i presto soubor s nastavenim neexistuje, vyhodime vyjjimku
				if (!File.Exists(Settings.default_filename))
					throw new Exception("Nepovedlo se naèíst soubor config.xml.\n\nSoubor config.xml nebyl nalezen. Pravdìpodobnì se jedná o první spuštìní tohoto programu, proto bude zobrazen dialog pro nastavení tohoto programu.");

				setting = Settings.getInstance();

				vlc = setting.getString("nahravadlo/config/vlc", "");
				if (vlc.Length == 0)
					throw new Exception("Chyba v soubor config.xml.\n\nNení nastavena cesta k exe souboru programu VLC.\n\nPøeètìtet si prosím, jak nakonfigurovat program v souboru readme.txt.");

				if (!File.Exists(vlc))
					throw new Exception(string.Format("Chyba v soubor config.xml.\n\nCesta k VLC \"{0}\" neexistuje, nebo je adresáø (musí být soubor).\n\nPøeètìtet si prosím, jak nakonfigurovat program v souboru readme.txt.", vlc));

				username = setting.getString("nahravadlo/config/login/username", "");
				password = setting.getString("nahravadlo/config/login/password", "");
				defaultDirectory = setting.getString("nahravadlo/config/defaultdirectory", @"C:\");

				useMpegTS = setting.getBool("nahravadlo/config/use_mpegts", false);

				filenameMask = setting.getString("nahravadlo/config/filename/mask", "%N.mpg");
				filenameWithoutDiacritics = setting.getBool("nahravadlo/config/filename/without_diacritics", false);
				filenameLowerCase = setting.getBool("nahravadlo/config/filename/lower_case", false);
				filenameSpaceReplacement = setting.getInt("nahravadlo/config/filename/space_replacement", 0);
				if (filenameSpaceReplacement < 0 || filenameSpaceReplacement > 3)
					filenameSpaceReplacement = 0;

				var channels = new Channels(setting).Get();

				cmbProgram.Items.Clear();
				foreach (var channel in channels)
					cmbProgram.Items.Add(channel);

				if (cmbProgram.Items.Count > 0)
					cmbProgram.SelectedIndex = 0;

				ReformatFilename();
			}
			catch (Exception ex)
			{
				//zobrazime zpravu o chybe, pravdepodobne problem s nastavenim
				MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

				// a zobrazime dialog s nastavenim
				var f = new formSettings {Text = ("Nastavení programu " + Text)};
				if (Equals(f.ShowDialog(this), DialogResult.OK))
					LoadConfig();
				else
					forceClose = true;
			}
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			ReformatFilename();

			cmdSave.Enabled = SCHEDULES.Exist(txtName.Text);

			cmdDelete.Enabled = cmdSave.Enabled;

			if (cmbProgram.SelectedIndex < 0 || txtName.Text.Length == 0 || txtFilename.Text.Length == 0 || SCHEDULES.Exist(txtName.Text))
				cmdAdd.Enabled = false;
			else
				cmdAdd.Enabled = true;
		}

		private void ReformatFilename()
		{
			var tmp = filenameMask;

			tmp = tmp.Replace("%%", Char.ConvertFromUtf32(0));

			tmp = tmp.Replace("%N", txtName.Text.Trim());
			tmp = tmp.Replace("%S", cmbProgram.Text.Trim());

			tmp = tmp.Replace("%H", dteBegin.Value.Hour.ToString("00"));
			tmp = tmp.Replace("%i", dteBegin.Value.Minute.ToString("00"));

			tmp = tmp.Replace("%D", dteBegin.Value.Day.ToString("00"));
			tmp = tmp.Replace("%M", dteBegin.Value.Month.ToString("00"));
			tmp = tmp.Replace("%Y", dteBegin.Value.Year.ToString("0000"));
			tmp = tmp.Replace("%y", (dteBegin.Value.Year%100).ToString("00"));

			tmp = tmp.Replace("%L", Decimal.Round((decimal) dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes).ToString());

			if (filenameWithoutDiacritics)
				tmp = Utils.RemoveDiacritics(tmp);
			if (filenameLowerCase)
				tmp = tmp.ToLower();

			switch (filenameSpaceReplacement)
			{
				case 1:
					tmp = tmp.Replace(' ', '_');
					break;
				case 2:
					tmp = tmp.Replace(' ', '-');
					break;
				case 3:
					tmp = tmp.Replace(" ", "");
					break;
				default:
					break;
			}

			tmp = tmp.Replace(Char.ConvertFromUtf32(0), "%");

			tmp = Utils.CorrectFilename(tmp);

			txtFilename.Text = tmp;
		}

		private void cmdBrowse_Click(object sender, EventArgs e)
		{
			dialog.InitialDirectory = defaultDirectory;
			dialog.FileName = txtFilename.Text;
			dialog.OverwritePrompt = true;
			dialog.Filter = "MPEG 2 soubor (*.mpg)|*.mpg|VLC soubor (*.vlc)|*.vlc";
			dialog.ValidateNames = true;
			if (dialog.ShowDialog() == DialogResult.OK)
				txtFilename.Text = dialog.FileName;
		}

		private void cmdSave_Click(object sender, EventArgs e)
		{
			var itemName = (string) lst.SelectedItem;
			try
			{
				using (var job = SCHEDULES.Get(itemName))
				{
					job.Start = dteBegin.Value;
					job.Length = (int) numLength.Value;
					job.UseMPEGTS = useMpegTS;
					job.Uri = ((Channel) cmbProgram.SelectedItem).Uri;
					job.Filename = txtFilename.Text;

					job.Save(username, password);
				}
			}
			catch
			{
				lst.Items.Remove(itemName);
			}
		}

		private void cmdDelete_Click(object sender, EventArgs e)
		{
			var itemName = (string) lst.SelectedItem;

			var selectedIndex = lst.SelectedIndex;

			try
			{
				using (var job = SCHEDULES.Get(itemName))
				{
					if (job.Status == JobStatus.Running)
						job.Terminate();
				}
			}
			catch {}

			try
			{
				SCHEDULES.Remove(itemName);
			}
			catch {}

			lst.Items.Remove(itemName);

			txtName.Text = "";
			cmbProgram.SelectedIndex = 0;
			dteBegin.Value = DateTime.Now;
			dteEnd.Value = dteBegin.Value.AddMinutes(1);
			numLength.Value = 1;
			txtFilename.Text = "";
			txtStatus.Text = "Nahrávání nebylo ještì založeno.";

			ReformatFilename();

			if (lst.Items.Count <= 0)
				return;

			if (lst.Items.Count > selectedIndex)
				lst.SelectedIndex = selectedIndex;
			else if (lst.Items.Count == selectedIndex)
				lst.SelectedIndex = lst.Items.Count - 1;
		}

		private void optionMenuItem_Click(object sender, EventArgs e)
		{
			var f = new formSettings {Text = ("Nastavení programu " + Text)};
			f.ShowDialog(this);
			if (f.DialogResult == DialogResult.OK)
				LoadConfig();

			TestUACElevation(false);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			var names = SCHEDULES.GetAllNames().ToArray();

			foreach (var name in names)
			{
				if (lst.FindStringExact(name) == -1)
					lst.Items.Add(name);
			}

			if (lst.Items.Count <= 0)
				return;

			var items = new string[lst.Items.Count];
			lst.Items.CopyTo(items, 0);

			foreach (var item in items)
			{
				if (Array.IndexOf(names, item) == -1)
				{
					if (Equals(lst.SelectedItem, item))
					{
						txtName.Text = "";
						cmbProgram.SelectedIndex = 0;
						dteBegin.Value = DateTime.Now;
						dteEnd.Value = dteBegin.Value.AddMinutes(1);
						numLength.Value = 1;
						txtFilename.Text = "";
						txtStatus.Text = "Nahrávání nebylo ještì založeno.";
						btnStopRecording.Enabled = false;
					}
					lst.Items.Remove(item);
				}
				else
				{
					if (Equals(lst.SelectedItem, item))
					{
						using (var job = SCHEDULES.Get(item))
						{
							txtStatus.Text = job.StatusText;
							btnStopRecording.Enabled = (job.Status == JobStatus.Running);
						}
					}
				}

				if (Equals(lst.SelectedItem, item) && !SCHEDULES.Exist(item))
					lst.Items.Remove(item);
			}
		}

		private void RecordNowMenuItem_Click(object sender, EventArgs e)
		{
			var f = new formRecordNow();
			f.ShowDialog(this);
		}

		private void dteEnd_Validating(object sender, CancelEventArgs e)
		{
			if (dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes > 0)
				return;

			//e.Cancel = true;
			Utils.ShowBubble(dteEnd, ToolTipIcon.Error, "Chyba v datumu!", "Datum konce poøadu je nastaven pøed datum zaèátku!");

			var val = dteBegin.Value;
			val = val.AddMinutes(1);
			dteEnd.Value = val;
		}

		private void dteEnd_ValueChanged(object sender, EventArgs e)
		{
			if (dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes <= 0)
				return;

			numLength.Value = (int) Decimal.Round((decimal) dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes);
			ReformatFilename();
		}

		private void numLength_ValueChanged(object sender, EventArgs e)
		{
			var val = dteBegin.Value;
			val = val.AddMinutes((double) numLength.Value);
			dteEnd.Value = val;
			ReformatFilename();
		}

		private void dteBegin_ValueChanged(object sender, EventArgs e)
		{
			var val = dteBegin.Value;
			val = val.AddMinutes((double) numLength.Value);
			dteEnd.Value = val;
			ReformatFilename();
		}

		private void aboutMenuItem_Click(object sender, EventArgs e)
		{
			const string content = "Nahrávadlo {0}.{1}.{2}\n----------------------------------\nNaprogramoval: Arcao\n\nhttp://nahravadlo.arcao.com";
			var ver = Application.ProductVersion.Split('.');
			MessageBox.Show(String.Format(content, ver[0], ver[1], ver[2]), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void btnStopRecording_Click(object sender, EventArgs e)
		{
			try
			{
				using (var job = SCHEDULES.Get((string) lst.SelectedItem))
					job.Terminate();
			}
			catch {}
		}
	}
}