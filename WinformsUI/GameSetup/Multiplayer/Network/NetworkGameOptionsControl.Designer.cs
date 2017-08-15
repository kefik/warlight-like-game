namespace WinformsUI.GameSetup.Multiplayer.Network
{
    partial class NetworkGameOptionsControl
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
            this.gameOptionsTabControl = new System.Windows.Forms.TabControl();
            this.myGamesTabPage = new System.Windows.Forms.TabPage();
            this.openedGamesTabPage = new System.Windows.Forms.TabPage();
            this.newGameTabPage = new System.Windows.Forms.TabPage();
            this.networkNewGameSettingsControl = new WinformsUI.GameSetup.Multiplayer.Network.NetworkNewGameSettingsControl();
            this.myGamesControl = new WinformsUI.GameSetup.Multiplayer.Network.MyGamesControl();
            this.gameOptionsTabControl.SuspendLayout();
            this.myGamesTabPage.SuspendLayout();
            this.newGameTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameOptionsTabControl
            // 
            this.gameOptionsTabControl.Controls.Add(this.myGamesTabPage);
            this.gameOptionsTabControl.Controls.Add(this.openedGamesTabPage);
            this.gameOptionsTabControl.Controls.Add(this.newGameTabPage);
            this.gameOptionsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameOptionsTabControl.Location = new System.Drawing.Point(0, 0);
            this.gameOptionsTabControl.Name = "gameOptionsTabControl";
            this.gameOptionsTabControl.SelectedIndex = 0;
            this.gameOptionsTabControl.Size = new System.Drawing.Size(540, 301);
            this.gameOptionsTabControl.TabIndex = 3;
            // 
            // myGamesTabPage
            // 
            this.myGamesTabPage.Controls.Add(this.myGamesControl);
            this.myGamesTabPage.Location = new System.Drawing.Point(4, 22);
            this.myGamesTabPage.Name = "myGamesTabPage";
            this.myGamesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.myGamesTabPage.Size = new System.Drawing.Size(532, 275);
            this.myGamesTabPage.TabIndex = 0;
            this.myGamesTabPage.Text = "My games";
            this.myGamesTabPage.UseVisualStyleBackColor = true;
            // 
            // openedGamesTabPage
            // 
            this.openedGamesTabPage.Location = new System.Drawing.Point(4, 22);
            this.openedGamesTabPage.Name = "openedGamesTabPage";
            this.openedGamesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.openedGamesTabPage.Size = new System.Drawing.Size(532, 275);
            this.openedGamesTabPage.TabIndex = 1;
            this.openedGamesTabPage.Text = "Opened games";
            this.openedGamesTabPage.UseVisualStyleBackColor = true;
            // 
            // newGameTabPage
            // 
            this.newGameTabPage.AutoScroll = true;
            this.newGameTabPage.Controls.Add(this.networkNewGameSettingsControl);
            this.newGameTabPage.Location = new System.Drawing.Point(4, 22);
            this.newGameTabPage.Name = "newGameTabPage";
            this.newGameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.newGameTabPage.Size = new System.Drawing.Size(532, 275);
            this.newGameTabPage.TabIndex = 2;
            this.newGameTabPage.Text = "New game";
            this.newGameTabPage.UseVisualStyleBackColor = true;
            // 
            // networkNewGameSettingsControl
            // 
            this.networkNewGameSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.networkNewGameSettingsControl.Location = new System.Drawing.Point(3, 3);
            this.networkNewGameSettingsControl.Name = "networkNewGameSettingsControl";
            this.networkNewGameSettingsControl.Size = new System.Drawing.Size(472, 650);
            this.networkNewGameSettingsControl.TabIndex = 0;
            // 
            // myGamesControl
            // 
            this.myGamesControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myGamesControl.Location = new System.Drawing.Point(3, 3);
            this.myGamesControl.Name = "myGamesControl";
            this.myGamesControl.Size = new System.Drawing.Size(526, 277);
            this.myGamesControl.TabIndex = 0;
            // 
            // NetworkGameOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gameOptionsTabControl);
            this.Name = "NetworkGameOptionsControl";
            this.Size = new System.Drawing.Size(540, 301);
            this.gameOptionsTabControl.ResumeLayout(false);
            this.myGamesTabPage.ResumeLayout(false);
            this.newGameTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl gameOptionsTabControl;
        private System.Windows.Forms.TabPage myGamesTabPage;
        private System.Windows.Forms.TabPage openedGamesTabPage;
        private System.Windows.Forms.TabPage newGameTabPage;
        private NetworkNewGameSettingsControl networkNewGameSettingsControl;
        private MyGamesControl myGamesControl;
    }
}
