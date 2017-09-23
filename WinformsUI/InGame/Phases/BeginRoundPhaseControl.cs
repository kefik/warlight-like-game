namespace WinformsUI.InGame.Phases
{
    using System;
    using System.Windows.Forms;

    public partial class BeginRoundPhaseControl : UserControl
    {
        public BeginRoundPhaseControl()
        {
            InitializeComponent();

            beginButton.Enabled = false;
        }

        public event Action OnBegin;
        public event Action OnWatch;

        private void BeginRound(object sender, EventArgs e)
        {
            OnBegin?.Invoke();
        }

        private void WatchRound(object sender, EventArgs e)
        {
            OnWatch?.Invoke();
            beginButton.Enabled = true;
        }
    }
}
