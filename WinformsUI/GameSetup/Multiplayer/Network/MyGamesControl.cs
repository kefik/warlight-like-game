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
        IEnumerable<GameHeaderMessageObject> gameHeaders;

        public MyGamesControl()
        {
            InitializeComponent();
        }

        private void ControlLoad(object sender, System.EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var user = Global.MyUser;
            });
        }
    }
}
