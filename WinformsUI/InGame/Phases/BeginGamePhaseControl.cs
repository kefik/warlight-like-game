using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameObjectsLib;
using GameObjectsLib.Game;

namespace WinformsUI.InGame.Phases
{
    public partial class BeginGamePhaseControl : UserControl
    {
        public BeginGamePhaseControl()
        {
            InitializeComponent();
        }

        public event Action OnStartOver;
        public event Action OnCommit;
        public GameBeginningRound BeginningRound { get; } = new GameBeginningRound();
    
        private void Commit(object sender, EventArgs e)
        {
            if (BeginningRound.SelectedRegions.Count < 2)
            {
                MessageBox.Show("Not enough regions were chosen to start the game.");
                return;
            }
            OnCommit?.Invoke();
        }

        private void StartOver(object sender, EventArgs e)
        {
            OnStartOver?.Invoke();
        }
    }
}
