using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Nahravadlo
{
	public partial class formQuickAdd : Form
	{
		private bool terminate = false;

		public bool Terminate
		{
			get { return terminate; }
		}

		public formQuickAdd(string gid, string name, DateTime start, DateTime stop, long length)
		{
			InitializeComponent();
			Application.EnableVisualStyles();

			Channels channels = new Channels(formMain.SETTING);

			//Naplneni kanalu
			foreach(Channel channel in channels.getChannels())
				cmbProgram.Items.Add(channel);

			//vybereme kanal
			if (channels.getChannelFromId(gid) != null)
				cmbProgram.Text = channels.getChannelFromId(gid).ToString();

			//vlozime nazev nahravani
			txtName.Text = Utils.CorrectFilename(name);

			//vlozime nazev souboru
			txtFilename.Text = txtName.Text + ".mpg";

			//nastavime pocatek nahravani
			dteBegin.Value = start;

			//posuneme konec nahravani, pokud je to nastaveny
			stop += TimeSpan.FromMinutes(formMain.SETTING.getInt("nahravadlo/config/add_schedule_minutes", 0));
			//nastavime konec nahravani
			dteEnd.Value = stop;
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

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			txtFilename.Text = txtName.Text + ".mpg";

			if (cmbProgram.SelectedIndex < 0 || txtName.Text.Length == 0 || txtFilename.Text.Length == 0 ||
			    formMain.SCHEDULES.exist(txtName.Text))
				cmdAdd.Enabled = false;
			else
				cmdAdd.Enabled = true;

			cmdAddAndClose.Enabled = cmdAdd.Enabled;
		}

		private void cmdClose_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Abort;
			Close();
		}

		private void txtFilename_TextChanged(object sender, EventArgs e)
		{
			if (cmbProgram.SelectedIndex < 0 || txtName.Text.Length == 0 || txtFilename.Text.Length == 0 ||
			    formMain.SCHEDULES.exist(txtName.Text))
				cmdAdd.Enabled = false;
			else
				cmdAdd.Enabled = true;

			cmdAddAndClose.Enabled = cmdAdd.Enabled;
		}

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			using(Job job = formMain.SCHEDULES.create(txtName.Text))
			{
				job.Start = dteBegin.Value;
				job.End = dteEnd.Value;
				job.Uri = ((Channel) cmbProgram.SelectedItem).getUri();
				job.Filename = txtFilename.Text;
				job.UseMPEGTS = formMain.useMpegTS;

				string username = formMain.SETTING.getString("nahravadlo/config/login/username", "");
				string password = formMain.SETTING.getString("nahravadlo/config/login/password", "");

				job.SetUsernameAndPassword(username, password);
			}
			DialogResult = DialogResult.Yes;
			Close();
		}

		private void cmdAddAndClose_Click(object sender, EventArgs e)
		{
			using(Job job = formMain.SCHEDULES.create(txtName.Text))
			{
				job.Start = dteBegin.Value;
				job.End = dteEnd.Value;
				job.Uri = ((Channel) cmbProgram.SelectedItem).getUri();
				job.Filename = txtFilename.Text;
				job.UseMPEGTS = formMain.useMpegTS;

				string username = formMain.SETTING.getString("nahravadlo/config/login/username", "");
				string password = formMain.SETTING.getString("nahravadlo/config/login/password", "");

				job.SetUsernameAndPassword(username, password);
			}
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}