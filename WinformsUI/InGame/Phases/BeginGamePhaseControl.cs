namespace WinformsUI.InGame.Phases
{
    using System;
    using System.Windows.Forms;
    using GameObjectsLib;

    public partial class BeginGamePhaseControl : UserControl
    {
        public BeginGamePhaseControl()
        {
            InitializeComponent();
        }

        public event Action<GameBeginningRound> OnStartOver;
        public event Action<GameBeginningRound> OnCommit;
        public GameBeginningRound BeginningRound { get; } = new GameBeginningRound();

        private void Commit(object sender, EventArgs e)
        {
            if (BeginningRound.SelectedRegions.Count < 2)
            {
                MessageBox.Show("Not enough regions were chosen to start the game.");
                return;
            }
            commitButton.Enabled = false;
            OnCommit?.Invoke(BeginningRound);
        }

        private void StartOver(object sender, EventArgs e)
        {
            commitButton.Enabled = true;
            OnStartOver?.Invoke(BeginningRound);
        }
    }
}
