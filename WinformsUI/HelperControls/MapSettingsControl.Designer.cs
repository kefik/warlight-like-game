namespace WinformsUI.HelperControls
{
    partial class MapSettingsControl
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
            this.mapLabel = new System.Windows.Forms.Label();
            this.mapComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // mapLabel
            // 
            this.mapLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mapLabel.AutoSize = true;
            this.mapLabel.Location = new System.Drawing.Point(3, 11);
            this.mapLabel.Name = "mapLabel";
            this.mapLabel.Size = new System.Drawing.Size(28, 13);
            this.mapLabel.TabIndex = 0;
            this.mapLabel.Text = "Map";
            // 
            // mapComboBox
            // 
            this.mapComboBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mapComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mapComboBox.FormattingEnabled = true;
            this.mapComboBox.Location = new System.Drawing.Point(45, 8);
            this.mapComboBox.Name = "mapComboBox";
            this.mapComboBox.Size = new System.Drawing.Size(121, 21);
            this.mapComboBox.TabIndex = 1;
            this.mapComboBox.SelectedIndexChanged += new System.EventHandler(this.MapChosen);
            // 
            // MapSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapComboBox);
            this.Controls.Add(this.mapLabel);
            this.Name = "MapSettingsControl";
            this.Size = new System.Drawing.Size(179, 37);
            this.Load += new System.EventHandler(this.ControlLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mapLabel;
        private System.Windows.Forms.ComboBox mapComboBox;
    }
}
