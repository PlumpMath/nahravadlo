using System;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using TaskScheduler;

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

		public formMain()
		{
			InitializeComponent();
			Application.EnableVisualStyles();
			comboChannels = cmbProgram;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			String[] ver = Application.ProductVersion.Split('.');

			Text = String.Format("Nahrávadlo {0}.{1}.{2} by Arcao", ver[0], ver[1], ver[2]);
			LoadConfig();
			RefreshList();
			dteBegin.Value = DateTime.Now;
			dteEnd.Value = dteBegin.Value.AddMinutes(1);
			timer.Enabled = true;
		}

		public void RefreshList()
		{
			lst.Items.Clear();

			ScheduledTasks st = new ScheduledTasks();
			string[] taskNames = st.GetTaskNames();
			foreach(string name in taskNames)
			{
				try
				{
					if (name.Substring(0, 12).CompareTo("Nahrávání - ") == 0)
					{
						Task t = st.OpenTask(name);
						lst.Items.Add(new ListContainer(t.Name.Substring(12), name));
						t.Close();
					}
				} catch
				{
				}
			}

			st.Dispose();
		}

		private void lst_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListContainer item = (ListContainer) lst.SelectedItem;
			try
			{
				ScheduledTasks st = new ScheduledTasks();

				Task t = st.OpenTask(item.key);
				txtName.Text = item.name;

				foreach(Trigger tr in t.Triggers)
				{
					if (tr is RunOnceTrigger)
					{
						DateTime dt = (tr as RunOnceTrigger).BeginDate;

						dteBegin.Value = new DateTime(dt.Year, dt.Month, dt.Day, (tr as RunOnceTrigger).StartHour, (tr as RunOnceTrigger).StartMinute, 0);
					}
				}

				numLength.Value = (decimal) t.MaxRunTime.TotalMinutes;

				Regex r = new Regex("(?<uri>(udp://([0-9:@.]+))).*(:demuxdump-file=\"|:sout=#duplicate{dst=std{access=file,mux=ps,(url|dst)=\")(?<filename>([^\"]+))(\"|\"}})");
				Match m = r.Match(t.Parameters);
				cmbProgram.SelectedIndex = getProgramItemFromKey(m.Groups["uri"].Value);
				txtFilename.Text = m.Groups["filename"].Value;

				txtStatus.Text = StatusToText(t.Status);

				t.Close();

				st.Dispose();
			} catch(Exception)
			{
				lst.Items.Remove(item);
			}
		}

		public int getProgramItemFromKey(string key)
		{
			foreach(Object item in cmbProgram.Items)
			{
				if (((ProgramContainer) item).key.CompareTo(key) == 0)
					return cmbProgram.FindString(item.ToString());
			}
			return -1;
		}

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			ScheduledTasks st = new ScheduledTasks();
			try
			{
				Task t = st.CreateTask("Nahrávání - " + txtName.Text);
				if (t != null)
				{
					t.Triggers.Add(new RunOnceTrigger(dteBegin.Value));
					t.ApplicationName = vlc;

					if (useMpegTS)
					{
						t.Parameters = string.Format("{0} :demux=dump :demuxdump-file=\"{1}\"", ((ProgramContainer) cmbProgram.SelectedItem).key, txtFilename.Text);
					} else
					{
						t.Parameters = string.Format("{0} :sout=#duplicate{{dst=std{{access=file,mux=ps,url=\"{1}\"}}}}", ((ProgramContainer) cmbProgram.SelectedItem).key, txtFilename.Text);
					}
					t.MaxRunTime = new TimeSpan(0, (int) numLength.Value, 0);

					if (username.Length == 0) username = WindowsIdentity.GetCurrent().Name.ToString();
					t.Flags = (password == null || password.Length == 0) ? TaskFlags.RunOnlyIfLoggedOn : 0;

					if (password != null && password.Length == 0) password = null;
					t.SetAccountInformation(username, password);
					t.WorkingDirectory = defaultDirectory;
					t.Flags |= TaskFlags.DeleteWhenDone;

					String name = t.Name;

					t.Save();
					t.Close();

					int index = lst.Items.Add(new ListContainer(name.Substring(12), name + ".job"));
					lst.SelectedIndex = index;

					if (st.OpenTask("Nahrávání - " + txtName.Text) == null)
						cmdSave.Enabled = false;
					else
						cmdSave.Enabled = true;

					cmdDelete.Enabled = cmdSave.Enabled;

					if (txtName.Text.Length == 0 || txtFilename.Text.Length == 0 || st.OpenTask("Nahrávání - " + txtName.Text) != null)
						cmdAdd.Enabled = false;
					else
						cmdAdd.Enabled = true;
				} else
				{
					MessageBox.Show("Nepovedlo se pøidat nahrávání.\n\nUjistìte se, že název nahrávání neobsahuje následující znaky:\n/ \\ : * ? \" < > |", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			} catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			st.Dispose();
		}

		public void LoadConfig()
		{
			XmlDocument objXmlDoc = new XmlDocument();
			try
			{
				if (!File.Exists(string.Format(@"{0}\config.xml", Application.StartupPath)))
				{
					throw new Exception("Soubor config.xml nebyl nalezen. Pravdìpodobnì se jedná o první spuštìní tohoto programu, proto bude zobrazen dialog pro nastavení tohoto programu.");
				}
				objXmlDoc.Load(string.Format(@"{0}\config.xml", Application.StartupPath));

				XmlElement objRootXmlElement = objXmlDoc.DocumentElement;

				if (objRootXmlElement.SelectSingleNode("config/vlc") != null)
				{
					vlc = objRootXmlElement.SelectSingleNode("config/vlc").InnerText;
				} else
				{
					MessageBox.Show("Chyba v soubor config.xml.\n\nNení nastavena cesta k exe souboru programu VLC.\n\nPøeètìtet si prosím, jak nakonfigurovat program v souboru readme.txt.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					Application.Exit();
				}
				if (!File.Exists(vlc))
				{
					throw new Exception(string.Format("Chyba v soubor config.xml.\n\nCesta k VLC \"{0}\" neexistuje, nebo je adresáø (musí být soubor).\n\nPøeètìtet si prosím, jak nakonfigurovat program v souboru readme.txt.", vlc));
				}
				if (objRootXmlElement.SelectSingleNode("config/login/username") != null)
					username = objRootXmlElement.SelectSingleNode("config/login/username").InnerText;
				if (objRootXmlElement.SelectSingleNode("config/login/password") != null)
					password = objRootXmlElement.SelectSingleNode("config/login/password").InnerText;

				try
				{
					defaultDirectory = objRootXmlElement.SelectSingleNode("config/defaultdirectory").InnerText;
				} catch(Exception)
				{
					defaultDirectory = @"C:\";
				}
				try
				{
					useMpegTS = Boolean.Parse(objRootXmlElement.SelectSingleNode("config/use_mpegts").InnerText);
				} catch(Exception)
				{
				}

				XmlNodeList objNodes = objRootXmlElement.SelectNodes("programy/program");
				cmbProgram.Items.Clear();
				foreach(XmlNode objNode in objNodes)
					cmbProgram.Items.Add(new ProgramContainer(objNode.SelectSingleNode("nazev").InnerText, objNode.SelectSingleNode("uri").InnerText));

				if (cmbProgram.Items.Count > 0) cmbProgram.SelectedIndex = 0;

				objNodes = null;
				objXmlDoc = null;
			} catch(Exception ex)
			{
				MessageBox.Show(string.Format("Nepovedlo se naèíst soubor config.xml.\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				//Application.Exit();
				formSettings f = new formSettings();
				f.Text = "Nastavení programu " + Text;
				f.ShowDialog();
				if (!f.isCanceled)
				{
					LoadConfig();
				} else
				{
					Application.Exit();
				}
			}
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			txtFilename.Text = txtName.Text + ".mpg";

			ScheduledTasks st = new ScheduledTasks();

			if (st.OpenTask("Nahrávání - " + txtName.Text) == null)
				cmdSave.Enabled = false;
			else
				cmdSave.Enabled = true;

			cmdDelete.Enabled = cmdSave.Enabled;

			if (txtName.Text.Length == 0 || txtFilename.Text.Length == 0 || st.OpenTask("Nahrávání - " + txtName.Text) != null)
				cmdAdd.Enabled = false;
			else
				cmdAdd.Enabled = true;

			st.Dispose();
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
			ListContainer item = (ListContainer) lst.SelectedItem;
			try
			{
				ScheduledTasks st = new ScheduledTasks();

				Task t = st.OpenTask(item.key);
				t.Triggers.Clear();
				t.Triggers.Add(new RunOnceTrigger(dteBegin.Value));
				t.ApplicationName = vlc;

				if (useMpegTS)
				{
					t.Parameters = string.Format("{0} :demux=dump :demuxdump-file=\"{1}\"", ((ProgramContainer) cmbProgram.SelectedItem).key, txtFilename.Text);
				} else
				{
					t.Parameters = string.Format("{0} :sout=#duplicate{{dst=std{{access=file,mux=ps,url=\"{1}\"}}}}", ((ProgramContainer) cmbProgram.SelectedItem).key, txtFilename.Text);
				}

				t.MaxRunTime = new TimeSpan(0, (int) numLength.Value, 0);

				if (username.Length == 0)
					username = WindowsIdentity.GetCurrent().Name.ToString();
				t.Flags = (password == null || password.Length == 0) ? TaskFlags.RunOnlyIfLoggedOn : 0;

				if (password != null && password.Length == 0)
					password = null;
				t.SetAccountInformation(username, password);
				t.WorkingDirectory = defaultDirectory;
				t.Flags |= TaskFlags.DeleteWhenDone;

				t.Save();
				t.Close();
			} catch(Exception)
			{
				lst.Items.Remove(item);
			}
		}

		private void cmdDelete_Click(object sender, EventArgs e)
		{
			ScheduledTasks st = new ScheduledTasks();
			ListContainer item = (ListContainer) lst.SelectedItem;

			try
			{
				Task t = st.OpenTask(item.key);
				if (t.Status == TaskStatus.Running)
				{
					t.Terminate();
					Thread.Sleep(1000);
				}
				/*t.Save();*/
				t.Close();
			} catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			try
			{
				st.DeleteTask(item.key);
			} catch(Exception)
			{
			}
			txtName.Text = "";
			cmbProgram.SelectedIndex = 0;
			dteBegin.Value = DateTime.Now;
			dteEnd.Value = dteBegin.Value.AddMinutes(1);
			numLength.Value = 1;
			txtFilename.Text = "";
			txtStatus.Text = "Nahrávání nebylo ještì založeno.";
			lst.Items.Remove(item);

			st.Dispose();
		}

		public string StatusToText(TaskStatus status)
		{
			switch(status)
			{
				case TaskStatus.Disabled:
					return "Zakázaný";
				case TaskStatus.NeverRun:
					return "Pøipraveno k nahrávání";
				case TaskStatus.NoMoreRuns:
					return "Nenaplánováno další spuštìní";
				case TaskStatus.NoTriggers:
					return "Nenaplánováno";
				case TaskStatus.NoTriggerTime:
					return "Nenaplánováno";
				case TaskStatus.NotScheduled:
					return "Nenaplánováno";
				case TaskStatus.Ready:
					return "Pøipraveno k nahrávání";
				case TaskStatus.Running:
					return "Bìží";
				case TaskStatus.Terminated:
					return "Neúspìšnì vykonáno";
			}
			return "";
		}

		private void optionMenuItem_Click(object sender, EventArgs e)
		{
			formSettings f = new formSettings();
			f.Text = "Nastavení programu " + Text;
			f.ShowDialog(this);
			if (!f.isCanceled)
			{
				LoadConfig();
			}
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			ScheduledTasks st = new ScheduledTasks();
			string[] taskNames = st.GetTaskNames();

			foreach(string name in taskNames)
			{
				try
				{
					if (name.Substring(0, 12).CompareTo("Nahrávání - ") == 0)
					{
						Task t = st.OpenTask(name);
						string origName = t.Name.Substring(12);
						if (lst.FindStringExact(origName) == -1)
						{
							lst.Items.Add(new ListContainer(origName, name));
						}
						t.Close();
					}
				} catch
				{
				}
			}

			if (lst.Items.Count > 0)
			{
				ListContainer[] lc = new ListContainer[lst.Items.Count];
				lst.Items.CopyTo(lc, 0);

				foreach(ListContainer container in lc)
				{
					if (Array.IndexOf(taskNames, container.key) == -1)
					{
						if (lst.SelectedItem == container)
						{
							txtName.Text = "";
							cmbProgram.SelectedIndex = 0;
							dteBegin.Value = DateTime.Now;
							dteEnd.Value = dteBegin.Value.AddMinutes(1);
							numLength.Value = 1;
							txtFilename.Text = "";
							txtStatus.Text = "Nahrávání nebylo ještì založeno.";
						}
						lst.Items.Remove(container);
					}

					if (lst.SelectedItem == container)
					{
						try
						{
							Task t = st.OpenTask(container.key);
							txtStatus.Text = StatusToText(t.Status);
							t.Close();
						} catch(Exception)
						{
						}
					}
				}
			}
			st.Dispose();
		}

		private void recordNowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			formRecordNow f;
			f = new formRecordNow();
			f.ShowDialog();
		}

		private void dteEnd_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes <= 0)
			{
				//e.Cancel = true;
				Utils.ShowBubble(dteEnd,ToolTipIcon.Error, "Chyba v datumu!", "Datum konce poøadu je nastaven pøed datum zaèátku!");
				
				DateTime val = dteBegin.Value;
				val = val.AddMinutes(1);
				dteEnd.Value = val;
			}
		}

		private void dteEnd_ValueChanged(object sender, EventArgs e)
		{
			if (dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes <= 0)
				return;
			
			numLength.Value = (int) Decimal.Round((decimal)dteEnd.Value.Subtract(dteBegin.Value).TotalMinutes);
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
	}

	public class ListContainer
	{
		public string name;
		public string key;

		public ListContainer(string name, string key)
		{
			this.name = name;
			this.key = key;
		}

		public override string ToString()
		{
			return name;
		}
	}

	public class ProgramContainer
	{
		public string name;
		public string key;

		public ProgramContainer(string name, string key)
		{
			this.name = name;
			this.key = key;
		}

		public override string ToString()
		{
			return name;
		}
	}
}