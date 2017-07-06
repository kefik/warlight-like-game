using System.Drawing;
using System.Windows.Forms;

namespace WinformsUI
{
    public partial class MainGameForm : Form
    {
        public MainGameForm()
        {
            InitializeComponent();
            var g = new GameSetup.Multiplayer.Hotseat.NewGameSettingsControl()
            {
                Parent = this.singleplayerTabPage,
                Dock = DockStyle.Top
                //BackColor = Color.Red
            };
            g.Show();


        }
    }
}
