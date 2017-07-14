namespace WinformsUI.GameSetup.Multiplayer
{
    partial class GameTypeChoiceForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.multiplayerGameTypeComboBox = new System.Windows.Forms.ComboBox();
            this.gameTypeLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // multiplayerGameTypeComboBox
            // 
            this.multiplayerGameTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.multiplayerGameTypeComboBox.FormattingEnabled = true;
            this.multiplayerGameTypeComboBox.Items.AddRange(new object[] {
            "Hotseat",
            "Network"});
            this.multiplayerGameTypeComboBox.Location = new System.Drawing.Point(74, 28);
            this.multiplayerGameTypeComboBox.Name = "multiplayerGameTypeComboBox";
            this.multiplayerGameTypeComboBox.Size = new System.Drawing.Size(160, 21);
            this.multiplayerGameTypeComboBox.TabIndex = 0;
            // 
            // gameTypeLabel
            // 
            this.gameTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.gameTypeLabel.Location = new System.Drawing.Point(12, 9);
            this.gameTypeLabel.Name = "gameTypeLabel";
            this.gameTypeLabel.Size = new System.Drawing.Size(100, 16);
            this.gameTypeLabel.TabIndex = 1;
            this.gameTypeLabel.Text = "Game type";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(197, 66);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.Ok);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(116, 66);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.Cancel);
            // 
            // GameTypeChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 101);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.gameTypeLabel);
            this.Controls.Add(this.multiplayerGameTypeComboBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(304, 140);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(304, 140);
            this.Name = "GameTypeChoiceForm";
            this.Text = "Warlight | Type choose";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox multiplayerGameTypeComboBox;
        private System.Windows.Forms.Label gameTypeLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}