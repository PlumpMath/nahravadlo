namespace Nahravadlo
{
	partial class formMain
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMain));
			this.lst = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtStatus = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.cmdDelete = new System.Windows.Forms.Button();
			this.cmdSave = new System.Windows.Forms.Button();
			this.cmdAdd = new System.Windows.Forms.Button();
			this.cmdBrowse = new System.Windows.Forms.Button();
			this.txtFilename = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.numLength = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.dteBegin = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.cmbProgram = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.dialog = new System.Windows.Forms.SaveFileDialog();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.optionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recordNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) (this.numLength)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lst
			// 
			this.lst.FormattingEnabled = true;
			resources.ApplyResources(this.lst, "lst");
			this.lst.Name = "lst";
			this.lst.Sorted = true;
			this.lst.SelectedIndexChanged += new System.EventHandler(this.lst_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtStatus);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.cmdDelete);
			this.groupBox1.Controls.Add(this.cmdSave);
			this.groupBox1.Controls.Add(this.cmdAdd);
			this.groupBox1.Controls.Add(this.cmdBrowse);
			this.groupBox1.Controls.Add(this.txtFilename);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.txtName);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.numLength);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.dteBegin);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.cmbProgram);
			this.groupBox1.Controls.Add(this.label1);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// txtStatus
			// 
			this.txtStatus.BackColor = System.Drawing.SystemColors.Control;
			this.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this.txtStatus, "txtStatus");
			this.txtStatus.Name = "txtStatus";
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// cmdDelete
			// 
			resources.ApplyResources(this.cmdDelete, "cmdDelete");
			this.cmdDelete.Name = "cmdDelete";
			this.cmdDelete.UseVisualStyleBackColor = true;
			this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
			// 
			// cmdSave
			// 
			resources.ApplyResources(this.cmdSave, "cmdSave");
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.UseVisualStyleBackColor = true;
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			// 
			// cmdAdd
			// 
			resources.ApplyResources(this.cmdAdd, "cmdAdd");
			this.cmdAdd.Name = "cmdAdd";
			this.cmdAdd.UseVisualStyleBackColor = true;
			this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			// 
			// cmdBrowse
			// 
			resources.ApplyResources(this.cmdBrowse, "cmdBrowse");
			this.cmdBrowse.Name = "cmdBrowse";
			this.cmdBrowse.UseVisualStyleBackColor = true;
			this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
			// 
			// txtFilename
			// 
			resources.ApplyResources(this.txtFilename, "txtFilename");
			this.txtFilename.Name = "txtFilename";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// txtName
			// 
			resources.ApplyResources(this.txtName, "txtName");
			this.txtName.Name = "txtName";
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// numLength
			// 
			resources.ApplyResources(this.numLength, "numLength");
			this.numLength.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
			this.numLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numLength.Name = "numLength";
			this.numLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// dteBegin
			// 
			resources.ApplyResources(this.dteBegin, "dteBegin");
			this.dteBegin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dteBegin.Name = "dteBegin";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// cmbProgram
			// 
			this.cmbProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbProgram.FormattingEnabled = true;
			resources.ApplyResources(this.cmbProgram, "cmbProgram");
			this.cmbProgram.Name = "cmbProgram";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionMenuItem,
            this.recordNowToolStripMenuItem});
			resources.ApplyResources(this.menuStrip1, "menuStrip1");
			this.menuStrip1.Name = "menuStrip1";
			// 
			// optionMenuItem
			// 
			this.optionMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.optionMenuItem.Name = "optionMenuItem";
			resources.ApplyResources(this.optionMenuItem, "optionMenuItem");
			this.optionMenuItem.Click += new System.EventHandler(this.optionMenuItem_Click);
			// 
			// recordNowToolStripMenuItem
			// 
			this.recordNowToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.recordNowToolStripMenuItem.Name = "recordNowToolStripMenuItem";
			resources.ApplyResources(this.recordNowToolStripMenuItem, "recordNowToolStripMenuItem");
			this.recordNowToolStripMenuItem.Click += new System.EventHandler(this.recordNowToolStripMenuItem_Click);
			// 
			// timer
			// 
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// formMain
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lst);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.Name = "formMain";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize) (this.numLength)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lst;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dteBegin;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cmbProgram;
		private System.Windows.Forms.NumericUpDown numLength;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button cmdBrowse;
		private System.Windows.Forms.TextBox txtFilename;
		private System.Windows.Forms.Button cmdSave;
		private System.Windows.Forms.Button cmdAdd;
		private System.Windows.Forms.SaveFileDialog dialog;
		private System.Windows.Forms.Button cmdDelete;
		private System.Windows.Forms.TextBox txtStatus;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem optionMenuItem;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ToolStripMenuItem recordNowToolStripMenuItem;
	}
}

