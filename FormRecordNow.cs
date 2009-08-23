using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Nahravadlo
{
	public partial class FormRecordNow : Form
	{
		private readonly bool _filenameLowerCase;
		private readonly string _filenameMask = "%N.mpg";
		private readonly int _filenameSpaceReplacement;
		private readonly bool _filenameWithoutDiacritics;

		public FormRecordNow()
		{
			InitializeComponent();
			PopulateCmbChannel();
			cmdRunNow.Enabled = false;

			var setting = Settings.GetInstance();

			_filenameMask = setting.GetString("nahravadlo/config/filename/mask", "%N.mpg");
			_filenameWithoutDiacritics = setting.GetBool("nahravadlo/config/filename/without_diacritics", false);
			_filenameLowerCase = setting.GetBool("nahravadlo/config/filename/lower_case", false);
			_filenameSpaceReplacement = setting.GetInt("nahravadlo/config/filename/space_replacement", 0);
			if (_filenameSpaceReplacement < 0 || _filenameSpaceReplacement > 3)
				_filenameSpaceReplacement = 0;

			ReformatFilename();
		}

		private void cmdBrowse_Click(object sender, EventArgs e)
		{
			dialog.InitialDirectory = FormMain.DefaultDirectory;
			dialog.FileName = txtFilename.Text;
			dialog.OverwritePrompt = true;
			dialog.Filter = "MPEG2 soubor (*.mpg)|*.mpg|VLC soubor (*.vlc)|*.vlc";
			dialog.ValidateNames = true;
			if (dialog.ShowDialog() == DialogResult.OK)
				txtFilename.Text = dialog.FileName;
		}

		private void PopulateCmbChannel()
		{
			for (var i = 0; i < FormMain.ComboChannels.Items.Count; i++)
				cmbChannel.Items.Add(FormMain.ComboChannels.Items[i]);
			if (cmbChannel.Items.Count > 0)
				cmbChannel.SelectedIndex = 0;
		}

		private void cmdRunNow_Click(object sender, EventArgs e)
		{
			var dst = "";
			var mux = "ps";

			if (chkPlayStream.Checked)
				dst = "dst=display,";
			if (FormMain.UseMpegTS)
				mux = "ts";

			//parameters = string.Format("{0} :demux=dump :demuxdump-file=\"{1}\"", ((Channel) cmbChannel.SelectedItem).getUri(), txtFilename.Text);
			var parameters = string.Format("{0} :sout=#duplicate{{{1}dst=std{{access=file,mux={2},dst=\"{3}\"}}}}", ((Channel) cmbChannel.SelectedItem).Uri, dst, mux, txtFilename.Text);

			//MessageBox.Show(parameters);

			try
			{
				var psi = new ProcessStartInfo(FormMain.Vlc, parameters) {WorkingDirectory = FormMain.DefaultDirectory};
				var process = Process.Start(psi);
				if (process != null)
					process.PriorityClass = ProcessPriorityClass.AboveNormal;
			}
			catch {}
			Close();
		}

		private void txtFilename_TextChanged(object sender, EventArgs e)
		{
			cmdRunNow.Enabled = (txtFilename.Text.Trim().Length > 0);
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			ReformatFilename();
		}

		private void ReformatFilename()
		{
			var tmp = _filenameMask;

			tmp = tmp.Replace("%%", Char.ConvertFromUtf32(0));

			tmp = tmp.Replace("%N", txtName.Text.Trim());
			tmp = tmp.Replace("%S", cmbChannel.Text.Trim());

			tmp = tmp.Replace("%H", DateTime.Now.Hour.ToString("00"));
			tmp = tmp.Replace("%i", DateTime.Now.Minute.ToString("00"));

			tmp = tmp.Replace("%D", DateTime.Now.Day.ToString("00"));
			tmp = tmp.Replace("%M", DateTime.Now.Month.ToString("00"));
			tmp = tmp.Replace("%Y", DateTime.Now.Year.ToString("0000"));
			tmp = tmp.Replace("%y", (DateTime.Now.Year%100).ToString("00"));

			tmp = tmp.Replace("%L", "0");

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
	}
}