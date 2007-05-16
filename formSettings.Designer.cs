namespace Nahravadlo
{
	partial class formSettings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSaveAndClose = new System.Windows.Forms.Button();
			this.openFile = new System.Windows.Forms.OpenFileDialog();
			this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.btnHelp = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtVLCPath = new System.Windows.Forms.TextBox();
			this.btnSelectVLC = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.txtDefaultDirectory = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.chkUseMPEGTS = new System.Windows.Forms.CheckBox();
			this.btnContainerHelp = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabControl1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(12, 293);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(95, 30);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Storno";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSaveAndClose
			// 
			this.btnSaveAndClose.Location = new System.Drawing.Point(423, 293);
			this.btnSaveAndClose.Name = "btnSaveAndClose";
			this.btnSaveAndClose.Size = new System.Drawing.Size(95, 30);
			this.btnSaveAndClose.TabIndex = 2;
			this.btnSaveAndClose.Text = "Uložit && Zavřít";
			this.btnSaveAndClose.UseVisualStyleBackColor = true;
			this.btnSaveAndClose.Click += new System.EventHandler(this.btnSaveAndClose_Click);
			// 
			// openFile
			// 
			this.openFile.Filter = "Spustitelné soubory|*.exe;*.com;*.cmd;*.bat|Všechny soubory|*.*";
			this.openFile.Title = "Nalezení programu VLC...";
			// 
			// folderBrowser
			// 
			this.folderBrowser.Description = "Vyberte adresář, kam se budou ukládat soubory, pokud nebude u nich uvedena cesta." +
				"";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.ItemSize = new System.Drawing.Size(100, 18);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(506, 275);
			this.tabControl1.TabIndex = 4;
			// 
			// btnHelp
			// 
			this.btnHelp.Location = new System.Drawing.Point(268, 20);
			this.btnHelp.Name = "btnHelp";
			this.btnHelp.Size = new System.Drawing.Size(63, 23);
			this.btnHelp.TabIndex = 0;
			this.btnHelp.Text = "Nápověda";
			this.btnHelp.UseVisualStyleBackColor = true;
			this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Uživatel:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Heslo:";
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(71, 22);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(191, 20);
			this.txtUsername.TabIndex = 5;
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(71, 45);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '●';
			this.txtPassword.Size = new System.Drawing.Size(191, 20);
			this.txtPassword.TabIndex = 6;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.txtPassword);
			this.groupBox2.Controls.Add(this.txtUsername);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.btnHelp);
			this.groupBox2.Location = new System.Drawing.Point(6, 145);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(486, 98);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Spustit pod užvatelem";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Cesta k VLC:";
			// 
			// txtVLCPath
			// 
			this.txtVLCPath.Location = new System.Drawing.Point(121, 24);
			this.txtVLCPath.Name = "txtVLCPath";
			this.txtVLCPath.Size = new System.Drawing.Size(325, 20);
			this.txtVLCPath.TabIndex = 1;
			// 
			// btnSelectVLC
			// 
			this.btnSelectVLC.Location = new System.Drawing.Point(449, 22);
			this.btnSelectVLC.Margin = new System.Windows.Forms.Padding(0);
			this.btnSelectVLC.Name = "btnSelectVLC";
			this.btnSelectVLC.Size = new System.Drawing.Size(34, 23);
			this.btnSelectVLC.TabIndex = 2;
			this.btnSelectVLC.Text = "...";
			this.btnSelectVLC.UseVisualStyleBackColor = true;
			this.btnSelectVLC.Click += new System.EventHandler(this.btnSelectVLC_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 58);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(114, 26);
			this.label4.TabIndex = 3;
			this.label4.Text = "Výchozí adresář: \r\n(pro ukládání souboru)";
			// 
			// txtDefaultDirectory
			// 
			this.txtDefaultDirectory.Location = new System.Drawing.Point(121, 55);
			this.txtDefaultDirectory.Name = "txtDefaultDirectory";
			this.txtDefaultDirectory.Size = new System.Drawing.Size(325, 20);
			this.txtDefaultDirectory.TabIndex = 4;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(449, 53);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(34, 24);
			this.btnBrowse.TabIndex = 5;
			this.btnBrowse.Text = "...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// chkUseMPEGTS
			// 
			this.chkUseMPEGTS.AutoSize = true;
			this.chkUseMPEGTS.Location = new System.Drawing.Point(9, 100);
			this.chkUseMPEGTS.Name = "chkUseMPEGTS";
			this.chkUseMPEGTS.Size = new System.Drawing.Size(288, 17);
			this.chkUseMPEGTS.TabIndex = 6;
			this.chkUseMPEGTS.Text = "Použít místo MPEG PS kontejneru MPEG TS kontejner";
			this.chkUseMPEGTS.UseVisualStyleBackColor = true;
			// 
			// btnContainerHelp
			// 
			this.btnContainerHelp.Location = new System.Drawing.Point(303, 96);
			this.btnContainerHelp.Name = "btnContainerHelp";
			this.btnContainerHelp.Size = new System.Drawing.Size(67, 23);
			this.btnContainerHelp.TabIndex = 7;
			this.btnContainerHelp.Text = "Nápověda";
			this.btnContainerHelp.UseVisualStyleBackColor = true;
			this.btnContainerHelp.Click += new System.EventHandler(this.btnContainerHelp_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnContainerHelp);
			this.groupBox1.Controls.Add(this.chkUseMPEGTS);
			this.groupBox1.Controls.Add(this.btnBrowse);
			this.groupBox1.Controls.Add(this.txtDefaultDirectory);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.btnSelectVLC);
			this.groupBox1.Controls.Add(this.txtVLCPath);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(6, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(486, 136);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Nastavení VLC a cesty ukládání";
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(498, 249);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Obecné";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// formSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(530, 335);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.btnSaveAndClose);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "formSettings";
			this.Text = "Nastavení programu Nahrávadlo";
			this.tabControl1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPage1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnSaveAndClose;
		private System.Windows.Forms.OpenFileDialog openFile;
		private System.Windows.Forms.FolderBrowserDialog folderBrowser;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnContainerHelp;
		private System.Windows.Forms.CheckBox chkUseMPEGTS;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox txtDefaultDirectory;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnSelectVLC;
		private System.Windows.Forms.TextBox txtVLCPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnHelp;
	}
}