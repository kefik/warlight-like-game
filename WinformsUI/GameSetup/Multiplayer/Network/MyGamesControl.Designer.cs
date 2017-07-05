namespace WinformsUI.GameSetup.Multiplayer.Network
{
    partial class MyGamesControl
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
            this.multiDayGamesLabel = new System.Windows.Forms.Label();
            this.openButton = new System.Windows.Forms.Button();
            this.oneDayListBox = new System.Windows.Forms.ListBox();
            this.multiDayListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // oneDayGameslabel
            // 
            this.oneDayGameslabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.oneDayGameslabel.AutoSize = true;
            this.oneDayGameslabel.Location = new System.Drawing.Point(3, 22);
            this.oneDayGameslabel.Name = "oneDayGameslabel";
            this.oneDayGameslabel.Size = new System.Drawing.Size(81, 13);
            this.oneDayGameslabel.TabIndex = 3;
            this.oneDayGameslabel.Text = "One day games";
            // 
            // multiDayGamesLabel
            // 
            this.multiDayGamesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.multiDayGamesLabel.AutoSize = true;
            this.multiDayGamesLabel.Location = new System.Drawing.Point(1, 138);
            this.multiDayGamesLabel.Name = "multiDayGamesLabel";
            this.multiDayGamesLabel.Size = new System.Drawing.Size(83, 13);
            this.multiDayGamesLabel.TabIndex = 4;
            this.multiDayGamesLabel.Text = "Multi-day games";
            // 
            // openButton
            // 
            this.openButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.openButton.Location = new System.Drawing.Point(373, 246);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 5;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            // 
            // oneDayListBox
            // 
            this.oneDayListBox.FormattingEnabled = true;
            this.oneDayListBox.Items.AddRange(new object[] {
            "Human: 3; AI: 4; Map: World; Game-started: 26.09.2017 15:40"});
            this.oneDayListBox.Location = new System.Drawing.Point(17, 38);
            this.oneDayListBox.Name = "oneDayListBox";
            this.oneDayListBox.Size = new System.Drawing.Size(431, 82);
            this.oneDayListBox.TabIndex = 6;
            // 
            // multiDayListBox
            // 
            this.multiDayListBox.FormattingEnabled = true;
            this.multiDayListBox.Items.AddRange(new object[] {
            "Human: 3; AI: 4; Map: World; Round-started: 26.09.2017 15:40; Game-started: 26.09" +
                ".2017 15:10"});
            this.multiDayListBox.Location = new System.Drawing.Point(17, 154);
            this.multiDayListBox.Name = "multiDayListBox";
            this.multiDayListBox.Size = new System.Drawing.Size(431, 82);
            this.multiDayListBox.TabIndex = 7;
            // 
            // MyGamesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.multiDayListBox);
            this.Controls.Add(this.oneDayListBox);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.multiDayGamesLabel);
            this.Controls.Add(this.oneDayGameslabel);
            this.Name = "MyGamesControl";
            this.Size = new System.Drawing.Size(464, 280);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label oneDayGameslabel;
        private System.Windows.Forms.Label multiDayGamesLabel;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.ListBox oneDayListBox;
        private System.Windows.Forms.ListBox multiDayListBox;
    }
}
