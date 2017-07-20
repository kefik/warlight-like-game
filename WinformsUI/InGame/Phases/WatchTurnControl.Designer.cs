namespace WinformsUI.InGame.Phases
{
    partial class WatchTurnControl
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
            this.menuButton = new System.Windows.Forms.Button();
            this.watchTurnButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // menuButton
            // 
            this.menuButton.Location = new System.Drawing.Point(34, 380);
            this.menuButton.Name = "menuButton";
            this.menuButton.Size = new System.Drawing.Size(139, 21);
            this.menuButton.TabIndex = 5;
            this.menuButton.Text = "Menu";
            this.menuButton.UseVisualStyleBackColor = true;
            // 
            // watchTurnButton
            // 
            this.watchTurnButton.Location = new System.Drawing.Point(34, 153);
            this.watchTurnButton.Name = "watchTurnButton";
            this.watchTurnButton.Size = new System.Drawing.Size(139, 35);
            this.watchTurnButton.TabIndex = 4;
            this.watchTurnButton.Text = "Watch turn";
            this.watchTurnButton.UseVisualStyleBackColor = true;
            // 
            // WatchTurnControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuButton);
            this.Controls.Add(this.watchTurnButton);
            this.Name = "WatchTurnControl";
            this.Size = new System.Drawing.Size(201, 410);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button menuButton;
        private System.Windows.Forms.Button watchTurnButton;
    }
}
