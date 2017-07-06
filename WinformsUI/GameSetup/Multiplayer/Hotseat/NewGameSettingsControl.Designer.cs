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
            this.mapSettingsPanel = new System.Windows.Forms.Panel();
            this.aiPlayersSettingsPanel = new System.Windows.Forms.Panel();
            this.aiSettingsControl1 = new WinformsUI.HelperControls.AISettingsControl();
            this.playersSettingsPanel = new System.Windows.Forms.Panel();
            this.playersSettingsControl = new WinformsUI.HelperControls.PlayersSettingsControl();
            this.mapSettingsControl1 = new WinformsUI.HelperControls.MapSettingsControl();
            this.mapSettingsPanel.SuspendLayout();
            this.aiPlayersSettingsPanel.SuspendLayout();
            this.playersSettingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mapSettingsPanel
            // 
            this.mapSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.mapSettingsPanel.AutoScroll = true;
            this.mapSettingsPanel.Controls.Add(this.mapSettingsControl1);
            this.mapSettingsPanel.Location = new System.Drawing.Point(4, 4);
            this.mapSettingsPanel.Name = "mapSettingsPanel";
            this.mapSettingsPanel.Size = new System.Drawing.Size(564, 48);
            this.mapSettingsPanel.TabIndex = 0;
            // 
            // aiPlayersSettingsPanel
            // 
            this.aiPlayersSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.aiPlayersSettingsPanel.AutoScroll = true;
            this.aiPlayersSettingsPanel.Controls.Add(this.aiSettingsControl1);
            this.aiPlayersSettingsPanel.Location = new System.Drawing.Point(4, 58);
            this.aiPlayersSettingsPanel.Name = "aiPlayersSettingsPanel";
            this.aiPlayersSettingsPanel.Size = new System.Drawing.Size(564, 122);
            this.aiPlayersSettingsPanel.TabIndex = 1;
            // 
            // aiSettingsControl1
            // 
            this.aiSettingsControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.aiSettingsControl1.Location = new System.Drawing.Point(0, 0);
            this.aiSettingsControl1.Name = "aiSettingsControl1";
            this.aiSettingsControl1.Size = new System.Drawing.Size(547, 268);
            this.aiSettingsControl1.TabIndex = 0;
            // 
            // playersSettingsPanel
            // 
            this.playersSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.playersSettingsPanel.AutoScroll = true;
            this.playersSettingsPanel.Controls.Add(this.playersSettingsControl);
            this.playersSettingsPanel.Location = new System.Drawing.Point(4, 186);
            this.playersSettingsPanel.Name = "playersSettingsPanel";
            this.playersSettingsPanel.Size = new System.Drawing.Size(564, 137);
            this.playersSettingsPanel.TabIndex = 2;
            // 
            // playersSettingsControl
            // 
            this.playersSettingsControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.playersSettingsControl.Location = new System.Drawing.Point(0, 0);
            this.playersSettingsControl.Name = "playersSettingsControl";
            this.playersSettingsControl.Size = new System.Drawing.Size(547, 266);
            this.playersSettingsControl.TabIndex = 0;
            // 
            // mapSettingsControl1
            // 
            this.mapSettingsControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.mapSettingsControl1.Location = new System.Drawing.Point(0, 0);
            this.mapSettingsControl1.Name = "mapSettingsControl1";
            this.mapSettingsControl1.Size = new System.Drawing.Size(564, 35);
            this.mapSettingsControl1.TabIndex = 0;
            // 
            // NewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.playersSettingsPanel);
            this.Controls.Add(this.aiPlayersSettingsPanel);
            this.Controls.Add(this.mapSettingsPanel);
            this.Name = "NewGameSettingsControl";
            this.Size = new System.Drawing.Size(571, 326);
            this.mapSettingsPanel.ResumeLayout(false);
            this.aiPlayersSettingsPanel.ResumeLayout(false);
            this.playersSettingsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mapSettingsPanel;
        private System.Windows.Forms.Panel aiPlayersSettingsPanel;
        private System.Windows.Forms.Panel playersSettingsPanel;
        private HelperControls.AISettingsControl aiSettingsControl1;
        private HelperControls.PlayersSettingsControl playersSettingsControl;
        private HelperControls.MapSettingsControl mapSettingsControl1;
    }
}
