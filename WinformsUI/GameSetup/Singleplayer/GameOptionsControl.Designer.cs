namespace WinformsUI.GameSetup.Singleplayer
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
            this.gameOptionsTabControl = new System.Windows.Forms.TabControl();
            this.continueTabPage = new System.Windows.Forms.TabPage();
            this.newGameTabPage = new System.Windows.Forms.TabPage();
            this.loadGameTabPage = new System.Windows.Forms.TabPage();
            this.gameOptionsTabControl.SuspendLayout();
            this.SuspendLayout();
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
            this.gameOptionsTabControl.TabIndex = 1;
            // 
            // continueTabPage
            // 
            this.continueTabPage.Location = new System.Drawing.Point(4, 22);
            this.continueTabPage.Name = "continueTabPage";
            this.continueTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.continueTabPage.Size = new System.Drawing.Size(192, 74);
            this.continueTabPage.TabIndex = 0;
            this.continueTabPage.Text = "Continue";
            this.continueTabPage.UseVisualStyleBackColor = true;
            // 
            // newGameTabPage
            // 
            this.newGameTabPage.Location = new System.Drawing.Point(4, 22);
            this.newGameTabPage.Name = "newGameTabPage";
            this.newGameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.newGameTabPage.Size = new System.Drawing.Size(192, 74);
            this.newGameTabPage.TabIndex = 1;
            this.newGameTabPage.Text = "New game";
            this.newGameTabPage.UseVisualStyleBackColor = true;
            // 
            // loadGameTabPage
            // 
            this.loadGameTabPage.Location = new System.Drawing.Point(4, 22);
            this.loadGameTabPage.Name = "loadGameTabPage";
            this.loadGameTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.loadGameTabPage.Size = new System.Drawing.Size(485, 253);
            this.loadGameTabPage.TabIndex = 2;
            this.loadGameTabPage.Text = "Load game";
            this.loadGameTabPage.UseVisualStyleBackColor = true;
            // 
            // GameOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gameOptionsTabControl);
            this.Name = "GameOptionsControl";
            this.Size = new System.Drawing.Size(493, 279);
            this.gameOptionsTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl gameOptionsTabControl;
        private System.Windows.Forms.TabPage continueTabPage;
        private System.Windows.Forms.TabPage newGameTabPage;
        private System.Windows.Forms.TabPage loadGameTabPage;
    }
}
