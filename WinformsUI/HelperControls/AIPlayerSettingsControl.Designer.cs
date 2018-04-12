namespace WinformsUI.HelperControls
{
    partial class AiPlayerSettingsControl
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
            this.playersTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // playersTableLayoutPanel
            // 
            this.playersTableLayoutPanel.AutoSize = true;
            this.playersTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.playersTableLayoutPanel.ColumnCount = 1;
            this.playersTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.playersTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playersTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.playersTableLayoutPanel.Name = "playersTableLayoutPanel";
            this.playersTableLayoutPanel.RowCount = 1;
            this.playersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.playersTableLayoutPanel.Size = new System.Drawing.Size(380, 268);
            this.playersTableLayoutPanel.TabIndex = 2;
            // 
            // AiPlayerSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.playersTableLayoutPanel);
            this.Name = "AiPlayerSettingsControl";
            this.Size = new System.Drawing.Size(380, 268);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel playersTableLayoutPanel;
    }
}
