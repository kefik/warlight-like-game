using System;
using System.Windows.Forms;
using GameObjectsLib.Game;
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

        public Func<User> GetUser;
        public Action<User> SetUser;

        public event Action<Game> OnNewGameStarted
        {
            add
            {
                singleplayerNewGameSettingsControl.OnGameStarted += value;
            }
            remove
            {
                singleplayerNewGameSettingsControl.OnGameStarted -= value;
            }
        }
        public event Action<Game> OnGameLoaded
        {
            add { singleplayerLoadGamesControl.OnSingleplayerGameLoaded += value; }
            remove
            {
                { singleplayerLoadGamesControl.OnSingleplayerGameLoaded -= value; }
            }
        }

        public SingleplayerGameOptionsControl()
        {
            InitializeComponent();
        }
    }
}
