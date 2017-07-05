namespace WinformsUI.GameSetup.Multiplayer.Hotseat
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
            this.aiSettingsPanel = new System.Windows.Forms.Panel();
            this.playersSettingsPanel = new System.Windows.Forms.Panel();
            this.mapSettingsPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // aiSettingsPanel
            // 
            this.aiSettingsPanel.Location = new System.Drawing.Point(26, 21);
            this.aiSettingsPanel.Name = "aiSettingsPanel";
            this.aiSettingsPanel.Size = new System.Drawing.Size(356, 115);
            this.aiSettingsPanel.TabIndex = 0;
            // 
            // playersSettingsPanel
            // 
            this.playersSettingsPanel.Location = new System.Drawing.Point(26, 160);
            this.playersSettingsPanel.Name = "playersSettingsPanel";
            this.playersSettingsPanel.Size = new System.Drawing.Size(356, 115);
            this.playersSettingsPanel.TabIndex = 1;
            // 
            // mapSettingsPanel
            // 
            this.mapSettingsPanel.Location = new System.Drawing.Point(408, 21);
            this.mapSettingsPanel.Name = "mapSettingsPanel";
            this.mapSettingsPanel.Size = new System.Drawing.Size(128, 254);
            this.mapSettingsPanel.TabIndex = 2;
            // 
            // NewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapSettingsPanel);
            this.Controls.Add(this.playersSettingsPanel);
            this.Controls.Add(this.aiSettingsPanel);
            this.Name = "NewGameSettingsControl";
            this.Size = new System.Drawing.Size(571, 326);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel aiSettingsPanel;
        private System.Windows.Forms.Panel playersSettingsPanel;
        private System.Windows.Forms.Panel mapSettingsPanel;
    }
}
