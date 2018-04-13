namespace WinformsUI.HelperControls
{
    partial class ProgressBarForm
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.aiPlayerEvaluatingMessageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 40);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(242, 35);
            this.progressBar.TabIndex = 0;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoBackgroundWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.ProgressChanged);
            // 
            // aiPlayerEvaluatingMessageLabel
            // 
            this.aiPlayerEvaluatingMessageLabel.AutoSize = true;
            this.aiPlayerEvaluatingMessageLabel.Location = new System.Drawing.Point(46, 22);
            this.aiPlayerEvaluatingMessageLabel.Name = "aiPlayerEvaluatingMessageLabel";
            this.aiPlayerEvaluatingMessageLabel.Size = new System.Drawing.Size(163, 13);
            this.aiPlayerEvaluatingMessageLabel.TabIndex = 1;
            this.aiPlayerEvaluatingMessageLabel.Text = "aiPlayerEvaluatingMessageLabel";
            // 
            // ProgressBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 87);
            this.ControlBox = false;
            this.Controls.Add(this.aiPlayerEvaluatingMessageLabel);
            this.Controls.Add(this.progressBar);
            this.Name = "ProgressBarForm";
            this.Text = "Progress";
            this.Load += new System.EventHandler(this.FormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Label aiPlayerEvaluatingMessageLabel;
    }
}