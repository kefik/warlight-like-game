using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsUI.HelperControls
{
    using System.Threading;
    using GameObjectsLib.Players;

    public partial class ProgressBarForm : Form
    {
        public TimeSpan TimeSpan { get; set; }

        public AiPlayer AiPlayer { get; set; }

        public ProgressBarForm()
        {
            InitializeComponent();
        }

        public void Start()
        {
            aiPlayerEvaluatingMessageLabel.Text =
                $"Bot {AiPlayer?.Name} is being evaluated";
            
            backgroundWorker.RunWorkerAsync(TimeSpan);
        }

        private void DoBackgroundWork(object sender, DoWorkEventArgs e)
        {
            var timeSpan = (TimeSpan)e.Argument;

            int totalDividedMilliseconds = (int)timeSpan.TotalMilliseconds / 10;
            
            for (int i = 0; i <= 100; i += 10)
            {
                backgroundWorker.ReportProgress(i);

                Thread.Sleep(totalDividedMilliseconds);
            }

            Invoke(new Action(Close));
        }

        private void FormLoad(object sender, EventArgs e)
        {
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
    }
}
