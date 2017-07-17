using System;
using System.Windows.Forms;
using ConquestObjectsLib.GameUser;

namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    public partial class HotseatGameOptionsControl : UserControl
    {
        public User MyUser
        {
            get { return hotseatNewGameSettingsControl.MyUser; }
            set { hotseatNewGameSettingsControl.MyUser = value; }
        }
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
