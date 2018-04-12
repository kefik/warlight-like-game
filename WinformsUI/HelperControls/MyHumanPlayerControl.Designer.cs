namespace WinformsUI.HelperControls
{
    partial class MyHumanPlayerControl
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
            this.playerNameTextBox = new System.Windows.Forms.TextBox();
            this.colorButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // playerNameTextBox
            // 
            this.playerNameTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.playerNameTextBox.Enabled = false;
            this.playerNameTextBox.Location = new System.Drawing.Point(3, 3);
            this.playerNameTextBox.Name = "playerNameTextBox";
            this.playerNameTextBox.Size = new System.Drawing.Size(81, 20);
            this.playerNameTextBox.TabIndex = 5;
            this.playerNameTextBox.TextChanged += new System.EventHandler(this.NameTextBoxTextChanged);
            // 
            // colorButton
            // 
            this.colorButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.colorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.colorButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.colorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorButton.Location = new System.Drawing.Point(110, 3);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(62, 21);
            this.colorButton.TabIndex = 4;
            this.colorButton.UseVisualStyleBackColor = false;
            this.colorButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChangeColor);
            // 
            // MyHumanPlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.playerNameTextBox);
            this.Controls.Add(this.colorButton);
            this.Name = "MyHumanPlayerControl";
            this.Size = new System.Drawing.Size(175, 26);
            this.Load += new System.EventHandler(this.ControlLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.TextBox playerNameTextBox;
        protected System.Windows.Forms.Button colorButton;
    }
}
