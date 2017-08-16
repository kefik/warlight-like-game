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
            LoadTableAsync();
        }

        async void LoadTableAsync()
        {
            var user = Global.MyUser as MyNetworkUser;

            if (user == null) throw new ArgumentException();

            gameHeaders = await user.GetListOfMyGamesAsync() ?? new List<GameHeaderMessageObject>();

            foreach (var gameHeader in gameHeaders)
            {
                if (gameHeader.GetType() == typeof(OpenedGameHeaderMessageObject))
                {
                    Invoke(new Action(() => multiDayListBox.Items.Add(gameHeader)));
                }
                else if (gameHeader.GetType() == typeof(StartedGameHeaderMessageObject))
                {
                    Invoke(new Action(() => multiDayListBox.Items.Add(gameHeader)));
                }
            }
        }

        private void OpenButtonClick(object sender, EventArgs e)
        {

        }
    }
}
