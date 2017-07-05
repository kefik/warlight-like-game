namespace WinformsUI.HelperControls
{
    partial class PlayersSettingsControl
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
            this.playersFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.playersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.playersLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.playersNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // playersFlowLayoutPanel
            // 
            this.playersFlowLayoutPanel.AutoScroll = true;
            this.playersFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.playersFlowLayoutPanel.Location = new System.Drawing.Point(19, 55);
            this.playersFlowLayoutPanel.Name = "playersFlowLayoutPanel";
            this.playersFlowLayoutPanel.Size = new System.Drawing.Size(343, 192);
            this.playersFlowLayoutPanel.TabIndex = 5;
            this.playersFlowLayoutPanel.WrapContents = false;
            // 
            // playersNumberNumericUpDown
            // 
            this.playersNumberNumericUpDown.Location = new System.Drawing.Point(315, 19);
            this.playersNumberNumericUpDown.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.playersNumberNumericUpDown.Name = "playersNumberNumericUpDown";
            this.playersNumberNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.playersNumberNumericUpDown.TabIndex = 4;
            this.playersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // playersLabel
            // 
            this.playersLabel.AutoSize = true;
            this.playersLabel.Location = new System.Drawing.Point(16, 19);
            this.playersLabel.Name = "playersLabel";
            this.playersLabel.Size = new System.Drawing.Size(127, 13);
            this.playersLabel.TabIndex = 3;
            this.playersLabel.Text = "Number of human players";
            // 
            // PlayersSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.playersFlowLayoutPanel);
            this.Controls.Add(this.playersNumberNumericUpDown);
            this.Controls.Add(this.playersLabel);
            this.Name = "PlayersSettingsControl";
            this.Size = new System.Drawing.Size(380, 266);
            ((System.ComponentModel.ISupportInitialize)(this.playersNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel playersFlowLayoutPanel;
        private System.Windows.Forms.NumericUpDown playersNumberNumericUpDown;
        private System.Windows.Forms.Label playersLabel;
    }
}
