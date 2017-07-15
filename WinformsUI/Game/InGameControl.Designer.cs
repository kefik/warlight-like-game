namespace WinformsUI.Game
{
    partial class InGameControl
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
            this.gamePanel = new System.Windows.Forms.Panel();
            this.gameMenuPanel = new System.Windows.Forms.Panel();
            this.gameMapPictureBox = new System.Windows.Forms.PictureBox();
            this.gamePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gameMapPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // gamePanel
            // 
            this.gamePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamePanel.Controls.Add(this.gameMapPictureBox);
            this.gamePanel.Location = new System.Drawing.Point(189, 0);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.Size = new System.Drawing.Size(462, 436);
            this.gamePanel.TabIndex = 0;
            // 
            // gameMenuPanel
            // 
            this.gameMenuPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gameMenuPanel.Location = new System.Drawing.Point(0, 0);
            this.gameMenuPanel.Name = "gameMenuPanel";
            this.gameMenuPanel.Size = new System.Drawing.Size(183, 436);
            this.gameMenuPanel.TabIndex = 0;
            // 
            // gameMapPictureBox
            // 
            this.gameMapPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameMapPictureBox.Location = new System.Drawing.Point(0, 0);
            this.gameMapPictureBox.Name = "gameMapPictureBox";
            this.gameMapPictureBox.Size = new System.Drawing.Size(462, 436);
            this.gameMapPictureBox.TabIndex = 0;
            this.gameMapPictureBox.TabStop = false;
            // 
            // InGameControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gameMenuPanel);
            this.Controls.Add(this.gamePanel);
            this.Name = "InGameControl";
            this.Size = new System.Drawing.Size(651, 436);
            this.gamePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gameMapPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel gamePanel;
        private System.Windows.Forms.Panel gameMenuPanel;
        private System.Windows.Forms.PictureBox gameMapPictureBox;
    }
}
