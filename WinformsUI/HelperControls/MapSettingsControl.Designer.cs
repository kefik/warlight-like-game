namespace WinformsUI.HelperControls
{
    partial class MapSettingsControl
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
            this.mapLabel = new System.Windows.Forms.Label();
            this.mapComboBox = new System.Windows.Forms.ComboBox();
            this.mapPlayersLimitLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mapLabel
            // 
            this.mapLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mapLabel.AutoSize = true;
            this.mapLabel.Location = new System.Drawing.Point(22, 25);
            this.mapLabel.Name = "mapLabel";
            this.mapLabel.Size = new System.Drawing.Size(28, 13);
            this.mapLabel.TabIndex = 0;
            this.mapLabel.Text = "Map";
            // 
            // mapComboBox
            // 
            this.mapComboBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mapComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mapComboBox.FormattingEnabled = true;
            this.mapComboBox.Items.AddRange(new object[] {
            "World"});
            this.mapComboBox.Location = new System.Drawing.Point(152, 25);
            this.mapComboBox.Name = "mapComboBox";
            this.mapComboBox.Size = new System.Drawing.Size(121, 21);
            this.mapComboBox.TabIndex = 1;
            this.mapComboBox.SelectedIndexChanged += new System.EventHandler(this.MapChosen);
            // 
            // mapPlayersLimitLabel
            // 
            this.mapPlayersLimitLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mapPlayersLimitLabel.AutoSize = true;
            this.mapPlayersLimitLabel.Location = new System.Drawing.Point(288, 28);
            this.mapPlayersLimitLabel.Name = "mapPlayersLimitLabel";
            this.mapPlayersLimitLabel.Size = new System.Drawing.Size(13, 13);
            this.mapPlayersLimitLabel.TabIndex = 2;
            this.mapPlayersLimitLabel.Text = "0";
            // 
            // MapSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapPlayersLimitLabel);
            this.Controls.Add(this.mapComboBox);
            this.Controls.Add(this.mapLabel);
            this.Name = "MapSettingsControl";
            this.Size = new System.Drawing.Size(317, 71);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mapLabel;
        private System.Windows.Forms.ComboBox mapComboBox;
        private System.Windows.Forms.Label mapPlayersLimitLabel;
    }
}
