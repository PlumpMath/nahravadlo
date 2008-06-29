using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Nahravadlo
{
    public partial class formRecordNow : Form
    {
        private readonly bool filenameLowerCase;
        private readonly string filenameMask = "%N.mpg";
        private readonly int filenameSpaceReplacement;
        private readonly bool filenameWithoutDiacritics;

        public formRecordNow()
        {
            InitializeComponent();
            populateCmbChannel();
            cmdRunNow.Enabled = false;

            Settings setting = Settings.getInstance();

            filenameMask = setting.getString("nahravadlo/config/filename/mask", "%N.mpg");
            filenameWithoutDiacritics = setting.getBool("nahravadlo/config/filename/without_diacritics", false);
            filenameLowerCase = setting.getBool("nahravadlo/config/filename/lower_case", false);
            filenameSpaceReplacement = setting.getInt("nahravadlo/config/filename/space_replacement", 0);
            if (filenameSpaceReplacement < 0 || filenameSpaceReplacement > 3) filenameSpaceReplacement = 0;

            ReformatFilename();
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
                cmbChannel.Items.Add(formMain.comboChannels.Items[i]);
            if (cmbChannel.Items.Count > 0)
                cmbChannel.SelectedIndex = 0;
        }

        private void cmdRunNow_Click(object sender, EventArgs e)
        {
            String dst = "";
            String mux = "ps";

            if (chkPlayStream.Checked) dst = "dst=display,";
            if (formMain.useMpegTS) mux = "ts";

            //parameters = string.Format("{0} :demux=dump :demuxdump-file=\"{1}\"", ((Channel) cmbChannel.SelectedItem).getUri(), txtFilename.Text);
            string parameters = string.Format("{0} :sout=#duplicate{{{1}dst=std{{access=file,mux={2},url=\"{3}\"}}}}",
                                              ((Channel) cmbChannel.SelectedItem).getUri(), dst, mux, txtFilename.Text);

            //MessageBox.Show(parameters);

            try
            {
                var psi = new ProcessStartInfo(formMain.vlc, parameters) { WorkingDirectory = formMain.defaultDirectory };
                var process = Process.Start(psi);
                if (process != null) process.PriorityClass = ProcessPriorityClass.AboveNormal;
            } catch {}
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
            string tmp = filenameMask;

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

            if (filenameWithoutDiacritics) tmp = Utils.RemoveDiacritics(tmp);
            if (filenameLowerCase) tmp = tmp.ToLower();

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
    }
}