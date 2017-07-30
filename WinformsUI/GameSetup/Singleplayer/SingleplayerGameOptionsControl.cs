using System;
using System.Windows.Forms;
using GameObjectsLib.Game;
using GameObjectsLib.GameUser;

namespace WinformsUI.GameSetup.Singleplayer
{
    public partial class SingleplayerGameOptionsControl : UserControl
    {
        Func<User> getUser;
        public Func<User> GetUser
        {
            get { return getUser; }
            set
            {
                getUser = value;
                singleplayerNewGameSettingsControl.GetUser = value;
            }
        }

        Action<User> setUser;
        public Action<User> SetUser
        {
            get { return setUser; }
            set
            {
                setUser = value;
                singleplayerNewGameSettingsControl.SetUser = value;
            }
        }

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
