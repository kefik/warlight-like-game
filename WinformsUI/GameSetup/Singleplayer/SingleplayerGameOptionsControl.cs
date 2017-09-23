namespace WinformsUI.GameSetup.Singleplayer
{
    using System;
    using System.Windows.Forms;
    using GameObjectsLib.Game;

    public partial class SingleplayerGameOptionsControl : UserControl
    {
        public event Action<Game> OnNewGameStarted
        {
            add { singleplayerNewGameSettingsControl.OnGameStarted += value; }
            remove { singleplayerNewGameSettingsControl.OnGameStarted -= value; }
        }

        public event Action<Game> OnGameLoaded
        {
            add { singleplayerLoadGamesControl.OnSingleplayerGameLoaded += value; }
            remove
            {
                {
                    singleplayerLoadGamesControl.OnSingleplayerGameLoaded -= value;
                }
            }
        }

        public SingleplayerGameOptionsControl()
        {
            InitializeComponent();
        }
    }
}
