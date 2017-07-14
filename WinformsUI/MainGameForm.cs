using System.Drawing;
using System.Windows.Forms;
using ConquestObjectsLib;

namespace WinformsUI
{
    public partial class MainGameForm : Form
    {
        public MainGameForm()
        {
            InitializeComponent();
            var g = new GameSetup.Singleplayer.SinglepleplayerGameOptionsControl()
            {
                Parent = this.singleplayerTabPage,
                Dock = DockStyle.Fill
            };
            g.Show();

            var f = new GameSetup.Multiplayer.Hotseat.HotseatGameOptionsControl()
            {
                Parent = this.multiplayerTabPage,
                Dock = DockStyle.Fill
            };
            f.Show();
        }

        /// <summary>
        /// Loads proper screens starting the game.
        /// </summary>
        /// <param name="game">Instance representing the game to be started.</param>
        public void StartGame(ConquestObjectsLib.Game game)
        {
            game.Start(); // starts the game

            // TODO: load proper screens

        }
    }
}
