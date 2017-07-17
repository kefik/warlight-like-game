namespace WinformsUI.GameSetup.Multiplayer.Network
{
    partial class NetworkNewGameSettingsControl
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
            this.myUserPanel = new System.Windows.Forms.Panel();
            this.humanPlayerSettingsControl = new WinformsUI.HelperControls.HumanPlayerSettingsControl();
            this.humanPlayersLabel = new System.Windows.Forms.Label();
            this.humanPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.aiPlayerSettingsControl = new WinformsUI.HelperControls.AIPlayerSettingsControl();
            this.aiPlayersNumberLabel = new System.Windows.Forms.Label();
            this.aiPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.createButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.humanPlayersNumberNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // mapSettingsControl
            // 
            this.mapSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapSettingsControl.Location = new System.Drawing.Point(14, 3);
            this.mapSettingsControl.Name = "mapSettingsControl";
            this.mapSettingsControl.Size = new System.Drawing.Size(346, 31);
            this.mapSettingsControl.TabIndex = 2;
            // 
            // myUserPanel
            // 
            this.myUserPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myUserPanel.Location = new System.Drawing.Point(6, 45);
            this.myUserPanel.Name = "myUserPanel";
            this.myUserPanel.Size = new System.Drawing.Size(371, 37);
            this.myUserPanel.TabIndex = 12;
            // 
            // humanPlayerSettingsControl
            // 
            this.humanPlayerSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.humanPlayerSettingsControl.Location = new System.Drawing.Point(3, 114);
            this.humanPlayerSettingsControl.Name = "humanPlayerSettingsControl";
            this.humanPlayerSettingsControl.PlayersLimit = 0;
            this.humanPlayerSettingsControl.Size = new System.Drawing.Size(371, 234);
            this.humanPlayerSettingsControl.TabIndex = 15;
            // 
            // humanPlayersLabel
            // 
            this.humanPlayersLabel.AutoSize = true;
            this.humanPlayersLabel.Location = new System.Drawing.Point(3, 88);
            this.humanPlayersLabel.Name = "humanPlayersLabel";
            this.humanPlayersLabel.Size = new System.Drawing.Size(127, 13);
            this.humanPlayersLabel.TabIndex = 14;
            this.humanPlayersLabel.Text = "Number of human players";
            // 
            // humanPlayersNumberNumericUpDown
            // 
            this.humanPlayersNumberNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.humanPlayersNumberNumericUpDown.Location = new System.Drawing.Point(322, 88);
            this.humanPlayersNumberNumericUpDown.Name = "humanPlayersNumberNumericUpDown";
            this.humanPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.humanPlayersNumberNumericUpDown.TabIndex = 13;
            this.humanPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.humanPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.OnNumberOfHumanPlayersChanged);
            // 
            // aiPlayerSettingsControl
            // 
            this.aiPlayerSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aiPlayerSettingsControl.Location = new System.Drawing.Point(3, 382);
            this.aiPlayerSettingsControl.Name = "aiPlayerSettingsControl";
            this.aiPlayerSettingsControl.PlayersLimit = 0;
            this.aiPlayerSettingsControl.Size = new System.Drawing.Size(371, 232);
            this.aiPlayerSettingsControl.TabIndex = 18;
            // 
            // aiPlayersNumberLabel
            // 
            this.aiPlayersNumberLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.aiPlayersNumberLabel.AutoSize = true;
            this.aiPlayersNumberLabel.Location = new System.Drawing.Point(3, 358);
            this.aiPlayersNumberLabel.Name = "aiPlayersNumberLabel";
            this.aiPlayersNumberLabel.Size = new System.Drawing.Size(105, 13);
            this.aiPlayersNumberLabel.TabIndex = 17;
            this.aiPlayersNumberLabel.Text = "Number of AI players";
            // 
            // aiPlayersNumberNumericUpDown
            // 
            this.aiPlayersNumberNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.aiPlayersNumberNumericUpDown.Location = new System.Drawing.Point(322, 356);
            this.aiPlayersNumberNumericUpDown.Name = "aiPlayersNumberNumericUpDown";
            this.aiPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.aiPlayersNumberNumericUpDown.TabIndex = 16;
            this.aiPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.aiPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.OnNumberOfAIPlayersChanged);
            // 
            // createButton
            // 
            this.createButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.createButton.Location = new System.Drawing.Point(305, 624);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(75, 23);
            this.createButton.TabIndex = 19;
            this.createButton.Text = "Create";
            this.createButton.UseVisualStyleBackColor = true;
            // 
            // NetworkNewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.aiPlayerSettingsControl);
            this.Controls.Add(this.aiPlayersNumberLabel);
            this.Controls.Add(this.aiPlayersNumberNumericUpDown);
            this.Controls.Add(this.humanPlayerSettingsControl);
            this.Controls.Add(this.humanPlayersLabel);
            this.Controls.Add(this.humanPlayersNumberNumericUpDown);
            this.Controls.Add(this.myUserPanel);
            this.Controls.Add(this.mapSettingsControl);
            this.Name = "NetworkNewGameSettingsControl";
            this.Size = new System.Drawing.Size(383, 650);
            ((System.ComponentModel.ISupportInitialize)(this.humanPlayersNumberNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelperControls.MapSettingsControl mapSettingsControl;
        private System.Windows.Forms.Panel myUserPanel;
        private HelperControls.HumanPlayerSettingsControl humanPlayerSettingsControl;
        private System.Windows.Forms.Label humanPlayersLabel;
        private System.Windows.Forms.NumericUpDown humanPlayersNumberNumericUpDown;
        private HelperControls.AIPlayerSettingsControl aiPlayerSettingsControl;
        private System.Windows.Forms.Label aiPlayersNumberLabel;
        private System.Windows.Forms.NumericUpDown aiPlayersNumberNumericUpDown;
        private System.Windows.Forms.Button createButton;
    }
}
