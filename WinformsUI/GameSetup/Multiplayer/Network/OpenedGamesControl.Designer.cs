namespace WinformsUI.GameSetup.Multiplayer.Network
{
    partial class OpenedGamesControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.oneDayGameslabel = new System.Windows.Forms.Label();
            this.oneDayListBox = new System.Windows.Forms.ListBox();
            this.multiDayGamesLabel = new System.Windows.Forms.Label();
            this.multiDayListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // oneDayGameslabel
            // 
            this.oneDayGameslabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.oneDayGameslabel.AutoSize = true;
            this.oneDayGameslabel.Location = new System.Drawing.Point(3, 9);
            this.oneDayGameslabel.Name = "oneDayGameslabel";
            this.oneDayGameslabel.Size = new System.Drawing.Size(81, 13);
            this.oneDayGameslabel.TabIndex = 8;
            this.oneDayGameslabel.Text = "One day games";
            // 
            // oneDayListBox
            // 
            this.oneDayListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.oneDayListBox.FormattingEnabled = true;
            this.oneDayListBox.Location = new System.Drawing.Point(14, 25);
            this.oneDayListBox.Name = "oneDayListBox";
            this.oneDayListBox.Size = new System.Drawing.Size(446, 82);
            this.oneDayListBox.TabIndex = 10;
            // 
            // multiDayGamesLabel
            // 
            this.multiDayGamesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.multiDayGamesLabel.AutoSize = true;
            this.multiDayGamesLabel.Location = new System.Drawing.Point(3, 110);
            this.multiDayGamesLabel.Name = "multiDayGamesLabel";
            this.multiDayGamesLabel.Size = new System.Drawing.Size(83, 13);
            this.multiDayGamesLabel.TabIndex = 9;
            this.multiDayGamesLabel.Text = "Multi-day games";
            // 
            // multiDayListBox
            // 
            this.multiDayListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.multiDayListBox.FormattingEnabled = true;
            this.multiDayListBox.Location = new System.Drawing.Point(14, 134);
            this.multiDayListBox.Name = "multiDayListBox";
            this.multiDayListBox.Size = new System.Drawing.Size(446, 82);
            this.multiDayListBox.TabIndex = 11;
            // 
            // OpenedGamesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.oneDayGameslabel);
            this.Controls.Add(this.oneDayListBox);
            this.Controls.Add(this.multiDayGamesLabel);
            this.Controls.Add(this.multiDayListBox);
            this.Name = "OpenedGamesControl";
            this.Size = new System.Drawing.Size(476, 238);
            this.Load += new System.EventHandler(this.ControlLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label oneDayGameslabel;
        private System.Windows.Forms.ListBox oneDayListBox;
        private System.Windows.Forms.Label multiDayGamesLabel;
        private System.Windows.Forms.ListBox multiDayListBox;
    }
}
