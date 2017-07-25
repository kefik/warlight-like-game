namespace WinformsUI.InGame.Phases
{
    partial class TurnPhaseControl
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
            this.nextButton = new System.Windows.Forms.Button();
            this.repeatRoundButton = new System.Windows.Forms.Button();
            this.deployLabel = new System.Windows.Forms.Label();
            this.attackLabel = new System.Windows.Forms.Label();
            this.commitLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // nextButton
            // 
            this.nextButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nextButton.Location = new System.Drawing.Point(83, 245);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(87, 23);
            this.nextButton.TabIndex = 6;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.Next);
            // 
            // repeatRoundButton
            // 
            this.repeatRoundButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.repeatRoundButton.Location = new System.Drawing.Point(3, 245);
            this.repeatRoundButton.Name = "repeatRoundButton";
            this.repeatRoundButton.Size = new System.Drawing.Size(74, 23);
            this.repeatRoundButton.TabIndex = 7;
            this.repeatRoundButton.Text = "Repeat round";
            this.repeatRoundButton.UseVisualStyleBackColor = true;
            this.repeatRoundButton.Click += new System.EventHandler(this.Repeat);
            // 
            // deployLabel
            // 
            this.deployLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.deployLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deployLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deployLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.deployLabel.Location = new System.Drawing.Point(52, 78);
            this.deployLabel.Name = "deployLabel";
            this.deployLabel.Size = new System.Drawing.Size(74, 26);
            this.deployLabel.TabIndex = 3;
            this.deployLabel.Text = "1. Deploy";
            this.deployLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deployLabel.Click += new System.EventHandler(this.Deploying);
            // 
            // attackLabel
            // 
            this.attackLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.attackLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.attackLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.attackLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.attackLabel.Location = new System.Drawing.Point(52, 104);
            this.attackLabel.Name = "attackLabel";
            this.attackLabel.Size = new System.Drawing.Size(74, 28);
            this.attackLabel.TabIndex = 4;
            this.attackLabel.Text = "2. Attack";
            this.attackLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.attackLabel.Click += new System.EventHandler(this.Attacking);
            // 
            // commitLabel
            // 
            this.commitLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.commitLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.commitLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.commitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.commitLabel.Location = new System.Drawing.Point(52, 132);
            this.commitLabel.Name = "commitLabel";
            this.commitLabel.Size = new System.Drawing.Size(74, 27);
            this.commitLabel.TabIndex = 5;
            this.commitLabel.Text = "3. Commit";
            this.commitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.commitLabel.Click += new System.EventHandler(this.Committing);
            // 
            // TurnPhaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.repeatRoundButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.commitLabel);
            this.Controls.Add(this.attackLabel);
            this.Controls.Add(this.deployLabel);
            this.Name = "TurnPhaseControl";
            this.Size = new System.Drawing.Size(173, 316);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button repeatRoundButton;
        private System.Windows.Forms.Label deployLabel;
        private System.Windows.Forms.Label attackLabel;
        private System.Windows.Forms.Label commitLabel;
    }
}
