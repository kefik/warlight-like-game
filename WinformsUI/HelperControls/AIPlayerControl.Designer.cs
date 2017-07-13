namespace WinformsUI.HelperControls
{
    partial class AIPlayerControl
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
            this.difficultyComboBox = new System.Windows.Forms.ComboBox();
            this.colorButton = new System.Windows.Forms.Button();
            this.aiNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // difficultyComboBox
            // 
            this.difficultyComboBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.difficultyComboBox.FormattingEnabled = true;
            this.difficultyComboBox.Items.AddRange(new object[] {
            "Easy",
            "Medium",
            "Hard"});
            this.difficultyComboBox.Location = new System.Drawing.Point(90, 2);
            this.difficultyComboBox.Name = "difficultyComboBox";
            this.difficultyComboBox.Size = new System.Drawing.Size(109, 21);
            this.difficultyComboBox.TabIndex = 0;
            // 
            // colorButton
            // 
            this.colorButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.colorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.colorButton.FlatAppearance.BorderSize = 0;
            this.colorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorButton.Location = new System.Drawing.Point(205, 2);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(62, 21);
            this.colorButton.TabIndex = 1;
            this.colorButton.UseVisualStyleBackColor = false;
            // 
            // aiNameLabel
            // 
            this.aiNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aiNameLabel.AutoSize = true;
            this.aiNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.aiNameLabel.Location = new System.Drawing.Point(13, 5);
            this.aiNameLabel.Name = "aiNameLabel";
            this.aiNameLabel.Size = new System.Drawing.Size(27, 13);
            this.aiNameLabel.TabIndex = 2;
            this.aiNameLabel.Text = "PC1";
            // 
            // AIPlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aiNameLabel);
            this.Controls.Add(this.colorButton);
            this.Controls.Add(this.difficultyComboBox);
            this.MinimumSize = new System.Drawing.Size(272, 27);
            this.Name = "AIPlayerControl";
            this.Size = new System.Drawing.Size(272, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox difficultyComboBox;
        private System.Windows.Forms.Button colorButton;
        private System.Windows.Forms.Label aiNameLabel;
    }
}
