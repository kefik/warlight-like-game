using System;
using System.Windows.Forms;
using GameObjectsLib.Game;
using GameObjectsLib.GameUser;

namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    public partial class HotseatGameOptionsControl : UserControl
    {
        public event Action<Game> OnGameStarted
        {
            add { hotseatNewGameSettingsControl.OnGameStarted += value; }
            remove { hotseatNewGameSettingsControl.OnGameStarted -= value; }
        }
        public event Action<Game> OnGameLoaded
        {
            add { hotseatLoadGamesControl.OnHotseatGameLoaded += value; }
            remove { hotseatLoadGamesControl.OnHotseatGameLoaded -= value; }
        }
        public HotseatGameOptionsControl()
        {
            InitializeComponent();
        }
    }
}
