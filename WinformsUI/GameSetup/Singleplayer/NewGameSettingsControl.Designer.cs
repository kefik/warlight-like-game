namespace WinformsUI.GameSetup.Singleplayer
{
    partial class NewGameSettingsControl
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
            this.settingsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.mapSettingsControl1 = new WinformsUI.HelperControls.MapSettingsControl();
            this.aiSettingsControl1 = new WinformsUI.HelperControls.AISettingsControl();
            this.settingsTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsTableLayoutPanel
            // 
            this.settingsTableLayoutPanel.AutoScroll = true;
            this.settingsTableLayoutPanel.ColumnCount = 1;
            this.settingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.settingsTableLayoutPanel.Controls.Add(this.mapSettingsControl1, 0, 0);
            this.settingsTableLayoutPanel.Controls.Add(this.aiSettingsControl1, 0, 1);
            this.settingsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.settingsTableLayoutPanel.Name = "settingsTableLayoutPanel";
            this.settingsTableLayoutPanel.RowCount = 2;
            this.settingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.03448F));
            this.settingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.96552F));
            this.settingsTableLayoutPanel.Size = new System.Drawing.Size(495, 290);
            this.settingsTableLayoutPanel.TabIndex = 0;
            // 
            // mapSettingsControl1
            // 
            this.mapSettingsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapSettingsControl1.Location = new System.Drawing.Point(3, 3);
            this.mapSettingsControl1.Name = "mapSettingsControl1";
            this.mapSettingsControl1.Size = new System.Drawing.Size(489, 26);
            this.mapSettingsControl1.TabIndex = 0;
            // 
            // aiSettingsControl1
            // 
            this.aiSettingsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aiSettingsControl1.AutoScroll = true;
            this.aiSettingsControl1.Location = new System.Drawing.Point(3, 35);
            this.aiSettingsControl1.Name = "aiSettingsControl1";
            this.aiSettingsControl1.Size = new System.Drawing.Size(489, 252);
            this.aiSettingsControl1.TabIndex = 1;
            // 
            // NewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.settingsTableLayoutPanel);
            this.Name = "NewGameSettingsControl";
            this.Size = new System.Drawing.Size(495, 290);
            this.settingsTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel settingsTableLayoutPanel;
        private HelperControls.MapSettingsControl mapSettingsControl1;
        private HelperControls.AISettingsControl aiSettingsControl1;
    }
}
