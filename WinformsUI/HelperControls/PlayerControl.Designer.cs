namespace WinformsUI.HelperControls
{
    partial class PlayerControl
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
            this.colorButton = new System.Windows.Forms.Button();
            this.playerName = new System.Windows.Forms.Label();
            this.stateComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // colorButton
            // 
            this.colorButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.colorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.colorButton.FlatAppearance.BorderSize = 0;
            this.colorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorButton.Location = new System.Drawing.Point(203, 5);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(62, 21);
            this.colorButton.TabIndex = 2;
            this.colorButton.UseVisualStyleBackColor = false;
            // 
            // playerName
            // 
            this.playerName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.playerName.AutoSize = true;
            this.playerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.playerName.Location = new System.Drawing.Point(72, 8);
            this.playerName.Name = "playerName";
            this.playerName.Size = new System.Drawing.Size(75, 13);
            this.playerName.TabIndex = 3;
            this.playerName.Text = "Bimbinbiribong";
            // 
            // stateComboBox
            // 
            this.stateComboBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.stateComboBox.AutoCompleteCustomSource.AddRange(new string[] {
            "Free",
            "Filled"});
            this.stateComboBox.FormattingEnabled = true;
            this.stateComboBox.Items.AddRange(new object[] {
            "Easy",
            "Medium",
            "Hard"});
            this.stateComboBox.Location = new System.Drawing.Point(3, 5);
            this.stateComboBox.Name = "stateComboBox";
            this.stateComboBox.Size = new System.Drawing.Size(58, 21);
            this.stateComboBox.TabIndex = 4;
            // 
            // PlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stateComboBox);
            this.Controls.Add(this.playerName);
            this.Controls.Add(this.colorButton);
            this.MinimumSize = new System.Drawing.Size(243, 21);
            this.Name = "PlayerControl";
            this.Size = new System.Drawing.Size(268, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button colorButton;
        private System.Windows.Forms.Label playerName;
        private System.Windows.Forms.ComboBox stateComboBox;
    }
}
