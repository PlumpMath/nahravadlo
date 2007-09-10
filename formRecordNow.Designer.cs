namespace Nahravadlo
{
	partial class formRecordNow
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
			this.lblChannel = new System.Windows.Forms.Label();
			this.cmbChannel = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtFilename = new System.Windows.Forms.TextBox();
			this.cmdBrowse = new System.Windows.Forms.Button();
			this.cmdRunNow = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.dialog = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// lblChannel
			// 
			this.lblChannel.AutoSize = true;
			this.lblChannel.Location = new System.Drawing.Point(12, 9);
			this.lblChannel.Name = "lblChannel";
			this.lblChannel.Size = new System.Drawing.Size(46, 13);
			this.lblChannel.TabIndex = 0;
			this.lblChannel.Text = "Stanice:";
			// 
			// cmbChannel
			// 
			this.cmbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbChannel.FormattingEnabled = true;
			this.cmbChannel.Location = new System.Drawing.Point(115, 6);
			this.cmbChannel.Name = "cmbChannel";
			this.cmbChannel.Size = new System.Drawing.Size(170, 21);
			this.cmbChannel.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(92, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Uložit do souboru:";
			// 
			// txtFilename
			// 
			this.txtFilename.Location = new System.Drawing.Point(115, 33);
			this.txtFilename.Name = "txtFilename";
			this.txtFilename.Size = new System.Drawing.Size(170, 20);
			this.txtFilename.TabIndex = 3;
			this.txtFilename.TextChanged += new System.EventHandler(this.txtFilename_TextChanged);
			// 
			// cmdBrowse
			// 
			this.cmdBrowse.Location = new System.Drawing.Point(291, 32);
			this.cmdBrowse.Name = "cmdBrowse";
			this.cmdBrowse.Size = new System.Drawing.Size(33, 23);
			this.cmdBrowse.TabIndex = 4;
			this.cmdBrowse.Text = "...";
			this.cmdBrowse.UseVisualStyleBackColor = true;
			this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
			// 
			// cmdRunNow
			// 
			this.cmdRunNow.Location = new System.Drawing.Point(225, 66);
			this.cmdRunNow.Name = "cmdRunNow";
			this.cmdRunNow.Size = new System.Drawing.Size(99, 23);
			this.cmdRunNow.TabIndex = 5;
			this.cmdRunNow.Text = "Zaèít nahrávat";
			this.cmdRunNow.UseVisualStyleBackColor = true;
			this.cmdRunNow.Click += new System.EventHandler(this.cmdRunNow_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(12, 66);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(75, 23);
			this.cmdCancel.TabIndex = 6;
			this.cmdCancel.Text = "Storno";
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// formRecordNow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(336, 101);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdRunNow);
			this.Controls.Add(this.cmdBrowse);
			this.Controls.Add(this.txtFilename);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cmbChannel);
			this.Controls.Add(this.lblChannel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "formRecordNow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Okamžité nahrávání";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblChannel;
		private System.Windows.Forms.ComboBox cmbChannel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtFilename;
		private System.Windows.Forms.Button cmdBrowse;
		private System.Windows.Forms.Button cmdRunNow;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.SaveFileDialog dialog;
	}
}