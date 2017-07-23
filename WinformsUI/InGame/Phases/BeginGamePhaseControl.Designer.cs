namespace WinformsUI.InGame.Phases
{
    partial class BeginGamePhaseControl
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
            this.startOverButton = new System.Windows.Forms.Button();
            this.commitButton = new System.Windows.Forms.Button();
            this.instructionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startOverButton
            // 
            this.startOverButton.Location = new System.Drawing.Point(6, 175);
            this.startOverButton.Name = "startOverButton";
            this.startOverButton.Size = new System.Drawing.Size(75, 23);
            this.startOverButton.TabIndex = 0;
            this.startOverButton.Text = "Start over";
            this.startOverButton.UseVisualStyleBackColor = true;
            this.startOverButton.Click += new System.EventHandler(this.StartOver);
            // 
            // commitButton
            // 
            this.commitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.commitButton.Location = new System.Drawing.Point(81, 175);
            this.commitButton.Name = "commitButton";
            this.commitButton.Size = new System.Drawing.Size(75, 23);
            this.commitButton.TabIndex = 1;
            this.commitButton.Text = "Commit";
            this.commitButton.UseVisualStyleBackColor = true;
            this.commitButton.Click += new System.EventHandler(this.Commit);
            // 
            // instructionLabel
            // 
            this.instructionLabel.Location = new System.Drawing.Point(3, 201);
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.instructionLabel.Size = new System.Drawing.Size(156, 95);
            this.instructionLabel.TabIndex = 2;
            this.instructionLabel.Text = "Please choose 2 regions and commit to start the game.";
            this.instructionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BeginGamePhaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.instructionLabel);
            this.Controls.Add(this.commitButton);
            this.Controls.Add(this.startOverButton);
            this.Name = "BeginGamePhaseControl";
            this.Size = new System.Drawing.Size(162, 412);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button startOverButton;
        private System.Windows.Forms.Button commitButton;
        private System.Windows.Forms.Label instructionLabel;
    }
}
