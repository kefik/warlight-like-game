namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.NetworkCommObjects.Message;

    public partial class OpenedGamesControl : UserControl
    {
        IEnumerable<OpenedGameHeaderMessageObject> gameHeaders;

        public OpenedGamesControl()
        {
            InitializeComponent();
        }

        void ControlLoad(object sender, EventArgs e)
        {
            LoadTableAsync();
        }

        async void LoadTableAsync()
        {
            MyNetworkUser user = Global.MyUser as MyNetworkUser;

            if (user == null) throw new ArgumentException();

            gameHeaders = await user.GetListOfOpenedGamesAsync() ?? new List<OpenedGameHeaderMessageObject>();

            foreach (var gameHeader in gameHeaders) Invoke(new Action(() => multiDayListBox.Items.Add(gameHeader)));
        }
    }
}
