namespace WinformsUI.GameSetup.Singleplayer
{
    partial class LoadGamesControl
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
            this.loadedGamesListBox = new System.Windows.Forms.ListBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loadedGamesListBox
            // 
            this.loadedGamesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.loadedGamesListBox.FormattingEnabled = true;
            this.loadedGamesListBox.Items.AddRange(new object[] {
            "AI:4; Map: World; Saved: 18.05.2015 12:58"});
            this.loadedGamesListBox.Location = new System.Drawing.Point(15, 16);
            this.loadedGamesListBox.Name = "loadedGamesListBox";
            this.loadedGamesListBox.Size = new System.Drawing.Size(391, 160);
            this.loadedGamesListBox.TabIndex = 0;
            // 
            // loadButton
            // 
            this.loadButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.loadButton.Location = new System.Drawing.Point(332, 185);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(74, 23);
            this.loadButton.TabIndex = 1;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            // 
            // LoadGamesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.loadedGamesListBox);
            this.Name = "LoadGamesControl";
            this.Size = new System.Drawing.Size(418, 219);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox loadedGamesListBox;
        private System.Windows.Forms.Button loadButton;
    }
}
