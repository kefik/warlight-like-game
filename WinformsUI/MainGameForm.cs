using System.Windows.Forms;

namespace WinformsUI
{
    public partial class MainGameForm : Form
    {
        public MainGameForm()
        {
            InitializeComponent();
            var g = new GameSetup.Multiplayer.Network.NewGameSettingsControl()
            {
                Parent = this.multiplayerTabPage,
                Dock = DockStyle.Fill
            };
            g.Show();


        }
    }
}
