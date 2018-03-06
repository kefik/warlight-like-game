namespace WinformsUI.GameSetup.Simulator
{
    partial class SimulatorNewGameSettingsControl
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
            this.fogOfWarCheckBox = new System.Windows.Forms.CheckBox();
            this.startButton = new System.Windows.Forms.Button();
            this.aiPlayersNumberLabel = new System.Windows.Forms.Label();
            this.aiPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.aiPlayerSettingsControl = new WinformsUI.HelperControls.AiPlayerSettingsControl();
            this.mapSettingsControl = new WinformsUI.HelperControls.MapSettingsControl();
            this.generateRestrictionsCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // fogOfWarCheckBox
            // 
            this.fogOfWarCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.fogOfWarCheckBox.AutoSize = true;
            this.fogOfWarCheckBox.Checked = true;
            this.fogOfWarCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fogOfWarCheckBox.Location = new System.Drawing.Point(249, 84);
            this.fogOfWarCheckBox.Name = "fogOfWarCheckBox";
            this.fogOfWarCheckBox.Size = new System.Drawing.Size(76, 17);
            this.fogOfWarCheckBox.TabIndex = 13;
            this.fogOfWarCheckBox.Text = "Fog of war";
            this.fogOfWarCheckBox.UseVisualStyleBackColor = true;
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startButton.Location = new System.Drawing.Point(301, 399);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 11;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.Start);
            // 
            // aiPlayersNumberLabel
            // 
            this.aiPlayersNumberLabel.AutoSize = true;
            this.aiPlayersNumberLabel.Location = new System.Drawing.Point(26, 43);
            this.aiPlayersNumberLabel.Name = "aiPlayersNumberLabel";
            this.aiPlayersNumberLabel.Size = new System.Drawing.Size(104, 13);
            this.aiPlayersNumberLabel.TabIndex = 10;
            this.aiPlayersNumberLabel.Text = "Number of Ai players";
            // 
            // aiPlayersNumberNumericUpDown
            // 
            this.aiPlayersNumberNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.aiPlayersNumberNumericUpDown.Location = new System.Drawing.Point(314, 41);
            this.aiPlayersNumberNumericUpDown.Name = "aiPlayersNumberNumericUpDown";
            this.aiPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.aiPlayersNumberNumericUpDown.TabIndex = 9;
            this.aiPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.aiPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.PlayersNumberChanged);
            // 
            // aiPlayerSettingsControl
            // 
            this.aiPlayerSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.aiPlayerSettingsControl.Location = new System.Drawing.Point(5, 107);
            this.aiPlayerSettingsControl.Name = "aiPlayerSettingsControl";
            this.aiPlayerSettingsControl.PlayersLimit = 0;
            this.aiPlayerSettingsControl.Size = new System.Drawing.Size(371, 286);
            this.aiPlayerSettingsControl.TabIndex = 8;
            // 
            // mapSettingsControl
            // 
            this.mapSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapSettingsControl.Location = new System.Drawing.Point(26, 6);
            this.mapSettingsControl.Name = "mapSettingsControl";
            this.mapSettingsControl.Size = new System.Drawing.Size(333, 31);
            this.mapSettingsControl.TabIndex = 7;
            // 
            // generateRestrictionsCheckBox
            // 
            this.generateRestrictionsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.generateRestrictionsCheckBox.AutoSize = true;
            this.generateRestrictionsCheckBox.Checked = true;
            this.generateRestrictionsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.generateRestrictionsCheckBox.Location = new System.Drawing.Point(249, 67);
            this.generateRestrictionsCheckBox.Name = "generateRestrictionsCheckBox";
            this.generateRestrictionsCheckBox.Size = new System.Drawing.Size(123, 17);
            this.generateRestrictionsCheckBox.TabIndex = 14;
            this.generateRestrictionsCheckBox.Text = "Generate restrictions";
            this.generateRestrictionsCheckBox.UseVisualStyleBackColor = true;
            // 
            // SimulatorNewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.generateRestrictionsCheckBox);
            this.Controls.Add(this.fogOfWarCheckBox);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.aiPlayersNumberLabel);
            this.Controls.Add(this.aiPlayersNumberNumericUpDown);
            this.Controls.Add(this.aiPlayerSettingsControl);
            this.Controls.Add(this.mapSettingsControl);
            this.Name = "SimulatorNewGameSettingsControl";
            this.Size = new System.Drawing.Size(381, 429);
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox fogOfWarCheckBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label aiPlayersNumberLabel;
        private System.Windows.Forms.NumericUpDown aiPlayersNumberNumericUpDown;
        private HelperControls.AiPlayerSettingsControl aiPlayerSettingsControl;
        private HelperControls.MapSettingsControl mapSettingsControl;
        private System.Windows.Forms.CheckBox generateRestrictionsCheckBox;
    }
}
