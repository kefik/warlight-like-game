namespace WinformsUI.GameSetup.Simulator
{
    partial class SimulatorGameOptionsControl
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
            this.simulatorNewGameSettingsControl1 = new WinformsUI.GameSetup.Simulator.SimulatorNewGameSettingsControl();
            this.SuspendLayout();
            // 
            // simulatorNewGameSettingsControl1
            // 
            this.simulatorNewGameSettingsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.simulatorNewGameSettingsControl1.AutoScroll = true;
            this.simulatorNewGameSettingsControl1.Location = new System.Drawing.Point(3, 3);
            this.simulatorNewGameSettingsControl1.Name = "simulatorNewGameSettingsControl1";
            this.simulatorNewGameSettingsControl1.Size = new System.Drawing.Size(370, 429);
            this.simulatorNewGameSettingsControl1.TabIndex = 0;
            // 
            // SimulatorGameOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.simulatorNewGameSettingsControl1);
            this.Name = "SimulatorGameOptionsControl";
            this.Size = new System.Drawing.Size(370, 410);
            this.ResumeLayout(false);

        }

        #endregion

        private SimulatorNewGameSettingsControl simulatorNewGameSettingsControl1;
    }
}
