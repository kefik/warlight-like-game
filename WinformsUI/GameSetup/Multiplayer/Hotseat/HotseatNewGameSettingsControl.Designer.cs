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
            this.humanPlayersNumberLabel = new System.Windows.Forms.Label();
            this.humanPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.startButton = new System.Windows.Forms.Button();
            this.myUserPanel = new System.Windows.Forms.Panel();
            this.aiPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.aiPlayersNumberLabel = new System.Windows.Forms.Label();
            this.fogOfWarCheckBox = new System.Windows.Forms.CheckBox();
            this.humanPlayersLabel = new System.Windows.Forms.Label();
            this.separatorLabel = new System.Windows.Forms.Label();
            this.myPlayerLabel = new System.Windows.Forms.Label();
            this.aiPlayersPanel = new System.Windows.Forms.Panel();
            this.aiPlayersLabel = new System.Windows.Forms.Label();
            this.aiPlayerSettingsControl = new WinformsUI.HelperControls.AiPlayerSettingsControl();
            this.humanPlayerSettingsControl = new WinformsUI.HelperControls.HumanPlayerSettingsControl();
            this.mapSettingsControl = new WinformsUI.HelperControls.MapSettingsControl();
            ((System.ComponentModel.ISupportInitialize)(this.humanPlayersNumberNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).BeginInit();
            this.aiPlayersPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // humanPlayersNumberLabel
            // 
            this.humanPlayersNumberLabel.AutoSize = true;
            this.humanPlayersNumberLabel.Location = new System.Drawing.Point(0, 37);
            this.humanPlayersNumberLabel.Name = "humanPlayersNumberLabel";
            this.humanPlayersNumberLabel.Size = new System.Drawing.Size(127, 13);
            this.humanPlayersNumberLabel.TabIndex = 7;
            this.humanPlayersNumberLabel.Text = "Number of human players";
            // 
            // humanPlayersNumberNumericUpDown
            // 
            this.humanPlayersNumberNumericUpDown.Location = new System.Drawing.Point(128, 35);
            this.humanPlayersNumberNumericUpDown.Name = "humanPlayersNumberNumericUpDown";
            this.humanPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.humanPlayersNumberNumericUpDown.TabIndex = 6;
            this.humanPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.humanPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.OnNumberOfHumanPlayersChanged);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(184, 24);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 10;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.Start);
            // 
            // myUserPanel
            // 
            this.myUserPanel.Location = new System.Drawing.Point(0, 128);
            this.myUserPanel.Name = "myUserPanel";
            this.myUserPanel.Size = new System.Drawing.Size(255, 37);
            this.myUserPanel.TabIndex = 11;
            // 
            // aiPlayersNumberNumericUpDown
            // 
            this.aiPlayersNumberNumericUpDown.Location = new System.Drawing.Point(128, 60);
            this.aiPlayersNumberNumericUpDown.Name = "aiPlayersNumberNumericUpDown";
            this.aiPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.aiPlayersNumberNumericUpDown.TabIndex = 4;
            this.aiPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.aiPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.OnNumberOfAiPlayersChanged);
            // 
            // aiPlayersNumberLabel
            // 
            this.aiPlayersNumberLabel.AutoSize = true;
            this.aiPlayersNumberLabel.Location = new System.Drawing.Point(0, 62);
            this.aiPlayersNumberLabel.Name = "aiPlayersNumberLabel";
            this.aiPlayersNumberLabel.Size = new System.Drawing.Size(104, 13);
            this.aiPlayersNumberLabel.TabIndex = 5;
            this.aiPlayersNumberLabel.Text = "Number of Ai players";
            // 
            // fogOfWarCheckBox
            // 
            this.fogOfWarCheckBox.AutoSize = true;
            this.fogOfWarCheckBox.Checked = true;
            this.fogOfWarCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fogOfWarCheckBox.Location = new System.Drawing.Point(182, 11);
            this.fogOfWarCheckBox.Name = "fogOfWarCheckBox";
            this.fogOfWarCheckBox.Size = new System.Drawing.Size(76, 17);
            this.fogOfWarCheckBox.TabIndex = 21;
            this.fogOfWarCheckBox.Text = "Fog of war";
            this.fogOfWarCheckBox.UseVisualStyleBackColor = true;
            // 
            // humanPlayersLabel
            // 
            this.humanPlayersLabel.AutoSize = true;
            this.humanPlayersLabel.Location = new System.Drawing.Point(3, 180);
            this.humanPlayersLabel.Name = "humanPlayersLabel";
            this.humanPlayersLabel.Size = new System.Drawing.Size(104, 13);
            this.humanPlayersLabel.TabIndex = 22;
            this.humanPlayersLabel.Text = "Other human players";
            // 
            // separatorLabel
            // 
            this.separatorLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.separatorLabel.Location = new System.Drawing.Point(3, 99);
            this.separatorLabel.Name = "separatorLabel";
            this.separatorLabel.Size = new System.Drawing.Size(262, 2);
            this.separatorLabel.TabIndex = 23;
            // 
            // myPlayerLabel
            // 
            this.myPlayerLabel.AutoSize = true;
            this.myPlayerLabel.Location = new System.Drawing.Point(3, 112);
            this.myPlayerLabel.Name = "myPlayerLabel";
            this.myPlayerLabel.Size = new System.Drawing.Size(52, 13);
            this.myPlayerLabel.TabIndex = 8;
            this.myPlayerLabel.Text = "My player";
            // 
            // aiPlayersPanel
            // 
            this.aiPlayersPanel.AutoSize = true;
            this.aiPlayersPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.aiPlayersPanel.Controls.Add(this.aiPlayersLabel);
            this.aiPlayersPanel.Controls.Add(this.aiPlayerSettingsControl);
            this.aiPlayersPanel.Controls.Add(this.startButton);
            this.aiPlayersPanel.Location = new System.Drawing.Point(0, 196);
            this.aiPlayersPanel.Name = "aiPlayersPanel";
            this.aiPlayersPanel.Size = new System.Drawing.Size(262, 50);
            this.aiPlayersPanel.TabIndex = 24;
            // 
            // aiPlayersLabel
            // 
            this.aiPlayersLabel.AutoSize = true;
            this.aiPlayersLabel.Location = new System.Drawing.Point(4, 9);
            this.aiPlayersLabel.Name = "aiPlayersLabel";
            this.aiPlayersLabel.Size = new System.Drawing.Size(53, 13);
            this.aiPlayersLabel.TabIndex = 25;
            this.aiPlayersLabel.Text = "AI players";
            // 
            // aiPlayerSettingsControl
            // 
            this.aiPlayerSettingsControl.AutoSize = true;
            this.aiPlayerSettingsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.aiPlayerSettingsControl.Location = new System.Drawing.Point(3, 24);
            this.aiPlayerSettingsControl.Name = "aiPlayerSettingsControl";
            this.aiPlayerSettingsControl.PlayersLimit = 0;
            this.aiPlayerSettingsControl.Size = new System.Drawing.Size(0, 0);
            this.aiPlayerSettingsControl.TabIndex = 9;
            // 
            // humanPlayerSettingsControl
            // 
            this.humanPlayerSettingsControl.AutoSize = true;
            this.humanPlayerSettingsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.humanPlayerSettingsControl.Location = new System.Drawing.Point(0, 199);
            this.humanPlayerSettingsControl.Name = "humanPlayerSettingsControl";
            this.humanPlayerSettingsControl.PlayersLimit = 0;
            this.humanPlayerSettingsControl.Size = new System.Drawing.Size(0, 0);
            this.humanPlayerSettingsControl.TabIndex = 8;
            // 
            // mapSettingsControl
            // 
            this.mapSettingsControl.Location = new System.Drawing.Point(0, 3);
            this.mapSettingsControl.Name = "mapSettingsControl";
            this.mapSettingsControl.Size = new System.Drawing.Size(181, 31);
            this.mapSettingsControl.TabIndex = 1;
            // 
            // HotseatNewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aiPlayersPanel);
            this.Controls.Add(this.myPlayerLabel);
            this.Controls.Add(this.separatorLabel);
            this.Controls.Add(this.humanPlayersLabel);
            this.Controls.Add(this.fogOfWarCheckBox);
            this.Controls.Add(this.myUserPanel);
            this.Controls.Add(this.humanPlayerSettingsControl);
            this.Controls.Add(this.humanPlayersNumberLabel);
            this.Controls.Add(this.humanPlayersNumberNumericUpDown);
            this.Controls.Add(this.aiPlayersNumberLabel);
            this.Controls.Add(this.aiPlayersNumberNumericUpDown);
            this.Controls.Add(this.mapSettingsControl);
            this.Name = "HotseatNewGameSettingsControl";
            this.Size = new System.Drawing.Size(269, 473);
            ((System.ComponentModel.ISupportInitialize)(this.humanPlayersNumberNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).EndInit();
            this.aiPlayersPanel.ResumeLayout(false);
            this.aiPlayersPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelperControls.MapSettingsControl mapSettingsControl;
        private System.Windows.Forms.Label humanPlayersNumberLabel;
        private System.Windows.Forms.NumericUpDown humanPlayersNumberNumericUpDown;
        private HelperControls.HumanPlayerSettingsControl humanPlayerSettingsControl;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Panel myUserPanel;
        private HelperControls.AiPlayerSettingsControl aiPlayerSettingsControl;
        private System.Windows.Forms.NumericUpDown aiPlayersNumberNumericUpDown;
        private System.Windows.Forms.Label aiPlayersNumberLabel;
        private System.Windows.Forms.CheckBox fogOfWarCheckBox;
        private System.Windows.Forms.Label humanPlayersLabel;
        private System.Windows.Forms.Label separatorLabel;
        private System.Windows.Forms.Label myPlayerLabel;
        private System.Windows.Forms.Panel aiPlayersPanel;
        private System.Windows.Forms.Label aiPlayersLabel;
    }
}
