namespace WinformsUI.InGame
{
    partial class SimulatorInGameControl
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
            this.playerPerspectiveLabel = new System.Windows.Forms.Label();
            this.playerPerspectiveComboBox = new System.Windows.Forms.ComboBox();
            this.botThinkingTimeLabel = new System.Windows.Forms.Label();
            this.botThinkingTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.nextActionButton = new System.Windows.Forms.Button();
            this.previousActionButton = new System.Windows.Forms.Button();
            this.previousRoundButton = new System.Windows.Forms.Button();
            this.nextRoundButton = new System.Windows.Forms.Button();
            this.previousTurnButton = new System.Windows.Forms.Button();
            this.nextTurnButton = new System.Windows.Forms.Button();
            this.playPauseButton = new System.Windows.Forms.Button();
            this.gameMapPictureBox = new System.Windows.Forms.PictureBox();
            this.displayedRoundLabel = new System.Windows.Forms.Label();
            this.roundNumber = new System.Windows.Forms.Label();
            this.gameMenuPanel.SuspendLayout();
            this.gameStateMenuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.botThinkingTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameMapPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // gameMenuPanel
            // 
            this.gameMenuPanel.Controls.Add(this.menuButton);
            this.gameMenuPanel.Controls.Add(this.gameStateMenuPanel);
            this.gameMenuPanel.Location = new System.Drawing.Point(3, 5);
            this.gameMenuPanel.Name = "gameMenuPanel";
            this.gameMenuPanel.Size = new System.Drawing.Size(180, 436);
            this.gameMenuPanel.TabIndex = 4;
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
            this.gameStateMenuPanel.Controls.Add(this.roundNumber);
            this.gameStateMenuPanel.Controls.Add(this.displayedRoundLabel);
            this.gameStateMenuPanel.Controls.Add(this.playerPerspectiveLabel);
            this.gameStateMenuPanel.Controls.Add(this.playerPerspectiveComboBox);
            this.gameStateMenuPanel.Controls.Add(this.botThinkingTimeLabel);
            this.gameStateMenuPanel.Controls.Add(this.botThinkingTimeNumericUpDown);
            this.gameStateMenuPanel.Controls.Add(this.nextActionButton);
            this.gameStateMenuPanel.Controls.Add(this.previousActionButton);
            this.gameStateMenuPanel.Controls.Add(this.previousRoundButton);
            this.gameStateMenuPanel.Controls.Add(this.nextRoundButton);
            this.gameStateMenuPanel.Controls.Add(this.previousTurnButton);
            this.gameStateMenuPanel.Controls.Add(this.nextTurnButton);
            this.gameStateMenuPanel.Controls.Add(this.playPauseButton);
            this.gameStateMenuPanel.Location = new System.Drawing.Point(3, 3);
            this.gameStateMenuPanel.Name = "gameStateMenuPanel";
            this.gameStateMenuPanel.Size = new System.Drawing.Size(177, 358);
            this.gameStateMenuPanel.TabIndex = 0;
            // 
            // playerPerspectiveLabel
            // 
            this.playerPerspectiveLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.playerPerspectiveLabel.AutoSize = true;
            this.playerPerspectiveLabel.Location = new System.Drawing.Point(12, 273);
            this.playerPerspectiveLabel.Name = "playerPerspectiveLabel";
            this.playerPerspectiveLabel.Size = new System.Drawing.Size(88, 13);
            this.playerPerspectiveLabel.TabIndex = 10;
            this.playerPerspectiveLabel.Text = "View perspective";
            // 
            // playerPerspectiveComboBox
            // 
            this.playerPerspectiveComboBox.FormattingEnabled = true;
            this.playerPerspectiveComboBox.Location = new System.Drawing.Point(58, 289);
            this.playerPerspectiveComboBox.Name = "playerPerspectiveComboBox";
            this.playerPerspectiveComboBox.Size = new System.Drawing.Size(105, 21);
            this.playerPerspectiveComboBox.TabIndex = 9;
            this.playerPerspectiveComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.PlayerPerspectiveDrawItem);
            this.playerPerspectiveComboBox.SelectedIndexChanged += new System.EventHandler(this.PlayerPerspectiveChanged);
            // 
            // botThinkingTimeLabel
            // 
            this.botThinkingTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.botThinkingTimeLabel.AutoSize = true;
            this.botThinkingTimeLabel.Location = new System.Drawing.Point(12, 244);
            this.botThinkingTimeLabel.Name = "botThinkingTimeLabel";
            this.botThinkingTimeLabel.Size = new System.Drawing.Size(85, 13);
            this.botThinkingTimeLabel.TabIndex = 8;
            this.botThinkingTimeLabel.Text = "Time for bot (ms)";
            // 
            // botThinkingTimeNumericUpDown
            // 
            this.botThinkingTimeNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.botThinkingTimeNumericUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.botThinkingTimeNumericUpDown.Location = new System.Drawing.Point(103, 242);
            this.botThinkingTimeNumericUpDown.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.botThinkingTimeNumericUpDown.Name = "botThinkingTimeNumericUpDown";
            this.botThinkingTimeNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.botThinkingTimeNumericUpDown.TabIndex = 7;
            this.botThinkingTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.botThinkingTimeNumericUpDown.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // nextActionButton
            // 
            this.nextActionButton.Location = new System.Drawing.Point(82, 144);
            this.nextActionButton.Name = "nextActionButton";
            this.nextActionButton.Size = new System.Drawing.Size(45, 23);
            this.nextActionButton.TabIndex = 6;
            this.nextActionButton.Text = ">";
            this.nextActionButton.UseVisualStyleBackColor = true;
            this.nextActionButton.Click += new System.EventHandler(this.NextActionButtonClick);
            // 
            // previousActionButton
            // 
            this.previousActionButton.Location = new System.Drawing.Point(39, 144);
            this.previousActionButton.Name = "previousActionButton";
            this.previousActionButton.Size = new System.Drawing.Size(44, 23);
            this.previousActionButton.TabIndex = 5;
            this.previousActionButton.Text = "<";
            this.previousActionButton.UseVisualStyleBackColor = true;
            this.previousActionButton.Click += new System.EventHandler(this.PreviousActionButtonClick);
            // 
            // previousRoundButton
            // 
            this.previousRoundButton.Location = new System.Drawing.Point(39, 202);
            this.previousRoundButton.Name = "previousRoundButton";
            this.previousRoundButton.Size = new System.Drawing.Size(44, 23);
            this.previousRoundButton.TabIndex = 4;
            this.previousRoundButton.Text = "<<<";
            this.previousRoundButton.UseVisualStyleBackColor = true;
            this.previousRoundButton.Click += new System.EventHandler(this.BeginningOfTheGameButtonClick);
            // 
            // nextRoundButton
            // 
            this.nextRoundButton.Location = new System.Drawing.Point(82, 202);
            this.nextRoundButton.Name = "nextRoundButton";
            this.nextRoundButton.Size = new System.Drawing.Size(45, 23);
            this.nextRoundButton.TabIndex = 3;
            this.nextRoundButton.Text = ">>>";
            this.nextRoundButton.UseVisualStyleBackColor = true;
            this.nextRoundButton.Click += new System.EventHandler(this.EndOfTheGameButtonClick);
            // 
            // previousTurnButton
            // 
            this.previousTurnButton.Location = new System.Drawing.Point(39, 173);
            this.previousTurnButton.Name = "previousTurnButton";
            this.previousTurnButton.Size = new System.Drawing.Size(44, 23);
            this.previousTurnButton.TabIndex = 2;
            this.previousTurnButton.Text = "<<";
            this.previousTurnButton.UseVisualStyleBackColor = true;
            this.previousTurnButton.Click += new System.EventHandler(this.PreviousRoundButtonClick);
            // 
            // nextTurnButton
            // 
            this.nextTurnButton.Location = new System.Drawing.Point(82, 173);
            this.nextTurnButton.Name = "nextTurnButton";
            this.nextTurnButton.Size = new System.Drawing.Size(45, 23);
            this.nextTurnButton.TabIndex = 1;
            this.nextTurnButton.Text = ">>";
            this.nextTurnButton.UseVisualStyleBackColor = true;
            this.nextTurnButton.Click += new System.EventHandler(this.NextRoundButtonClick);
            // 
            // playPauseButton
            // 
            this.playPauseButton.Location = new System.Drawing.Point(39, 115);
            this.playPauseButton.Name = "playPauseButton";
            this.playPauseButton.Size = new System.Drawing.Size(88, 23);
            this.playPauseButton.TabIndex = 0;
            this.playPauseButton.Text = ">|";
            this.playPauseButton.UseVisualStyleBackColor = true;
            this.playPauseButton.Click += new System.EventHandler(this.PlayOrStopButtonClick);
            // 
            // gameMapPictureBox
            // 
            this.gameMapPictureBox.Location = new System.Drawing.Point(189, 5);
            this.gameMapPictureBox.Name = "gameMapPictureBox";
            this.gameMapPictureBox.Size = new System.Drawing.Size(578, 439);
            this.gameMapPictureBox.TabIndex = 5;
            this.gameMapPictureBox.TabStop = false;
            // 
            // displayedRoundLabel
            // 
            this.displayedRoundLabel.AutoSize = true;
            this.displayedRoundLabel.Location = new System.Drawing.Point(36, 56);
            this.displayedRoundLabel.Name = "displayedRoundLabel";
            this.displayedRoundLabel.Size = new System.Drawing.Size(83, 13);
            this.displayedRoundLabel.TabIndex = 11;
            this.displayedRoundLabel.Text = "Displayed round";
            // 
            // roundNumber
            // 
            this.roundNumber.AutoSize = true;
            this.roundNumber.Location = new System.Drawing.Point(70, 69);
            this.roundNumber.Name = "roundNumber";
            this.roundNumber.Size = new System.Drawing.Size(13, 13);
            this.roundNumber.TabIndex = 12;
            this.roundNumber.Text = "0";
            // 
            // SimulatorInGameControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gameMapPictureBox);
            this.Controls.Add(this.gameMenuPanel);
            this.Name = "SimulatorInGameControl";
            this.Size = new System.Drawing.Size(770, 444);
            this.gameMenuPanel.ResumeLayout(false);
            this.gameStateMenuPanel.ResumeLayout(false);
            this.gameStateMenuPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.botThinkingTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameMapPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel gameMenuPanel;
        private System.Windows.Forms.Button menuButton;
        private System.Windows.Forms.Panel gameStateMenuPanel;
        private System.Windows.Forms.Button nextActionButton;
        private System.Windows.Forms.Button previousActionButton;
        private System.Windows.Forms.Button previousRoundButton;
        private System.Windows.Forms.Button nextRoundButton;
        private System.Windows.Forms.Button previousTurnButton;
        private System.Windows.Forms.Button nextTurnButton;
        private System.Windows.Forms.Button playPauseButton;
        private System.Windows.Forms.PictureBox gameMapPictureBox;
        private System.Windows.Forms.Label botThinkingTimeLabel;
        private System.Windows.Forms.NumericUpDown botThinkingTimeNumericUpDown;
        private System.Windows.Forms.ComboBox playerPerspectiveComboBox;
        private System.Windows.Forms.Label playerPerspectiveLabel;
        private System.Windows.Forms.Label displayedRoundLabel;
        private System.Windows.Forms.Label roundNumber;
    }
}
