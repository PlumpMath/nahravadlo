using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Nahravadlo
{
	public partial class formRecordNow : Form
	{
		public formRecordNow()
		{
			InitializeComponent();
			populateCmbChannel();
			cmdRunNow.Enabled = false;
		}

		private void cmdBrowse_Click(object sender, EventArgs e)
		{
			dialog.InitialDirectory = formMain.defaultDirectory;
			dialog.FileName = txtFilename.Text;
			dialog.OverwritePrompt = true;
			dialog.Filter = "MPEG2 soubor (*.mpg)|*.mpg|VLC soubor (*.vlc)|*.vlc";
			dialog.ValidateNames = true;
			if (dialog.ShowDialog() == DialogResult.OK)
				txtFilename.Text = dialog.FileName;
		}

		private void populateCmbChannel()
		{
			for (int i = 0; i < formMain.comboChannels.Items.Count; i++)
			{
				cmbChannel.Items.Add(formMain.comboChannels.Items[i]);
			}
			if (cmbChannel.Items.Count > 0)
				cmbChannel.SelectedIndex = 0;
		}

		private void cmdRunNow_Click(object sender, EventArgs e)
		{
			String parameters;
			if (formMain.useMpegTS)
			{
				parameters = string.Format("{0} :demux=dump :demuxdump-file=\"{1}\"", ((Channel) cmbChannel.SelectedItem).getUri(), txtFilename.Text);
			} else
			{
				parameters = string.Format("{0} :sout=#duplicate{{dst=std{{access=file,mux=ps,url=\"{1}\"}}}}", ((Channel) cmbChannel.SelectedItem).getUri(), txtFilename.Text);
			}

			Process.Start(formMain.vlc, parameters);
			Close();
		}

		private void txtFilename_TextChanged(object sender, EventArgs e)
		{
			cmdRunNow.Enabled = (txtFilename.Text.Trim().Length > 0);
		}
	}
}