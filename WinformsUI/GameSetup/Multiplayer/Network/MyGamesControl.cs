using System.Windows.Forms;

namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.NetworkCommObjects.Message;

    public partial class MyGamesControl : UserControl
    {
        public Func<MyNetworkUser> GetUser;
        IEnumerable<GameHeaderMessageObject> gameHeaders;

        public MyGamesControl()
        {
            InitializeComponent();
        }

        private void ControlLoad(object sender, System.EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var user = GetUser();
            });
        }
    }
}
