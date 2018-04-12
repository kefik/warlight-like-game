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
            this.aiPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.aiPlayersNumberLabel = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.myPlayerPanel = new System.Windows.Forms.Panel();
            this.fogOfWarCheckBox = new System.Windows.Forms.CheckBox();
            this.myPlayerLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.separatorLabel = new System.Windows.Forms.Label();
            this.aiPlayerSettingsControl = new WinformsUI.HelperControls.AiPlayerSettingsControl();
            this.mapSettingsControl = new WinformsUI.HelperControls.MapSettingsControl();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // aiPlayersNumberNumericUpDown
            // 
            this.aiPlayersNumberNumericUpDown.Location = new System.Drawing.Point(124, 35);
            this.aiPlayersNumberNumericUpDown.Name = "aiPlayersNumberNumericUpDown";
            this.aiPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.aiPlayersNumberNumericUpDown.TabIndex = 2;
            this.aiPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.aiPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.PlayersNumberChanged);
            // 
            // aiPlayersNumberLabel
            // 
            this.aiPlayersNumberLabel.AutoSize = true;
            this.aiPlayersNumberLabel.Location = new System.Drawing.Point(4, 37);
            this.aiPlayersNumberLabel.Name = "aiPlayersNumberLabel";
            this.aiPlayersNumberLabel.Size = new System.Drawing.Size(105, 13);
            this.aiPlayersNumberLabel.TabIndex = 3;
            this.aiPlayersNumberLabel.Text = "Number of AI players";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(184, 163);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 4;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.Start);
            // 
            // myPlayerPanel
            // 
            this.myPlayerPanel.Location = new System.Drawing.Point(0, 105);
            this.myPlayerPanel.Name = "myPlayerPanel";
            this.myPlayerPanel.Size = new System.Drawing.Size(251, 33);
            this.myPlayerPanel.TabIndex = 5;
            // 
            // fogOfWarCheckBox
            // 
            this.fogOfWarCheckBox.AutoSize = true;
            this.fogOfWarCheckBox.Checked = true;
            this.fogOfWarCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fogOfWarCheckBox.Location = new System.Drawing.Point(187, 11);
            this.fogOfWarCheckBox.Name = "fogOfWarCheckBox";
            this.fogOfWarCheckBox.Size = new System.Drawing.Size(76, 17);
            this.fogOfWarCheckBox.TabIndex = 6;
            this.fogOfWarCheckBox.Text = "Fog of war";
            this.fogOfWarCheckBox.UseVisualStyleBackColor = true;
            // 
            // myPlayerLabel
            // 
            this.myPlayerLabel.AutoSize = true;
            this.myPlayerLabel.Location = new System.Drawing.Point(3, 86);
            this.myPlayerLabel.Name = "myPlayerLabel";
            this.myPlayerLabel.Size = new System.Drawing.Size(52, 13);
            this.myPlayerLabel.TabIndex = 7;
            this.myPlayerLabel.Text = "My player";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Enemy AI players";
            // 
            // separatorLabel
            // 
            this.separatorLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.separatorLabel.Location = new System.Drawing.Point(1, 64);
            this.separatorLabel.Name = "separatorLabel";
            this.separatorLabel.Size = new System.Drawing.Size(262, 2);
            this.separatorLabel.TabIndex = 9;
            // 
            // aiPlayerSettingsControl
            // 
            this.aiPlayerSettingsControl.AutoSize = true;
            this.aiPlayerSettingsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.aiPlayerSettingsControl.Location = new System.Drawing.Point(0, 160);
            this.aiPlayerSettingsControl.Name = "aiPlayerSettingsControl";
            this.aiPlayerSettingsControl.PlayersLimit = 0;
            this.aiPlayerSettingsControl.Size = new System.Drawing.Size(0, 0);
            this.aiPlayerSettingsControl.TabIndex = 1;
            // 
            // mapSettingsControl
            // 
            this.mapSettingsControl.Location = new System.Drawing.Point(0, 3);
            this.mapSettingsControl.Name = "mapSettingsControl";
            this.mapSettingsControl.Size = new System.Drawing.Size(181, 31);
            this.mapSettingsControl.TabIndex = 0;
            // 
            // SingleplayerNewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.separatorLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.myPlayerLabel);
            this.Controls.Add(this.fogOfWarCheckBox);
            this.Controls.Add(this.myPlayerPanel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.aiPlayersNumberLabel);
            this.Controls.Add(this.aiPlayersNumberNumericUpDown);
            this.Controls.Add(this.aiPlayerSettingsControl);
            this.Controls.Add(this.mapSettingsControl);
            this.Name = "SingleplayerNewGameSettingsControl";
            this.Size = new System.Drawing.Size(276, 473);
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelperControls.MapSettingsControl mapSettingsControl;
        private HelperControls.AiPlayerSettingsControl aiPlayerSettingsControl;
        private System.Windows.Forms.NumericUpDown aiPlayersNumberNumericUpDown;
        private System.Windows.Forms.Label aiPlayersNumberLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Panel myPlayerPanel;
        private System.Windows.Forms.CheckBox fogOfWarCheckBox;
        private System.Windows.Forms.Label myPlayerLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label separatorLabel;
    }
}
