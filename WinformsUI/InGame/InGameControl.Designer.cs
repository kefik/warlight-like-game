namespace WinformsUI.InGame
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
            this.gameMenuPanel = new System.Windows.Forms.Panel();
            this.menuButton = new System.Windows.Forms.Button();
            this.gameStateMenuPanel = new System.Windows.Forms.Panel();
            this.mapHandlerControl = new WinformsUI.InGame.MapHandlerControl();
            this.gameMenuPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameMenuPanel
            // 
            this.gameMenuPanel.Controls.Add(this.menuButton);
            this.gameMenuPanel.Controls.Add(this.gameStateMenuPanel);
            this.gameMenuPanel.Location = new System.Drawing.Point(3, 3);
            this.gameMenuPanel.Name = "gameMenuPanel";
            this.gameMenuPanel.Size = new System.Drawing.Size(180, 436);
            this.gameMenuPanel.TabIndex = 0;
            // 
            // menuButton
            // 
            this.menuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.menuButton.Enabled = false;
            this.menuButton.Location = new System.Drawing.Point(3, 378);
            this.menuButton.Name = "menuButton";
            this.menuButton.Size = new System.Drawing.Size(174, 52);
            this.menuButton.TabIndex = 1;
            this.menuButton.Text = "Menu";
            this.menuButton.UseVisualStyleBackColor = true;
            // 
            // gameStateMenuPanel
            // 
            this.gameStateMenuPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gameStateMenuPanel.Location = new System.Drawing.Point(3, 3);
            this.gameStateMenuPanel.Name = "gameStateMenuPanel";
            this.gameStateMenuPanel.Size = new System.Drawing.Size(177, 358);
            this.gameStateMenuPanel.TabIndex = 0;
            // 
            // mapHandlerControl
            // 
            this.mapHandlerControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapHandlerControl.Location = new System.Drawing.Point(189, 3);
            this.mapHandlerControl.Name = "mapHandlerControl";
            this.mapHandlerControl.Size = new System.Drawing.Size(594, 430);
            this.mapHandlerControl.TabIndex = 1;
            // 
            // InGameControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapHandlerControl);
            this.Controls.Add(this.gameMenuPanel);
            this.Name = "InGameControl";
            this.Size = new System.Drawing.Size(786, 436);
            this.gameMenuPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel gameMenuPanel;
        private System.Windows.Forms.Panel gameStateMenuPanel;
        private System.Windows.Forms.Button menuButton;
        private MapHandlerControl mapHandlerControl;
    }
}
