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
            this.playersNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.playersLabel = new System.Windows.Forms.Label();
            this.playersTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.playersNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // playersNumberNumericUpDown
            // 
            this.playersNumberNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
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
            this.playersNumberNumericUpDown.ValueChanged += new System.EventHandler(this.PlayersNumberChanged);
            // 
            // playersLabel
            // 
            this.playersLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.playersLabel.AutoSize = true;
            this.playersLabel.Location = new System.Drawing.Point(16, 19);
            this.playersLabel.Name = "playersLabel";
            this.playersLabel.Size = new System.Drawing.Size(127, 13);
            this.playersLabel.TabIndex = 3;
            this.playersLabel.Text = "Number of human players";
            // 
            // playersTableLayoutPanel
            // 
            this.playersTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.playersTableLayoutPanel.ColumnCount = 1;
            this.playersTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.playersTableLayoutPanel.Location = new System.Drawing.Point(19, 45);
            this.playersTableLayoutPanel.Name = "playersTableLayoutPanel";
            this.playersTableLayoutPanel.RowCount = 7;
            this.playersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.playersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.playersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.playersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.playersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.playersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.playersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.playersTableLayoutPanel.Size = new System.Drawing.Size(343, 206);
            this.playersTableLayoutPanel.TabIndex = 5;
            // 
            // PlayersSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.playersTableLayoutPanel);
            this.Controls.Add(this.playersNumberNumericUpDown);
            this.Controls.Add(this.playersLabel);
            this.Name = "PlayersSettingsControl";
            this.Size = new System.Drawing.Size(391, 266);
            ((System.ComponentModel.ISupportInitialize)(this.playersNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NumericUpDown playersNumberNumericUpDown;
        private System.Windows.Forms.Label playersLabel;
        private System.Windows.Forms.TableLayoutPanel playersTableLayoutPanel;
    }
}
