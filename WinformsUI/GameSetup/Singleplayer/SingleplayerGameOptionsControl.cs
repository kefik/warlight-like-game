using System;
using System.Windows.Forms;
using GameObjectsLib;
using GameObjectsLib.GameUser;

namespace WinformsUI.GameSetup.Singleplayer
{
    public partial class SingleplayerGameOptionsControl : UserControl
    {
        User user;

        public User User
        {
            get { return user; }
            set
            {
                user = value;
                singleplayerNewGameSettingsControl.User = value;
            }
        }
        public event Action<GameObjectsLib.Game.Game> OnGameStarted
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
