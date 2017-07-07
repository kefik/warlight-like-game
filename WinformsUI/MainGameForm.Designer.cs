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
            this.passwordLabel = new System.Windows.Forms.Label();
            this.playerNameLabel = new System.Windows.Forms.Label();
            this.emailLabel = new System.Windows.Forms.Label();
            this.playerNameTextBox = new System.Windows.Forms.TextBox();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.typeGameChoiceTabControl.SuspendLayout();
            this.settingsTabPage.SuspendLayout();
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
            this.settingsTabPage.Controls.Add(this.passwordTextBox);
            this.settingsTabPage.Controls.Add(this.emailTextBox);
            this.settingsTabPage.Controls.Add(this.playerNameTextBox);
            this.settingsTabPage.Controls.Add(this.emailLabel);
            this.settingsTabPage.Controls.Add(this.passwordLabel);
            this.settingsTabPage.Controls.Add(this.playerNameLabel);
            this.settingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.settingsTabPage.Name = "settingsTabPage";
            this.settingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.settingsTabPage.Size = new System.Drawing.Size(673, 431);
            this.settingsTabPage.TabIndex = 2;
            this.settingsTabPage.Text = "Settings";
            this.settingsTabPage.UseVisualStyleBackColor = true;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(86, 73);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(53, 13);
            this.passwordLabel.TabIndex = 1;
            this.passwordLabel.Text = "Password";
            // 
            // playerNameLabel
            // 
            this.playerNameLabel.AutoSize = true;
            this.playerNameLabel.Location = new System.Drawing.Point(86, 51);
            this.playerNameLabel.Name = "playerNameLabel";
            this.playerNameLabel.Size = new System.Drawing.Size(67, 13);
            this.playerNameLabel.TabIndex = 0;
            this.playerNameLabel.Text = "Player Name";
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point(86, 95);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(32, 13);
            this.emailLabel.TabIndex = 2;
            this.emailLabel.Text = "Email";
            // 
            // playerNameTextBox
            // 
            this.playerNameTextBox.Location = new System.Drawing.Point(484, 44);
            this.playerNameTextBox.Name = "playerNameTextBox";
            this.playerNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.playerNameTextBox.TabIndex = 3;
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(484, 88);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(100, 20);
            this.emailTextBox.TabIndex = 4;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(484, 66);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(100, 20);
            this.passwordTextBox.TabIndex = 5;
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
            this.settingsTabPage.ResumeLayout(false);
            this.settingsTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage multiplayerTabPage;
        private System.Windows.Forms.TabPage singleplayerTabPage;
        private System.Windows.Forms.TabControl typeGameChoiceTabControl;
        private System.Windows.Forms.TabPage settingsTabPage;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.TextBox playerNameTextBox;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label playerNameLabel;
    }
}

