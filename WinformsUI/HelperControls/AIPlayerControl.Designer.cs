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
            this.aiName = new System.Windows.Forms.Label();
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
            this.difficultyComboBox.Location = new System.Drawing.Point(95, 6);
            this.difficultyComboBox.Name = "difficultyComboBox";
            this.difficultyComboBox.Size = new System.Drawing.Size(109, 21);
            this.difficultyComboBox.TabIndex = 0;
            // 
            // colorButton
            // 
            this.colorButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.colorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.colorButton.FlatAppearance.BorderSize = 0;
            this.colorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorButton.Location = new System.Drawing.Point(210, 6);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(62, 21);
            this.colorButton.TabIndex = 1;
            this.colorButton.UseVisualStyleBackColor = false;
            // 
            // aiName
            // 
            this.aiName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aiName.AutoSize = true;
            this.aiName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.aiName.Location = new System.Drawing.Point(13, 9);
            this.aiName.Name = "aiName";
            this.aiName.Size = new System.Drawing.Size(27, 13);
            this.aiName.TabIndex = 2;
            this.aiName.Text = "PC1";
            // 
            // AIPlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aiName);
            this.Controls.Add(this.colorButton);
            this.Controls.Add(this.difficultyComboBox);
            this.Name = "AIPlayerControl";
            this.Size = new System.Drawing.Size(283, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox difficultyComboBox;
        private System.Windows.Forms.Button colorButton;
        private System.Windows.Forms.Label aiName;
    }
}
