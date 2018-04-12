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
            this.generateRestrictionsCheckBox = new System.Windows.Forms.CheckBox();
            this.separatorLabel = new System.Windows.Forms.Label();
            this.simulatorBotSettingsControl = new WinformsUI.HelperControls.SimulatorBotSettingsControl();
            this.mapSettingsControl = new WinformsUI.HelperControls.MapSettingsControl();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // fogOfWarCheckBox
            // 
            this.fogOfWarCheckBox.AutoSize = true;
            this.fogOfWarCheckBox.Checked = true;
            this.fogOfWarCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fogOfWarCheckBox.Location = new System.Drawing.Point(185, 5);
            this.fogOfWarCheckBox.Name = "fogOfWarCheckBox";
            this.fogOfWarCheckBox.Size = new System.Drawing.Size(76, 17);
            this.fogOfWarCheckBox.TabIndex = 13;
            this.fogOfWarCheckBox.Text = "Fog of war";
            this.fogOfWarCheckBox.UseVisualStyleBackColor = true;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(227, 106);
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
            this.aiPlayersNumberLabel.Location = new System.Drawing.Point(3, 41);
            this.aiPlayersNumberLabel.Name = "aiPlayersNumberLabel";
            this.aiPlayersNumberLabel.Size = new System.Drawing.Size(105, 13);
            this.aiPlayersNumberLabel.TabIndex = 10;
            this.aiPlayersNumberLabel.Text = "Number of AI players";
            // 
            // aiPlayersNumberNumericUpDown
            // 
            this.aiPlayersNumberNumericUpDown.Location = new System.Drawing.Point(122, 35);
            this.aiPlayersNumberNumericUpDown.Name = "aiPlayersNumberNumericUpDown";
            this.aiPlayersNumberNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.aiPlayersNumberNumericUpDown.TabIndex = 9;
            this.aiPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.aiPlayersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.PlayersNumberChanged);
            // 
            // generateRestrictionsCheckBox
            // 
            this.generateRestrictionsCheckBox.AutoSize = true;
            this.generateRestrictionsCheckBox.Checked = true;
            this.generateRestrictionsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.generateRestrictionsCheckBox.Location = new System.Drawing.Point(184, 20);
            this.generateRestrictionsCheckBox.Name = "generateRestrictionsCheckBox";
            this.generateRestrictionsCheckBox.Size = new System.Drawing.Size(123, 17);
            this.generateRestrictionsCheckBox.TabIndex = 14;
            this.generateRestrictionsCheckBox.Text = "Generate restrictions";
            this.generateRestrictionsCheckBox.UseVisualStyleBackColor = true;
            // 
            // separatorLabel
            // 
            this.separatorLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.separatorLabel.Location = new System.Drawing.Point(6, 71);
            this.separatorLabel.Name = "separatorLabel";
            this.separatorLabel.Size = new System.Drawing.Size(296, 2);
            this.separatorLabel.TabIndex = 16;
            // 
            // simulatorBotSettingsControl
            // 
            this.simulatorBotSettingsControl.AutoSize = true;
            this.simulatorBotSettingsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.simulatorBotSettingsControl.Location = new System.Drawing.Point(4, 100);
            this.simulatorBotSettingsControl.Name = "simulatorBotSettingsControl";
            this.simulatorBotSettingsControl.PlayersLimit = 0;
            this.simulatorBotSettingsControl.Size = new System.Drawing.Size(0, 0);
            this.simulatorBotSettingsControl.TabIndex = 15;
            // 
            // mapSettingsControl
            // 
            this.mapSettingsControl.Location = new System.Drawing.Point(3, 3);
            this.mapSettingsControl.Name = "mapSettingsControl";
            this.mapSettingsControl.Size = new System.Drawing.Size(178, 31);
            this.mapSettingsControl.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "AI players";
            // 
            // SimulatorNewGameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.separatorLabel);
            this.Controls.Add(this.simulatorBotSettingsControl);
            this.Controls.Add(this.generateRestrictionsCheckBox);
            this.Controls.Add(this.fogOfWarCheckBox);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.aiPlayersNumberLabel);
            this.Controls.Add(this.aiPlayersNumberNumericUpDown);
            this.Controls.Add(this.mapSettingsControl);
            this.Name = "SimulatorNewGameSettingsControl";
            this.Size = new System.Drawing.Size(309, 429);
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox fogOfWarCheckBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label aiPlayersNumberLabel;
        private System.Windows.Forms.NumericUpDown aiPlayersNumberNumericUpDown;
        private HelperControls.MapSettingsControl mapSettingsControl;
        private System.Windows.Forms.CheckBox generateRestrictionsCheckBox;
        private HelperControls.SimulatorBotSettingsControl simulatorBotSettingsControl;
        private System.Windows.Forms.Label separatorLabel;
        private System.Windows.Forms.Label label1;
    }
}
