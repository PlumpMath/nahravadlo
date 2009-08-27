using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Nahravadlo
{
	public partial class FormQuickAdd : Form
	{
		private readonly bool _filenameLowerCase;
		private readonly string _filenameMask = "%N.mpg";
		private readonly int _filenameSpaceReplacement;
		private readonly bool _filenameWithoutDiacritics;
		private readonly Settings _settings;
		private readonly Channels _channels;

		public FormQuickAdd()
		{
			InitializeComponent();
			Application.EnableVisualStyles();

			_settings = Settings.GetInstance();

			_filenameMask = _settings.GetString("nahravadlo/config/filename/mask", "%N.mpg");
			_filenameWithoutDiacritics = _settings.GetBool("nahravadlo/config/filename/without_diacritics", false);
			_filenameLowerCase = _settings.GetBool("nahravadlo/config/filename/lower_case", false);
			_filenameSpaceReplacement = _settings.GetInt("nahravadlo/config/filename/space_replacement", 0);
			if (_filenameSpaceReplacement < 0 || _filenameSpaceReplacement > 3)
				_filenameSpaceReplacement = 0;

			_channels = new Channels(_settings);

			//Naplneni kanalu
			foreach (var channel in _channels.Get())
				cmbProgram.Items.Add(channel);

			//Vyber prvniho kanalu v seznamu
			if (cmbProgram.Items.Count > 0)
				cmbProgram.SelectedIndex = 0;
		}

		public FormQuickAdd(string gid, string name, DateTime start, DateTime stop) : this()
		{
			//vybereme kanal
			if (_channels.ChannelFromId(gid) != null)
				cmbProgram.Text = _channels.ChannelFromId(gid).ToString();

			//vlozime nazev nahravani
			txtName.Text = Utils.CorrectFilename(name);

			//vlozime nazev souboru
			txtFilename.Text = txtName.Text + ".mpg";

			//nastavime pocatek nahravani
			dteBegin.Value = start;

			//posuneme konec nahravani, pokud je to nastaveny
			stop += TimeSpan.FromMinutes(_settings.GetInt("nahravadlo/config/add_schedule_minutes", 0));
			//nastavime konec nahravani
			dteEnd.Value = stop;
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

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			ReformatFilename();

			if (cmbProgram.SelectedIndex < 0 || txtName.Text.Length == 0 || txtFilename.Text.Length == 0 || FormMain.Schedules.Exist(txtName.Text))
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
			if (cmbProgram.SelectedIndex < 0 || txtName.Text.Length == 0 || txtFilename.Text.Length == 0 || FormMain.Schedules.Exist(txtName.Text))
				cmdAdd.Enabled = false;
			else
				cmdAdd.Enabled = true;

			cmdAddAndClose.Enabled = cmdAdd.Enabled;
		}

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			using (var job = FormMain.Schedules.Create(txtName.Text))
			{
				job.Start = dteBegin.Value;
				job.End = dteEnd.Value;
				job.Uri = ((Channel) cmbProgram.SelectedItem).Uri;
				job.Filename = txtFilename.Text;
				job.UseMPEGTS = FormMain.UseMpegTS;

				var username = _settings.GetString("nahravadlo/config/login/username", "");
				var password = _settings.GetString("nahravadlo/config/login/password", "");

				job.Save(username, password);
			}
			DialogResult = DialogResult.Yes;
			Close();
		}

		private void cmdAddAndClose_Click(object sender, EventArgs e)
		{
			using (var job = FormMain.Schedules.Create(txtName.Text))
			{
				job.Start = dteBegin.Value;
				job.End = dteEnd.Value;
				job.Uri = ((Channel) cmbProgram.SelectedItem).Uri;
				job.Filename = txtFilename.Text;
				job.UseMPEGTS = FormMain.UseMpegTS;

				var username = _settings.GetString("nahravadlo/config/login/username", "");
				var password = _settings.GetString("nahravadlo/config/login/password", "");

				job.Save(username, password);
			}
			DialogResult = DialogResult.OK;
			Close();
		}

		private void cmdBrowse_Click(object sender, EventArgs e)
		{
			dialog.InitialDirectory = FormMain.DefaultDirectory;
			dialog.FileName = txtFilename.Text;
			dialog.OverwritePrompt = true;
			dialog.Filter = "MPEG 2 soubor (*.mpg)|*.mpg|VLC soubor (*.vlc)|*.vlc";
			dialog.ValidateNames = true;
			if (dialog.ShowDialog() == DialogResult.OK)
				txtFilename.Text = dialog.FileName;
		}

		private void ReformatFilename()
		{
			var tmp = _filenameMask;

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

			if (_filenameWithoutDiacritics)
				tmp = Utils.RemoveDiacritics(tmp);
			if (_filenameLowerCase)
				tmp = tmp.ToLower();

			switch (_filenameSpaceReplacement)
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

		private void cmbProgram_SelectedIndexChanged(object sender, EventArgs e)
		{
			ReformatFilename();

			if (cmbProgram.SelectedIndex < 0 || txtName.Text.Length == 0 || txtFilename.Text.Length == 0 || FormMain.Schedules.Exist(txtName.Text))
				cmdAdd.Enabled = false;
			else
				cmdAdd.Enabled = true;

			cmdAddAndClose.Enabled = cmdAdd.Enabled;
		}
	}
}