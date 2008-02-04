using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Windows.Forms;

namespace Nahravadlo
{
	public partial class formMain : Form
	{
		public static string vlc;
		public static string defaultDirectory;
		private string username;
		private string password;
		public static bool useMpegTS = false;
		public static ComboBox comboChannels;

		public static Settings SETTING;

		private bool forceClose = false;

		public static Schedules SCHEDULES;

		public formMain()
		{
			InitializeComponent();

			Application.EnableVisualStyles();

			String[] ver = Application.ProductVersion.Split('.');
			Text = String.Format("Nahrávadlo {0}.{1}.{2} by Arcao", ver[0], ver[1], ver[2]);

			LoadConfig();
			comboChannels = cmbProgram;

			SCHEDULES = new Schedules(vlc, defaultDirectory);
		}

		public formMain(String url) : this()
		{
			//pokud se zavrel nastavovaci dialog bez ulozeni, ukoncime funkci
			if (forceClose) return;

			if (url == null) return;
			Uri uri = new Uri(url);
			String channelId = uri.Host;
			String programmName = Uri.UnescapeDataString(uri.AbsolutePath).Substring(1);
			NameValueCollection qItems = HttpUtility.ParseQueryString(uri.Query);

			formQuickAdd f =
				new formQuickAdd(channelId, programmName, Utils.ParseISO8601DateTime(qItems["start"]),
				                 Utils.ParseISO8601DateTime(qItems["stop"]), 0);
			f.Text = Text + " - Rychlé nahrávání";
			DialogResult res = f.ShowDialog(this);

			if (Equals(res, DialogResult.Abort) || Equals(res, DialogResult.OK))
				forceClose = true;
		}

		private void Form1_Load(object sender, EventArgs e)
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

			foreach(string name in SCHEDULES.getAllNames())
				lst.Items.Add(name);
		}

		private void lst_SelectedIndexChanged(object sender, EventArgs e)
		{
			string itemName = (string) lst.SelectedItem;
			try
			{
				Job job = SCHEDULES.get(itemName);
				txtName.Text = itemName;

				dteBegin.Value = job.Start;
				numLength.Value = job.Length;

				cmbProgram.SelectedIndex = getChannelIndexFromUri(job.Uri);
				txtFilename.Text = job.Filename;

				txtStatus.Text = job.StatusText;
				btnStopRecording.Enabled = (job.Status == JobStatus.Running);
			} catch
			{
				lst.Items.Remove(itemName);
			}
		}

