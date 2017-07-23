using System;
using System.Windows.Forms;

namespace WinformsUI.InGame.Phases
{
    public partial class BeginRoundPhaseControl : UserControl
    {
        public BeginRoundPhaseControl()
        {
            InitializeComponent();

            beginButton.Enabled = false;
        }

        public event Action OnBegin;
        public event Action OnWatch;

        private void BeginRound(object sender, System.EventArgs e)
        {
            OnBegin?.Invoke();
        }

        private void WatchRound(object sender, System.EventArgs e)
        {
            OnWatch?.Invoke();
            beginButton.Enabled = true;
        }
    }
}
