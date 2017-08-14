using System;
using System.Windows.Forms;
using GameObjectsLib.Game;
using GameObjectsLib.GameUser;

namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameObjectsLib;

    public partial class NetworkGameOptionsControl : UserControl
    {
        public event Func<HumanPlayer, ICollection<AiPlayer>, string, int, Task> OnGameCreated
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
