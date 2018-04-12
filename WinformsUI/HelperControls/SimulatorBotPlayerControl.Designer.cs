namespace WinformsUI.HelperControls
{
    partial class SimulatorBotPlayerControl
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
            this.aiNameLabel = new System.Windows.Forms.Label();
            this.colorButton = new System.Windows.Forms.Button();
            this.botTypeComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // aiNameLabel
            // 
            this.aiNameLabel.AutoSize = true;
            this.aiNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.aiNameLabel.Location = new System.Drawing.Point(3, 7);
            this.aiNameLabel.Name = "aiNameLabel";
            this.aiNameLabel.Size = new System.Drawing.Size(27, 13);
            this.aiNameLabel.TabIndex = 5;
            this.aiNameLabel.Text = "PC1";
            // 
            // colorButton
            // 
            this.colorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.colorButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.colorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorButton.Location = new System.Drawing.Point(217, 4);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(66, 21);
            this.colorButton.TabIndex = 4;
            this.colorButton.UseVisualStyleBackColor = false;
            this.colorButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChangeColor);
            // 
            // botTypeComboBox
            // 
            this.botTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.botTypeComboBox.FormattingEnabled = true;
            this.botTypeComboBox.Location = new System.Drawing.Point(99, 4);
            this.botTypeComboBox.Name = "botTypeComboBox";
            this.botTypeComboBox.Size = new System.Drawing.Size(112, 21);
            this.botTypeComboBox.TabIndex = 3;
            this.botTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.ChangeBotType);
            // 
            // SimulatorBotPlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.aiNameLabel);
            this.Controls.Add(this.colorButton);
            this.Controls.Add(this.botTypeComboBox);
            this.Name = "SimulatorBotPlayerControl";
            this.Size = new System.Drawing.Size(286, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label aiNameLabel;
        private System.Windows.Forms.Button colorButton;
        private System.Windows.Forms.ComboBox botTypeComboBox;
    }
}
