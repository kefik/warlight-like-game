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
            this.aiPlayersTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.aiPlayerControl1 = new WinformsUI.HelperControls.AIPlayerControl();
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).BeginInit();
            this.aiPlayersTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // aiPlayersLabel
            // 
            this.aiPlayersLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aiPlayersLabel.AutoSize = true;
            this.aiPlayersLabel.Location = new System.Drawing.Point(16, 23);
            this.aiPlayersLabel.Name = "aiPlayersLabel";
            this.aiPlayersLabel.Size = new System.Drawing.Size(105, 13);
            this.aiPlayersLabel.TabIndex = 0;
            this.aiPlayersLabel.Text = "Number of AI players";
            // 
            // aiPlayersNumberNumericUpDown
            // 
            this.aiPlayersNumberNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
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
            // aiPlayersTableLayoutPanel
            // 
            this.aiPlayersTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.aiPlayersTableLayoutPanel.ColumnCount = 1;
            this.aiPlayersTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.aiPlayersTableLayoutPanel.Controls.Add(this.aiPlayerControl1, 0, 1);
            this.aiPlayersTableLayoutPanel.Location = new System.Drawing.Point(19, 49);
            this.aiPlayersTableLayoutPanel.Name = "aiPlayersTableLayoutPanel";
            this.aiPlayersTableLayoutPanel.RowCount = 7;
            this.aiPlayersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.aiPlayersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.aiPlayersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.aiPlayersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.aiPlayersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.aiPlayersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.aiPlayersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.aiPlayersTableLayoutPanel.Size = new System.Drawing.Size(343, 206);
            this.aiPlayersTableLayoutPanel.TabIndex = 2;
            // 
            // aiPlayerControl1
            // 
            this.aiPlayerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aiPlayerControl1.Location = new System.Drawing.Point(3, 3);
            this.aiPlayerControl1.MinimumSize = new System.Drawing.Size(272, 27);
            this.aiPlayerControl1.Name = "aiPlayerControl1";
            this.aiPlayerControl1.Size = new System.Drawing.Size(337, 28);
            this.aiPlayerControl1.TabIndex = 0;
            // 
            // AISettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aiPlayersTableLayoutPanel);
            this.Controls.Add(this.aiPlayersNumberNumericUpDown);
            this.Controls.Add(this.aiPlayersLabel);
            this.Name = "AISettingsControl";
            this.Size = new System.Drawing.Size(380, 268);
            ((System.ComponentModel.ISupportInitialize)(this.aiPlayersNumberNumericUpDown)).EndInit();
            this.aiPlayersTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label aiPlayersLabel;
        private System.Windows.Forms.NumericUpDown aiPlayersNumberNumericUpDown;
        private System.Windows.Forms.TableLayoutPanel aiPlayersTableLayoutPanel;
        private AIPlayerControl aiPlayerControl1;
    }
}
