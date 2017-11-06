namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.Players;

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
