namespace WinformsUI.InGame
{
    partial class MapHandlerControl
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
            this.gameMapPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.gameMapPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // gameMapPictureBox
            // 
            this.gameMapPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gameMapPictureBox.Location = new System.Drawing.Point(4, 4);
            this.gameMapPictureBox.Name = "gameMapPictureBox";
            this.gameMapPictureBox.Size = new System.Drawing.Size(535, 364);
            this.gameMapPictureBox.TabIndex = 0;
            this.gameMapPictureBox.TabStop = false;
            this.gameMapPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageClick);
            this.gameMapPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageHover);
            // 
            // MapHandlerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.gameMapPictureBox);
            this.Name = "MapHandlerControl";
            this.Size = new System.Drawing.Size(542, 374);
            ((System.ComponentModel.ISupportInitialize)(this.gameMapPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox gameMapPictureBox;
    }
}
