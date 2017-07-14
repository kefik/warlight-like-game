namespace WinformsUI.GameSetup.Singleplayer
{
    partial class SingleplayerNewGameSettingsControl
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
            this.mapSettingsControl = new WinformsUI.HelperControls.MapSettingsControl();
            this.aiPlayerSettingsControl = new WinformsUI.HelperControls.AIPlayerSettingsControl();
            this.aiPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.aiPlayersNumberLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // mapSettingsControl
            // 
            this.mapSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapSettingsControl.Location = new System.Drawing.Point(24, 3);
            this.mapSettingsControl.Name = "mapSettingsControl";
            this.mapSettingsControl.Size = new System.Drawing.Size(333, 31);
            this.mapSettingsControl.TabIndex = 0;
            // 
            // aiPlayerSettingsControl
            // 
            this.aiPlayerSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.aiPlayerSettingsControl.Location = new System.Drawing.Point(3, 67);
            this.aiPlayerSettingsControl.Name = "aiPlayerSettingsControl";
            this.aiPlayerSettingsControl.PlayersLimit = 0;
            this.aiPlayerSettingsControl.Size = new System.Drawing.Size(371, 231);
            this.aiPlayerSettingsControl.TabIndex = 1;
            // 
            // aiPlayersNumberNumericUpDown
            // 
            this.aiPlayersNumberNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.aiPlayersNumberNumericUpDown.Location = new System.Drawing.Point(312, 38);
            this.aiPlayersNumberNumericUpDown.Name = "aiPlayersNumberNumericUpDown";
            this.aiPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.aiPlayersNumberNumericUpDown.TabIndex = 2;
            this.aiPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.aiPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.PlayersNumberChanged);
            // 
            // aiPlayersNumberLabel
            // 
            this.aiPlayersNumberLabel.AutoSize = true;
            this.aiPlayersNumberLabel.Location = new System.Drawing.Point(24, 40);
            this.aiPlayersNumberLabel.Name = "aiPlayersNumberLabel";
            this.aiPlayersNumberLabel.Size = new System.Drawing.Size(108, 13);
            this.aiPlayersNumberLabel.TabIndex = 3;
            this.aiPlayersNumberLabel.Text = "Number of AI players:";
            // 
            // SingleplayerNewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aiPlayersNumberLabel);
            this.Controls.Add(this.aiPlayersNumberNumericUpDown);
            this.Controls.Add(this.aiPlayerSettingsControl);
            this.Controls.Add(this.mapSettingsControl);
            this.Name = "SingleplayerNewGameSettingsControl";
            this.Size = new System.Drawing.Size(377, 298);
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelperControls.MapSettingsControl mapSettingsControl;
        private HelperControls.AIPlayerSettingsControl aiPlayerSettingsControl;
        private System.Windows.Forms.NumericUpDown aiPlayersNumberNumericUpDown;
        private System.Windows.Forms.Label aiPlayersNumberLabel;
    }
}