		public int getChannelIndexFromUri(string uri)
		{
			foreach(Object item in cmbProgram.Items)
			{
				if (((Channel) item).getUri().CompareTo(uri) == 0)
					return cmbProgram.FindString(item.ToString());
			}
			return -1;
		}

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			try
			{
				using(Job job = SCHEDULES.create(txtName.Text))
				{
					job.Start = dteBegin.Value;
					job.Uri = ((Channel) cmbProgram.SelectedItem).getUri();
					job.Filename = txtFilename.Text;

					job.UseMPEGTS = useMpegTS;

					job.Length = (int) numLength.Value;
					job.SetUsernameAndPassword(username, password);
				}

				lst.SelectedIndex = lst.Items.Add(txtName.Text);

				if (SCHEDULES.exist(txtName.Text))
					cmdSave.Enabled = false;
				else
					cmdSave.Enabled = true;

				cmdDelete.Enabled = cmdSave.Enabled;

				if (txtName.Text.Length == 0 || txtFilename.Text.Length == 0 || SCHEDULES.exist(txtName.Text))
					cmdAdd.Enabled = false;
				else
					cmdAdd.Enabled = true;
			} catch
			{
				MessageBox.Show(
					"Nepovedlo se pøidat nahrávání.\n\nUjistìte se, že název nahrávání neobsahuje následující znaky:\n/ \\ : * ? \" < > |",
					Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void LoadConfig()
		{
			//zjistime cestu k profilu s nastavenim
			string appSettingsPath =
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nahravadlo");
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
				} catch {}

				//pokud i presto soubor s nastavenim neexistuje, vyhodime vyjjimku
				if (!File.Exists(Settings.default_filename))
				{
					throw new Exception(
						"Nepovedlo se naèíst soubor config.xml.\n\nSoubor config.xml nebyl nalezen. Pravdìpodobnì se jedná o první spuštìní tohoto programu, proto bude zobrazen dialog pro nastavení tohoto programu.");
				}

				SETTING = Settings.getInstance();

				vlc = SETTING.getString("nahravadlo/config/vlc", "");
				if (vlc.Length == 0)
				{
					throw new Exception(
						"Chyba v soubor config.xml.\n\nNení nastavena cesta k exe souboru programu VLC.\n\nPøeètìtet si prosím, jak nakonfigurovat program v souboru readme.txt.");
				}

				if (!File.Exists(vlc))
				{
					throw new Exception(
						string.Format(
							"Chyba v soubor config.xml.\n\nCesta k VLC \"{0}\" neexistuje, nebo je adresáø (musí být soubor).\n\nPøeètìtet si prosím, jak nakonfigurovat program v souboru readme.txt.",
							vlc));
				}

				username = SETTING.getString("nahravadlo/config/login/username", "");
				password = SETTING.getString("nahravadlo/config/login/password", "");
				defaultDirectory = SETTING.getString("nahravadlo/config/defaultdirectory", @"C:\");

				useMpegTS = SETTING.getBool("nahravadlo/config/use_mpegts", false);

				Channel[] channels = new Channels(SETTING).getChannels();

				cmbProgram.Items.Clear();
				foreach(Channel channel in channels) cmbProgram.Items.Add(channel);

				if (cmbProgram.Items.Count > 0) cmbProgram.SelectedIndex = 0;
			} catch(Exception ex)
			{
				//zobrazime zpravu o chybe, pravdepodobne problem s nastavenim
				MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

				// a zobrazime dialog s nastavenim
				formSettings f = new formSettings();
				f.Text = "Nastavení programu " + Text;
				if (Equals(f.ShowDialog(this), DialogResult.OK))
					LoadConfig();
				else
					forceClose = true;
			}
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			txtFilename.Text = txtName.Text + ".mpg";

			if (!SCHEDULES.exist(txtName.Text))
				cmdSave.Enabled = false;
			else
				cmdSave.Enabled = true;

			cmdDelete.Enabled = cmdSave.Enabled;

			if (cmbProgram.SelectedIndex < 0 || txtName.Text.Length == 0 || txtFilename.Text.Length == 0 ||
			    SCHEDULES.exist(txtName.Text))
				cmdAdd.Enabled = false;
			else
				cmdAdd.Enabled = true;
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
			string itemName = (string) lst.SelectedItem;
			try
			{
				using(Job job = SCHEDULES.get(itemName))
				{
					job.Start = dteBegin.Value;
					job.Length = (int) numLength.Value;
					job.UseMPEGTS = useMpegTS;
					job.Uri = ((Channel) cmbProgram.SelectedItem).getUri();
					job.Filename = txtFilename.Text;

					job.SetUsernameAndPassword(username, password);
				}
			} catch
			{
				lst.Items.Remove(itemName);
			} finally
			{
				if (lst.Items.Count > 0) lst.SelectedIndex = 0;
			}
		}

		private void cmdDelete_Click(object sender, EventArgs e)
		{
			string itemName = (string) lst.SelectedItem;

			int selectedIndex = lst.SelectedIndex;

			try
			{
				using(Job job = SCHEDULES.get(itemName))
				{
					if (job.Status == JobStatus.Running)
						job.Terminate();
				}
			} catch {}

			try
			{
				SCHEDULES.remove(itemName);
			} catch {}

			lst.Items.Remove(itemName);

			txtName.Text = "";
			cmbProgram.SelectedIndex = 0;
			dteBegin.Value = DateTime.Now;
			dteEnd.Value = dteBegin.Value.AddMinutes(1);
			numLength.Value = 1;
			txtFilename.Text = "";
			txtStatus.Text = "Nahrávání nebylo ještì založeno.";

			if (lst.Items.Count > 0)
			{
				if (lst.Items.Count > selectedIndex)
					lst.SelectedIndex = selectedIndex;
				else if (lst.Items.Count == selectedIndex)
					lst.SelectedIndex = lst.Items.Count - 1;
			}
		}

		private void optionMenuItem_Click(object sender, EventArgs e)
		{
			formSettings f = new formSettings();
			f.Text = "Nastavení programu " + Text;
			f.ShowDialog(this);
			if (f.DialogResult == DialogResult.OK)
				LoadConfig();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			string[] names = SCHEDULES.getAllNames().ToArray();

			foreach(string name in names)
			{
				if (lst.FindStringExact(name) == -1)
					lst.Items.Add(name);
			}

			if (lst.Items.Count > 0)
			{
				string[] items = new string[lst.Items.Count];
				lst.Items.CopyTo(items, 0);

				foreach(string item in items)
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
					} else
					{
						if (Equals(lst.SelectedItem, item))
						{
							using(Job job = SCHEDULES.get(item))
							{
								txtStatus.Text = job.StatusText;
								btnStopRecording.Enabled = (job.Status == JobStatus.Running);
							}
						}
					}

					if (Equals(lst.SelectedItem, item) && !SCHEDULES.exist(item))
						lst.Items.Remove(item);
				}
			}
		}

		private void RecordNowMenuItem_Click(object sender, EventArgs e)
		{
			formRecordNow f;
			f = new formRecordNow();
			f.ShowDialog(this);
		}

		private void dteEnd_Validating(object sender, CancelEventArgs e)
		{
			if (dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes <= 0)
			{
				//e.Cancel = true;
				Utils.ShowBubble(dteEnd, ToolTipIcon.Error, "Chyba v datumu!", "Datum konce poøadu je nastaven pøed datum zaèátku!");

				DateTime val = dteBegin.Value;
				val = val.AddMinutes(1);
				dteEnd.Value = val;
			}
		}

		private void dteEnd_ValueChanged(object sender, EventArgs e)
		{
			if (dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes <= 0)
				return;

			numLength.Value = (int) Decimal.Round((decimal) dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes);
		}

		private void numLength_ValueChanged(object sender, EventArgs e)
		{
			DateTime val = dteBegin.Value;
			val = val.AddMinutes((double) numLength.Value);
			dteEnd.Value = val;
		}

		private void dteBegin_ValueChanged(object sender, EventArgs e)
		{
			DateTime val = dteBegin.Value;
			val = val.AddMinutes((double) numLength.Value);
			dteEnd.Value = val;
		}

		private void aboutMenuItem_Click(object sender, EventArgs e)
		{
			string content =
				"Nahrávadlo {0}.{1}.{2}\n----------------------------------\nNaprogramoval: Arcao\n\nhttp://nahravadlo.arcao.com";
			String[] ver = Application.ProductVersion.Split('.');
			MessageBox.Show(String.Format(content, ver[0], ver[1], ver[2]), Text, MessageBoxButtons.OK,
			                MessageBoxIcon.Information);
		}

		private void btnStopRecording_Click(object sender, EventArgs e)
		{
			try
			{
				using(Job job = SCHEDULES.get((string) lst.SelectedItem))
				{
					job.Terminate();
				}
			} catch(Exception) {}
		}
	}

	//public class ListContainer
	//{
	//    public string name;
	//    public string key;

	//    public ListContainer(string name, string key)
	//    {
	//        this.name = name;
	//        this.key = key;
	//    }

	//    public override string ToString()
	//    {
	//        return name;
	//    }
	//}
}