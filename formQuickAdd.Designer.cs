namespace Nahravadlo
{
    partial class formQuickAdd
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formQuickAdd));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dteEnd = new System.Windows.Forms.DateTimePicker();
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
			this.cmdClose = new System.Windows.Forms.Button();
			this.cmdAdd = new System.Windows.Forms.Button();
			this.cmdAddAndClose = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.dialog = new System.Windows.Forms.SaveFileDialog();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) (this.numLength)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.Controls.Add(this.dteEnd);
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
			this.groupBox1.Location = new System.Drawing.Point(12, 76);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(447, 167);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Informace o nahrávání";
			// 
			// dteEnd
			// 
			this.dteEnd.CustomFormat = "dd. MM. yyyy - HH:mm";
			this.dteEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dteEnd.Location = new System.Drawing.Point(311, 101);
			this.dteEnd.Name = "dteEnd";
			this.dteEnd.Size = new System.Drawing.Size(130, 20);
			this.dteEnd.TabIndex = 17;
			this.dteEnd.ValueChanged += new System.EventHandler(this.dteEnd_ValueChanged);
			this.dteEnd.Validating += new System.ComponentModel.CancelEventHandler(this.dteEnd_Validating);
			// 
			// cmdBrowse
			// 
			this.cmdBrowse.Location = new System.Drawing.Point(407, 126);
			this.cmdBrowse.Name = "cmdBrowse";
			this.cmdBrowse.Size = new System.Drawing.Size(34, 22);
			this.cmdBrowse.TabIndex = 11;
			this.cmdBrowse.Text = "...";
			this.cmdBrowse.UseVisualStyleBackColor = true;
			this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
			// 
			// txtFilename
			// 
			this.txtFilename.Location = new System.Drawing.Point(167, 127);
			this.txtFilename.Name = "txtFilename";
			this.txtFilename.Size = new System.Drawing.Size(234, 20);
			this.txtFilename.TabIndex = 10;
			this.txtFilename.TextChanged += new System.EventHandler(this.txtFilename_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 130);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(92, 13);
			this.label6.TabIndex = 9;
			this.label6.Text = "Uložit do souboru:";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(167, 25);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(274, 20);
			this.txtName.TabIndex = 8;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 25);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(93, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "Název nahrávání:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(225, 103);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "minut, nebo do:";
			// 
			// numLength
			// 
			this.numLength.Location = new System.Drawing.Point(167, 101);
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
			this.numLength.Size = new System.Drawing.Size(52, 20);
			this.numLength.TabIndex = 5;
			this.numLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numLength.ValueChanged += new System.EventHandler(this.numLength_ValueChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 103);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Délka poøadu:";
			// 
			// dteBegin
			// 
			this.dteBegin.CustomFormat = "dd. MM. yyyy - HH:mm";
			this.dteBegin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dteBegin.Location = new System.Drawing.Point(167, 75);
			this.dteBegin.Name = "dteBegin";
			this.dteBegin.Size = new System.Drawing.Size(274, 20);
			this.dteBegin.TabIndex = 3;
			this.dteBegin.ValueChanged += new System.EventHandler(this.dteBegin_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 79);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(148, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Datum a èas zaèátku poøadu:";
			// 
			// cmbProgram
			// 
			this.cmbProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbProgram.FormattingEnabled = true;
			this.cmbProgram.Location = new System.Drawing.Point(167, 48);
			this.cmbProgram.Name = "cmbProgram";
			this.cmbProgram.Size = new System.Drawing.Size(274, 21);
			this.cmbProgram.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Stanice:";
			// 
			// cmdClose
			// 
			this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdClose.Location = new System.Drawing.Point(12, 249);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(101, 28);
			this.cmdClose.TabIndex = 14;
			this.cmdClose.Text = "Zavøít";
			this.cmdClose.UseVisualStyleBackColor = true;
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// cmdAdd
			// 
			this.cmdAdd.Enabled = false;
			this.cmdAdd.Location = new System.Drawing.Point(251, 249);
			this.cmdAdd.Name = "cmdAdd";
			this.cmdAdd.Size = new System.Drawing.Size(101, 28);
			this.cmdAdd.TabIndex = 13;
			this.cmdAdd.Text = "Pøidat";
			this.cmdAdd.UseVisualStyleBackColor = true;
			this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			// 
			// cmdAddAndClose
			// 
			this.cmdAddAndClose.Enabled = false;
			this.cmdAddAndClose.Location = new System.Drawing.Point(358, 249);
			this.cmdAddAndClose.Name = "cmdAddAndClose";
			this.cmdAddAndClose.Size = new System.Drawing.Size(101, 28);
			this.cmdAddAndClose.TabIndex = 12;
			this.cmdAddAndClose.Text = "Pøidat a zavøit";
			this.cmdAddAndClose.UseVisualStyleBackColor = true;
			this.cmdAddAndClose.Click += new System.EventHandler(this.cmdAddAndClose_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.pictureBox1.Image = global::Nahravadlo.Properties.Resources.quickAddHeader;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(470, 71);
			this.pictureBox1.TabIndex = 15;
			this.pictureBox1.TabStop = false;
			// 
			// formQuickAdd
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(470, 286);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cmdAdd);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdAddAndClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "formQuickAdd";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Nahrávadlo - Rychlé nahrávání";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize) (this.numLength)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dteEnd;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.Button cmdAddAndClose;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dteBegin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbProgram;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.SaveFileDialog dialog;
    }
}