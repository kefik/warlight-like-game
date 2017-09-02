namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    partial class HotseatGameOptionsControl
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
            this.loadGameTabPage = new System.Windows.Forms.TabPage();
            this.hotseatLoadGamesControl = new WinformsUI.GameSetup.Multiplayer.Hotseat.HotseatLoadGamesControl();
            this.newGameTabPage = new System.Windows.Forms.TabPage();
            this.hotseatNewGameSettingsControl = new WinformsUI.GameSetup.Multiplayer.Hotseat.HotseatNewGameSettingsControl();
            this.gameOptionsTabControl = new System.Windows.Forms.TabControl();
            this.loadGameTabPage.SuspendLayout();
            this.newGameTabPage.SuspendLayout();
            this.gameOptionsTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadGameTabPage
            // 
            this.loadGameTabPage.Controls.Add(this.hotseatLoadGamesControl);
            this.loadGameTabPage.Location = new System.Drawing.Point(4, 22);
            this.loadGameTabPage.Name = "loadGameTabPage";
            this.loadGameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.loadGameTabPage.Size = new System.Drawing.Size(485, 253);
            this.loadGameTabPage.TabIndex = 2;
            this.loadGameTabPage.Text = "Load game";
            this.loadGameTabPage.UseVisualStyleBackColor = true;
            // 
            // hotseatLoadGamesControl
            // 
            this.hotseatLoadGamesControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hotseatLoadGamesControl.Location = new System.Drawing.Point(3, 3);
            this.hotseatLoadGamesControl.Name = "hotseatLoadGamesControl";
            this.hotseatLoadGamesControl.Size = new System.Drawing.Size(479, 244);
            this.hotseatLoadGamesControl.TabIndex = 0;
            // 
            // newGameTabPage
            // 
            this.newGameTabPage.AutoScroll = true;
            this.newGameTabPage.Controls.Add(this.hotseatNewGameSettingsControl);
            this.newGameTabPage.Location = new System.Drawing.Point(4, 22);
            this.newGameTabPage.Name = "newGameTabPage";
            this.newGameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.newGameTabPage.Size = new System.Drawing.Size(485, 253);
            this.newGameTabPage.TabIndex = 1;
            this.newGameTabPage.Text = "New game";
            this.newGameTabPage.UseVisualStyleBackColor = true;
            // 
            // hotseatNewGameSettingsControl
            // 
            this.hotseatNewGameSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hotseatNewGameSettingsControl.Location = new System.Drawing.Point(3, 6);
            this.hotseatNewGameSettingsControl.Name = "hotseatNewGameSettingsControl";
            this.hotseatNewGameSettingsControl.Size = new System.Drawing.Size(439, 644);
            this.hotseatNewGameSettingsControl.TabIndex = 0;
            // 
            // gameOptionsTabControl
            // 
            this.gameOptionsTabControl.Controls.Add(this.newGameTabPage);
            this.gameOptionsTabControl.Controls.Add(this.loadGameTabPage);
            this.gameOptionsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameOptionsTabControl.Location = new System.Drawing.Point(0, 0);
            this.gameOptionsTabControl.Name = "gameOptionsTabControl";
            this.gameOptionsTabControl.SelectedIndex = 0;
            this.gameOptionsTabControl.Size = new System.Drawing.Size(493, 279);
            this.gameOptionsTabControl.TabIndex = 2;
            // 
            // HotseatGameOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gameOptionsTabControl);
            this.Name = "HotseatGameOptionsControl";
            this.Size = new System.Drawing.Size(493, 279);
            this.loadGameTabPage.ResumeLayout(false);
            this.newGameTabPage.ResumeLayout(false);
            this.gameOptionsTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage loadGameTabPage;
        private System.Windows.Forms.TabPage newGameTabPage;
        private System.Windows.Forms.TabControl gameOptionsTabControl;
        private HotseatLoadGamesControl hotseatLoadGamesControl;
        private HotseatNewGameSettingsControl hotseatNewGameSettingsControl;
    }
}
