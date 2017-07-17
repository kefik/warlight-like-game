using System.Windows.Forms;
using ConquestObjectsLib.GameUser;

namespace WinformsUI.GameSetup.Multiplayer.Network
{
    public partial class NetworkGameOptionsControl : UserControl
    {
        public User MyUser
        {
            get { return networkNewGameSettingsControl.MyUser; }
            set { networkNewGameSettingsControl.MyUser = value; }
        }
        public NetworkGameOptionsControl()
        {
            InitializeComponent();
        }
    }
}
