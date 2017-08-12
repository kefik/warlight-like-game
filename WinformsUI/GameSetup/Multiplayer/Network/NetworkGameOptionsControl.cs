using System;
using System.Windows.Forms;
using GameObjectsLib.Game;
using GameObjectsLib.GameUser;

namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System.Threading.Tasks;

    public partial class NetworkGameOptionsControl : UserControl
    {
        Func<User> getUser;
        public Func<User> GetUser
        {
            get { return getUser; }
            set
            {
                getUser = value;
                networkNewGameSettingsControl.GetUser = value;
            }
        }

        Action<User> setUser;
        public Action<User> SetUser
        {
            get { return setUser; }
            set
            {
                setUser = value;
                networkNewGameSettingsControl.SetUser = value;
            }
        }

        public event Func<GameSeed, Task> OnGameCreated
        {
            add { networkNewGameSettingsControl.OnGameCreated += value; }
            remove { networkNewGameSettingsControl.OnGameCreated -= value; }
        }
        public NetworkGameOptionsControl()
        {
            InitializeComponent();
        }
    }
}
