namespace WinformsUI.HelperControls
{
    partial class HumanPlayerControl
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
            this.playerNameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // colorButton
            // 
            this.colorButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.colorButton.BackColor = System.Drawing.Color.Aqua;
            this.colorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorButton.Location = new System.Drawing.Point(110, 5);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(62, 21);
            this.colorButton.TabIndex = 2;
            this.colorButton.UseVisualStyleBackColor = false;
            this.colorButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChangeColor);
            // 
            // playerNameTextBox
            // 
            this.playerNameTextBox.Location = new System.Drawing.Point(3, 6);
            this.playerNameTextBox.Name = "playerNameTextBox";
            this.playerNameTextBox.Size = new System.Drawing.Size(75, 20);
            this.playerNameTextBox.TabIndex = 3;
            this.playerNameTextBox.TextChanged += new System.EventHandler(this.PlayerNameTextChanged);
            // 
            // HumanPlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(this.playerNameTextBox);
            this.Controls.Add(this.colorButton);
            this.MinimumSize = new System.Drawing.Size(0, 21);
            this.Name = "HumanPlayerControl";
            this.Size = new System.Drawing.Size(175, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button colorButton;
        private System.Windows.Forms.TextBox playerNameTextBox;
    }
}
