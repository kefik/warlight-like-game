namespace WinformsUI.HelperControls
{
    partial class AISettingsControl
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
            this.aiPlayersLabel = new System.Windows.Forms.Label();
            this.aiPlayersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.aiPlayersFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // aiPlayersLabel
            // 
            this.aiPlayersLabel.AutoSize = true;
            this.aiPlayersLabel.Location = new System.Drawing.Point(16, 23);
            this.aiPlayersLabel.Name = "aiPlayersLabel";
            this.aiPlayersLabel.Size = new System.Drawing.Size(105, 13);
            this.aiPlayersLabel.TabIndex = 0;
            this.aiPlayersLabel.Text = "Number of AI players";
            // 
            // aiPlayersNumberNumericUpDown
            // 
            this.aiPlayersNumberNumericUpDown.Location = new System.Drawing.Point(315, 23);
            this.aiPlayersNumberNumericUpDown.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.aiPlayersNumberNumericUpDown.Name = "aiPlayersNumberNumericUpDown";
            this.aiPlayersNumberNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.aiPlayersNumberNumericUpDown.TabIndex = 1;
            this.aiPlayersNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // aiPlayersFlowLayoutPanel
            // 
            this.aiPlayersFlowLayoutPanel.AutoScroll = true;
            this.aiPlayersFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.aiPlayersFlowLayoutPanel.Location = new System.Drawing.Point(19, 59);
            this.aiPlayersFlowLayoutPanel.Name = "aiPlayersFlowLayoutPanel";
            this.aiPlayersFlowLayoutPanel.Size = new System.Drawing.Size(343, 192);
            this.aiPlayersFlowLayoutPanel.TabIndex = 2;
            this.aiPlayersFlowLayoutPanel.WrapContents = false;
            // 
            // AISettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aiPlayersFlowLayoutPanel);
            this.Controls.Add(this.aiPlayersNumberNumericUpDown);
            this.Controls.Add(this.aiPlayersLabel);
            this.Name = "AISettingsControl";
            this.Size = new System.Drawing.Size(380, 268);
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label aiPlayersLabel;
        private System.Windows.Forms.NumericUpDown aiPlayersNumberNumericUpDown;
        private System.Windows.Forms.FlowLayoutPanel aiPlayersFlowLayoutPanel;
    }
}
