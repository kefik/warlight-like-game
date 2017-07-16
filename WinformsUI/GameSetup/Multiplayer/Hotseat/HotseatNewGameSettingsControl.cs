﻿using System;
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

namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    public partial class HotseatNewGameSettingsControl : UserControl
    {
        private User myUser = new User() { Name = "Me" };
        /// <summary>
        /// This instance represenets my user.
        /// </summary>
        public User MyUser
        {
            get { return myUser; }
            set
            {
                myPlayerControl.User = value;
                myUser = value;
            }
        }

        readonly HumanPlayerControl myPlayerControl;
        /// <summary>
        /// Represents number of total players that can play this given map.
        /// </summary>
        int TotalPlayersLimit
        {
            get { return mapSettingsControl.PlayersLimit; }
        }

        public HotseatNewGameSettingsControl()
        {
            InitializeComponent();

            myPlayerControl = new HumanPlayerControl()
            {
                User = MyUser,
                Parent = myUserPanel,
                Dock = DockStyle.Fill
            };
            myPlayerControl.Show();

            // TODO: get the code better
            mapSettingsControl.OnMapChosen += (sender, args) =>
            {
                int maxOtherPlayers = Math.Max(0, TotalPlayersLimit - 1);

                aiPlayersNumberNumericUpDown.Maximum = maxOtherPlayers;
                humanPlayersNumberNumericUpDown.Maximum = maxOtherPlayers;

                aiPlayerSettingsControl.PlayersLimit = maxOtherPlayers;
                humanPlayerSettingsControl.PlayersLimit = maxOtherPlayers;
            };
            // initialize
            int maxPlayers = Math.Max(0, TotalPlayersLimit - 1);
            aiPlayersNumberNumericUpDown.Maximum = maxPlayers;
            humanPlayersNumberNumericUpDown.Maximum = maxPlayers;
            

            aiPlayerSettingsControl.PlayersLimit = maxPlayers;
            humanPlayerSettingsControl.PlayersLimit = maxPlayers;


            previousAIPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            previousHumanPlayersNumber = humanPlayersNumberNumericUpDown.Value;
        }

        public event Action<ConquestObjectsLib.Game.Game> OnGameStarted;

        decimal previousAIPlayersNumber;
        private void OnNumberOfAIPlayersChanged(object sender, EventArgs e)
        {
            if (!DoesSumToPlayersLimit(aiPlayersNumberNumericUpDown.Value, previousHumanPlayersNumber))
            {
                aiPlayersNumberNumericUpDown.Value = previousAIPlayersNumber;
                return;
            }

            decimal difference;
            if (previousAIPlayersNumber < aiPlayersNumberNumericUpDown.Value)
            {
                difference = aiPlayersNumberNumericUpDown.Value - previousAIPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.AddPlayer();
                }
            }
            else
            {
                difference = previousAIPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.RemovePlayer();
                }
            }
            
            previousAIPlayersNumber = aiPlayersNumberNumericUpDown.Value;
        }

        decimal previousHumanPlayersNumber;
        private void OnNumberOfHumanPlayersChanged(object sender, EventArgs e)
        {
            if (!DoesSumToPlayersLimit(humanPlayersNumberNumericUpDown.Value, previousAIPlayersNumber))
            {
                humanPlayersNumberNumericUpDown.Value = previousHumanPlayersNumber;
                return;
            }

            decimal difference;
            if (previousHumanPlayersNumber < humanPlayersNumberNumericUpDown.Value)
            {
                difference = humanPlayersNumberNumericUpDown.Value - previousHumanPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    humanPlayerSettingsControl.AddPlayer(new User()); // TODO: problem with users
                }
            }
            else
            {
                difference = previousHumanPlayersNumber - humanPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    humanPlayerSettingsControl.RemovePlayer(); // TODO: problem with users
                }
            }
            
            previousHumanPlayersNumber = humanPlayersNumberNumericUpDown.Value;
        }

        bool DoesSumToPlayersLimit(decimal aiPlayers, decimal humanPlayers)
        {
            return aiPlayers + humanPlayers <= Math.Max(0, TotalPlayersLimit - 1);
        }

        private void Start(object sender, EventArgs e)
        {
            Map map = Map.Create(mapSettingsControl.GameMap);

            var players = aiPlayerSettingsControl.GetPlayers();
            
            foreach (var player in humanPlayerSettingsControl.GetPlayers())
            {
                players.Add(player);
            }

            ConquestObjectsLib.Game.Game game = new HotseatGame(map, players);

            OnGameStarted?.Invoke(game);
        }
    }
}
