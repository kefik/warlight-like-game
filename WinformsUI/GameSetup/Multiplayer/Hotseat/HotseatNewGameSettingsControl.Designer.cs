namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    partial class HotseatNewGameSettingsControl
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
            this.humanPlayersLabel = new System.Windows.Forms.Label();
            this.humanPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.humanPlayerSettingsControl = new WinformsUI.HelperControls.HumanPlayerSettingsControl();
            this.startButton = new System.Windows.Forms.Button();
            this.myUserPanel = new System.Windows.Forms.Panel();
            this.aiPlayerSettingsControl = new WinformsUI.HelperControls.AiPlayerSettingsControl();
            this.aiPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.aiPlayersNumberLabel = new System.Windows.Forms.Label();
            this.fogOfWarCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.humanPlayersNumberNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // mapSettingsControl
            // 
            this.mapSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapSettingsControl.Location = new System.Drawing.Point(18, 3);
            this.mapSettingsControl.Name = "mapSettingsControl";
            this.mapSettingsControl.Size = new System.Drawing.Size(346, 31);
            this.mapSettingsControl.TabIndex = 1;
            // 
            // humanPlayersLabel
            // 
            this.humanPlayersLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.humanPlayersLabel.AutoSize = true;
            this.humanPlayersLabel.Location = new System.Drawing.Point(15, 123);
            this.humanPlayersLabel.Name = "humanPlayersLabel";
            this.humanPlayersLabel.Size = new System.Drawing.Size(127, 13);
            this.humanPlayersLabel.TabIndex = 7;
            this.humanPlayersLabel.Text = "Number of human players";
            // 
            // humanPlayersNumberNumericUpDown
            // 
            this.humanPlayersNumberNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.humanPlayersNumberNumericUpDown.Location = new System.Drawing.Point(316, 121);
            this.humanPlayersNumberNumericUpDown.Name = "humanPlayersNumberNumericUpDown";
            this.humanPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.humanPlayersNumberNumericUpDown.TabIndex = 6;
            this.humanPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.humanPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.OnNumberOfHumanPlayersChanged);
            // 
            // humanPlayerSettingsControl
            // 
            this.humanPlayerSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.humanPlayerSettingsControl.Location = new System.Drawing.Point(6, 147);
            this.humanPlayerSettingsControl.Name = "humanPlayerSettingsControl";
            this.humanPlayerSettingsControl.PlayersLimit = 0;
            this.humanPlayerSettingsControl.Size = new System.Drawing.Size(371, 234);
            this.humanPlayerSettingsControl.TabIndex = 8;
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startButton.Location = new System.Drawing.Point(302, 665);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 10;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.Start);
            // 
            // myUserPanel
            // 
            this.myUserPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myUserPanel.Location = new System.Drawing.Point(3, 69);
            this.myUserPanel.Name = "myUserPanel";
            this.myUserPanel.Size = new System.Drawing.Size(371, 37);
            this.myUserPanel.TabIndex = 11;
            // 
            // aiPlayerSettingsControl
            // 
            this.aiPlayerSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.aiPlayerSettingsControl.Location = new System.Drawing.Point(9, 427);
            this.aiPlayerSettingsControl.Name = "aiPlayerSettingsControl";
            this.aiPlayerSettingsControl.PlayersLimit = 0;
            this.aiPlayerSettingsControl.Size = new System.Drawing.Size(371, 232);
            this.aiPlayerSettingsControl.TabIndex = 9;
            // 
            // aiPlayersNumberNumericUpDown
            // 
            this.aiPlayersNumberNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.aiPlayersNumberNumericUpDown.Location = new System.Drawing.Point(319, 401);
            this.aiPlayersNumberNumericUpDown.Name = "aiPlayersNumberNumericUpDown";
            this.aiPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.aiPlayersNumberNumericUpDown.TabIndex = 4;
            this.aiPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.aiPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.OnNumberOfAiPlayersChanged);
            // 
            // aiPlayersNumberLabel
            // 
            this.aiPlayersNumberLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aiPlayersNumberLabel.AutoSize = true;
            this.aiPlayersNumberLabel.Location = new System.Drawing.Point(18, 403);
            this.aiPlayersNumberLabel.Name = "aiPlayersNumberLabel";
            this.aiPlayersNumberLabel.Size = new System.Drawing.Size(104, 13);
            this.aiPlayersNumberLabel.TabIndex = 5;
            this.aiPlayersNumberLabel.Text = "Number of Ai players";
            // 
            // fogOfWarCheckBox
            // 
            this.fogOfWarCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fogOfWarCheckBox.AutoSize = true;
            this.fogOfWarCheckBox.Checked = true;
            this.fogOfWarCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fogOfWarCheckBox.Location = new System.Drawing.Point(298, 40);
            this.fogOfWarCheckBox.Name = "fogOfWarCheckBox";
            this.fogOfWarCheckBox.Size = new System.Drawing.Size(76, 17);
            this.fogOfWarCheckBox.TabIndex = 21;
            this.fogOfWarCheckBox.Text = "Fog of war";
            this.fogOfWarCheckBox.UseVisualStyleBackColor = true;
            // 
            // HotseatNewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fogOfWarCheckBox);
            this.Controls.Add(this.myUserPanel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.aiPlayerSettingsControl);
            this.Controls.Add(this.humanPlayerSettingsControl);
            this.Controls.Add(this.humanPlayersLabel);
            this.Controls.Add(this.humanPlayersNumberNumericUpDown);
            this.Controls.Add(this.aiPlayersNumberLabel);
            this.Controls.Add(this.aiPlayersNumberNumericUpDown);
            this.Controls.Add(this.mapSettingsControl);
            this.Name = "HotseatNewGameSettingsControl";
            this.Size = new System.Drawing.Size(380, 691);
            ((System.ComponentModel.ISupportInitialize)(this.humanPlayersNumberNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelperControls.MapSettingsControl mapSettingsControl;
        private System.Windows.Forms.Label humanPlayersLabel;
        private System.Windows.Forms.NumericUpDown humanPlayersNumberNumericUpDown;
        private HelperControls.HumanPlayerSettingsControl humanPlayerSettingsControl;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Panel myUserPanel;
        private HelperControls.AiPlayerSettingsControl aiPlayerSettingsControl;
        private System.Windows.Forms.NumericUpDown aiPlayersNumberNumericUpDown;
        private System.Windows.Forms.Label aiPlayersNumberLabel;
        private System.Windows.Forms.CheckBox fogOfWarCheckBox;
    }
}
