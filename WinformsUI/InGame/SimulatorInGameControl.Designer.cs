﻿namespace WinformsUI.InGame
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
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.gameMapPictureBox = new System.Windows.Forms.PictureBox();
            this.gameMenuPanel.SuspendLayout();
            this.gameStateMenuPanel.SuspendLayout();
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
            this.gameStateMenuPanel.Controls.Add(this.button7);
            this.gameStateMenuPanel.Controls.Add(this.button6);
            this.gameStateMenuPanel.Controls.Add(this.button5);
            this.gameStateMenuPanel.Controls.Add(this.button4);
            this.gameStateMenuPanel.Controls.Add(this.button3);
            this.gameStateMenuPanel.Controls.Add(this.button2);
            this.gameStateMenuPanel.Controls.Add(this.button1);
            this.gameStateMenuPanel.Location = new System.Drawing.Point(3, 3);
            this.gameStateMenuPanel.Name = "gameStateMenuPanel";
            this.gameStateMenuPanel.Size = new System.Drawing.Size(177, 358);
            this.gameStateMenuPanel.TabIndex = 0;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(82, 144);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(36, 23);
            this.button7.TabIndex = 6;
            this.button7.Text = ">";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(48, 144);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(35, 23);
            this.button6.TabIndex = 5;
            this.button6.Text = "<";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(48, 202);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(35, 23);
            this.button5.TabIndex = 4;
            this.button5.Text = "<<<";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(82, 202);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(36, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = ">>>";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(48, 173);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(35, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "<<";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(82, 173);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(36, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = ">>";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(48, 115);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = ">|";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // gameMapPictureBox
            // 
            this.gameMapPictureBox.Location = new System.Drawing.Point(189, 5);
            this.gameMapPictureBox.Name = "gameMapPictureBox";
            this.gameMapPictureBox.Size = new System.Drawing.Size(578, 439);
            this.gameMapPictureBox.TabIndex = 5;
            this.gameMapPictureBox.TabStop = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.gameMapPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel gameMenuPanel;
        private System.Windows.Forms.Button menuButton;
        private System.Windows.Forms.Panel gameStateMenuPanel;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox gameMapPictureBox;
    }
}
