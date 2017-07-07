namespace WinformsUI
{
    partial class MainGameForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.multiplayerTabPage = new System.Windows.Forms.TabPage();
            this.singleplayerTabPage = new System.Windows.Forms.TabPage();
            this.typeGameChoiceTabControl = new System.Windows.Forms.TabControl();
            this.settingsTabPage = new System.Windows.Forms.TabPage();
            this.typeGameChoiceTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // multiplayerTabPage
            // 
            this.multiplayerTabPage.Location = new System.Drawing.Point(4, 22);
            this.multiplayerTabPage.Name = "multiplayerTabPage";
            this.multiplayerTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.multiplayerTabPage.Size = new System.Drawing.Size(673, 431);
            this.multiplayerTabPage.TabIndex = 1;
            this.multiplayerTabPage.Text = "Multiplayer";
            this.multiplayerTabPage.UseVisualStyleBackColor = true;
            // 
            // singleplayerTabPage
            // 
            this.singleplayerTabPage.AutoScroll = true;
            this.singleplayerTabPage.Location = new System.Drawing.Point(4, 22);
            this.singleplayerTabPage.Name = "singleplayerTabPage";
            this.singleplayerTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.singleplayerTabPage.Size = new System.Drawing.Size(673, 431);
            this.singleplayerTabPage.TabIndex = 0;
            this.singleplayerTabPage.Text = "Singleplayer";
            this.singleplayerTabPage.UseVisualStyleBackColor = true;
            // 
            // typeGameChoiceTabControl
            // 
            this.typeGameChoiceTabControl.Controls.Add(this.singleplayerTabPage);
            this.typeGameChoiceTabControl.Controls.Add(this.multiplayerTabPage);
            this.typeGameChoiceTabControl.Controls.Add(this.settingsTabPage);
            this.typeGameChoiceTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeGameChoiceTabControl.Location = new System.Drawing.Point(0, 0);
            this.typeGameChoiceTabControl.Name = "typeGameChoiceTabControl";
            this.typeGameChoiceTabControl.SelectedIndex = 0;
            this.typeGameChoiceTabControl.Size = new System.Drawing.Size(681, 457);
            this.typeGameChoiceTabControl.TabIndex = 1;
            // 
            // settingsTabPage
            // 
            this.settingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.settingsTabPage.Name = "settingsTabPage";
            this.settingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.settingsTabPage.Size = new System.Drawing.Size(673, 431);
            this.settingsTabPage.TabIndex = 2;
            this.settingsTabPage.Text = "Settings";
            this.settingsTabPage.UseVisualStyleBackColor = true;
            // 
            // MainGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(681, 457);
            this.Controls.Add(this.typeGameChoiceTabControl);
            this.Name = "MainGameForm";
            this.Text = "Warlight";
            this.typeGameChoiceTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage multiplayerTabPage;
        private System.Windows.Forms.TabPage singleplayerTabPage;
        private System.Windows.Forms.TabControl typeGameChoiceTabControl;
        private System.Windows.Forms.TabPage settingsTabPage;
    }
}

