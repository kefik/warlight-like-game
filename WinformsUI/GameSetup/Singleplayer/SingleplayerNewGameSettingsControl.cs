﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConquestObjectsLib;
using ConquestObjectsLib.Game;
using ConquestObjectsLib.GameMap;
using WinformsUI.HelperControls;

namespace WinformsUI.GameSetup.Singleplayer
{
    public partial class SingleplayerNewGameSettingsControl : UserControl
    {
        HumanPlayerControl humanPlayerControl;

        User user;

        public User User
        {
            get { return user; }
            set
            {
                user = value;
                humanPlayerControl.User = value;
            }
        }

        public event Action<ConquestObjectsLib.Game.Game> OnGameStarted;
        public SingleplayerNewGameSettingsControl()
        {
            InitializeComponent();

            user = new User() {Name = "Me"};
            humanPlayerControl = new HumanPlayerControl(user)
            {
                Parent = myPlayerPanel,
                Dock = DockStyle.Fill,
            };
            humanPlayerControl.Show();

            previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit;
            aiPlayerSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit;
            // when the map is chosen, update maximum values
            mapSettingsControl.OnMapChosen += (o, e) =>
            {
                this.aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit - 1;
                aiPlayerSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit - 1;
            };
        }
        

        decimal previousPlayersNumber;
        private void PlayersNumberChanged(object sender, EventArgs e)
        {
            if (previousPlayersNumber < aiPlayersNumberNumericUpDown.Value)
            {
                decimal difference = aiPlayersNumberNumericUpDown.Value - previousPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.AddPlayer();
                }
                previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            }
            else if (previousPlayersNumber > aiPlayersNumberNumericUpDown.Value)
            {
                decimal difference = previousPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.RemovePlayer();
                }
                previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            }
        }
        

        private void Start(object sender, EventArgs e)
        {
            Map map = Map.Create(mapSettingsControl.GameMap);
            ICollection<Player> players = aiPlayerSettingsControl.GetPlayers(); /*as ICollection<Player>; */

            // adds clients player
            players.Add(humanPlayerControl.GetPlayer());

            ConquestObjectsLib.Game.Game game = new SingleplayerGame(map, players);

            OnGameStarted?.Invoke(game);
        }
    }
}
