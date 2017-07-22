using System;
using System.Windows.Forms;

namespace WinformsUI.InGame.Phases
{
    public partial class BeginRoundPhaseControl : UserControl
    {
        public BeginRoundPhaseControl()
        {
            InitializeComponent();
        }

        public event Action OnRoundBegin;
        public event Action OnRoundRepeat;

        private void BeginRound(object sender, System.EventArgs e)
        {
            OnRoundBegin?.Invoke();
        }

        private void RepeatRound(object sender, System.EventArgs e)
        {
            OnRoundRepeat?.Invoke();
        }
    }
}
