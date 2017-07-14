namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    partial class GameOptionsControl
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
            this.loadGamesControl = new WinformsUI.GameSetup.Multiplayer.Hotseat.LoadGamesControl();
            this.newGameTabPage = new System.Windows.Forms.TabPage();
            this.continueTabPage = new System.Windows.Forms.TabPage();
            this.gameOptionsTabControl = new System.Windows.Forms.TabControl();
            this.loadGameTabPage.SuspendLayout();
            this.gameOptionsTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadGameTabPage
            // 
            this.loadGameTabPage.Controls.Add(this.loadGamesControl);
            this.loadGameTabPage.Location = new System.Drawing.Point(4, 22);
            this.loadGameTabPage.Name = "loadGameTabPage";
            this.loadGameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.loadGameTabPage.Size = new System.Drawing.Size(485, 253);
            this.loadGameTabPage.TabIndex = 2;
            this.loadGameTabPage.Text = "Load game";
            this.loadGameTabPage.UseVisualStyleBackColor = true;
            // 
            // loadGamesControl
            // 
            this.loadGamesControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loadGamesControl.Location = new System.Drawing.Point(3, 3);
            this.loadGamesControl.Name = "loadGamesControl";
            this.loadGamesControl.Size = new System.Drawing.Size(479, 244);
            this.loadGamesControl.TabIndex = 0;
            // 
            // newGameTabPage
            // 
            this.newGameTabPage.Location = new System.Drawing.Point(4, 22);
            this.newGameTabPage.Name = "newGameTabPage";
            this.newGameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.newGameTabPage.Size = new System.Drawing.Size(485, 253);
            this.newGameTabPage.TabIndex = 1;
            this.newGameTabPage.Text = "New game";
            this.newGameTabPage.UseVisualStyleBackColor = true;
            // 
            // continueTabPage
            // 
            this.continueTabPage.Location = new System.Drawing.Point(4, 22);
            this.continueTabPage.Name = "continueTabPage";
            this.continueTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.continueTabPage.Size = new System.Drawing.Size(485, 253);
            this.continueTabPage.TabIndex = 0;
            this.continueTabPage.Text = "Continue";
            this.continueTabPage.UseVisualStyleBackColor = true;
            // 
            // gameOptionsTabControl
            // 
            this.gameOptionsTabControl.Controls.Add(this.continueTabPage);
            this.gameOptionsTabControl.Controls.Add(this.newGameTabPage);
            this.gameOptionsTabControl.Controls.Add(this.loadGameTabPage);
            this.gameOptionsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameOptionsTabControl.Location = new System.Drawing.Point(0, 0);
            this.gameOptionsTabControl.Name = "gameOptionsTabControl";
            this.gameOptionsTabControl.SelectedIndex = 0;
            this.gameOptionsTabControl.Size = new System.Drawing.Size(493, 279);
            this.gameOptionsTabControl.TabIndex = 2;
            // 
            // GameOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gameOptionsTabControl);
            this.Name = "GameOptionsControl";
            this.Size = new System.Drawing.Size(493, 279);
            this.loadGameTabPage.ResumeLayout(false);
            this.gameOptionsTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage loadGameTabPage;
        private System.Windows.Forms.TabPage newGameTabPage;
        private System.Windows.Forms.TabPage continueTabPage;
        private System.Windows.Forms.TabControl gameOptionsTabControl;
        private LoadGamesControl loadGamesControl;
    }
}
