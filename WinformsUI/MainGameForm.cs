using System.Drawing;
using System.Windows.Forms;

namespace WinformsUI
{
    public partial class MainGameForm : Form
    {
        public MainGameForm()
        {
            InitializeComponent();
            var g = new GameSetup.Singleplayer.GameOptionsControl()
            {
                Parent = this.singleplayerTabPage,
                Dock = DockStyle.Fill
                //BackColor = Color.Red
            };
            g.Show();

            var f = new GameSetup.Multiplayer.Hotseat.GameOptionsControl()
            {
                Parent = this.multiplayerTabPage,
                Dock = DockStyle.Fill
            };
            f.Show();


        }
    }
}
