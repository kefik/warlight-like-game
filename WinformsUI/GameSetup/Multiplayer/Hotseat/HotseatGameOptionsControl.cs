using System;
using System.Windows.Forms;

namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    public partial class HotseatGameOptionsControl : UserControl
    {
        public event Action<ConquestObjectsLib.Game.Game> OnGameStarted
        {
            add { hotseatNewGameSettingsControl.OnGameStarted += value; }
            remove { hotseatNewGameSettingsControl.OnGameStarted -= value; }
        }
        public HotseatGameOptionsControl()
        {
            InitializeComponent();
        }
    }
}
