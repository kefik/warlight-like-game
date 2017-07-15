using System;
using System.Drawing;
using System.Windows.Forms;
using ConquestObjectsLib;
using WinformsUI.Game;
using WinformsUI.GameSetup.Singleplayer;

namespace WinformsUI
{
    public partial class MainGameForm : Form
    {
        SingleplayerGameOptionsControl singleplayerGameOptions;
        InGameControl inGame;
        public MainGameForm()
        {
            InitializeComponent();
            singleplayerGameOptions = new SingleplayerGameOptionsControl()
            {
                Parent = this.singleplayerTabPage,
                Dock = DockStyle.Fill
            };
            singleplayerGameOptions.OnGameStarted += StartGame;
            singleplayerGameOptions.Show();

        }

        /// <summary>
        /// Loads proper screens starting newly created game the game.
        /// </summary>
        /// <param name="game">Instance representing the game to be started.</param>
        public void StartGame(ConquestObjectsLib.Game game)
        {
            // start the game
            game.Start();
            // removes previous
            singleplayerGameOptions?.Dispose();
            singleplayerGameOptions = null;
            // loads game screens
            inGame = new InGameControl()
            {
                Parent = singleplayerTabPage,
                Dock = DockStyle.Fill
            };
            inGame.Show();
            // TODO: fix starting phase
        }
    }
}
