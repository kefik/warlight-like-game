using System;
using System.Windows.Forms;
using GameObjectsLib.Game;
using GameObjectsLib.GameUser;

namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    public partial class HotseatGameOptionsControl : UserControl
    {
        Func<User> getUser;
        public Func<User> GetUser
        {
            get { return getUser; }
            set
            {
                getUser = value;
                hotseatNewGameSettingsControl.GetUser = value;
            }
        }

        Action<User> setUser;
        public Action<User> SetUser
        {
            get { return setUser; }
            set
            {
                setUser = value;
                hotseatNewGameSettingsControl.SetUser = value;
            }
        }
        
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
