namespace WinformsUI.InGame.Phases
{
    partial class BeginRoundPhaseControl
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
            this.watchButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // beginButton
            // 
            this.beginButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.beginButton.Location = new System.Drawing.Point(83, 203);
            this.beginButton.Name = "beginButton";
            this.beginButton.Size = new System.Drawing.Size(83, 23);
            this.beginButton.TabIndex = 0;
            this.beginButton.Text = "Begin";
            this.beginButton.UseVisualStyleBackColor = true;
            this.beginButton.Click += new System.EventHandler(this.BeginRound);
            // 
            // watchButton
            // 
            this.watchButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.watchButton.Location = new System.Drawing.Point(3, 203);
            this.watchButton.Name = "watchButton";
            this.watchButton.Size = new System.Drawing.Size(74, 23);
            this.watchButton.TabIndex = 3;
            this.watchButton.Text = "Watch";
            this.watchButton.UseVisualStyleBackColor = true;
            this.watchButton.Click += new System.EventHandler(this.WatchRound);
            // 
            // BeginRoundPhaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.watchButton);
            this.Controls.Add(this.beginButton);
            this.Name = "BeginRoundPhaseControl";
            this.Size = new System.Drawing.Size(170, 301);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button beginButton;
        private System.Windows.Forms.Button watchButton;
    }
}
