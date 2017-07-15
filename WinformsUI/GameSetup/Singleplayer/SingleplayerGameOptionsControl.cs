using System;
using System.Windows.Forms;

namespace WinformsUI.GameSetup.Singleplayer
{
    public partial class SingleplayerGameOptionsControl : UserControl
    {
        public event Action<ConquestObjectsLib.Game.Game> OnGameStarted
        {
            add { singleplayerNewGameSettingsControl.OnGameStarted += value; }
            remove { singleplayerNewGameSettingsControl.OnGameStarted -= value; }
        }

        public SingleplayerGameOptionsControl()
        {
            InitializeComponent();
        }
    }
}
