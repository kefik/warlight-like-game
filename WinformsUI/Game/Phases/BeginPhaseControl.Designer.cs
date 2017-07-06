namespace WinformsUI.Game.Phases
{
    partial class BeginPhaseControl
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
            this.beginButton = new System.Windows.Forms.Button();
            this.menuButton = new System.Windows.Forms.Button();
            this.repeatRoundButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // beginButton
            // 
            this.beginButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.beginButton.Location = new System.Drawing.Point(95, 196);
            this.beginButton.Name = "beginButton";
            this.beginButton.Size = new System.Drawing.Size(90, 23);
            this.beginButton.TabIndex = 0;
            this.beginButton.Text = "Begin";
            this.beginButton.UseVisualStyleBackColor = true;
            // 
            // menuButton
            // 
            this.menuButton.Location = new System.Drawing.Point(34, 373);
            this.menuButton.Name = "menuButton";
            this.menuButton.Size = new System.Drawing.Size(139, 21);
            this.menuButton.TabIndex = 1;
            this.menuButton.Text = "Menu";
            this.menuButton.UseVisualStyleBackColor = true;
            // 
            // repeatRoundButton
            // 
            this.repeatRoundButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.repeatRoundButton.Location = new System.Drawing.Point(3, 196);
            this.repeatRoundButton.Name = "repeatRoundButton";
            this.repeatRoundButton.Size = new System.Drawing.Size(89, 23);
            this.repeatRoundButton.TabIndex = 3;
            this.repeatRoundButton.Text = "Repeat round";
            this.repeatRoundButton.UseVisualStyleBackColor = true;
            // 
            // BeginPhaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.repeatRoundButton);
            this.Controls.Add(this.menuButton);
            this.Controls.Add(this.beginButton);
            this.Name = "BeginPhaseControl";
            this.Size = new System.Drawing.Size(189, 410);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button beginButton;
        private System.Windows.Forms.Button menuButton;
        private System.Windows.Forms.Button repeatRoundButton;
    }
}
