﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using DatabaseMapping;
using GameObjectsLib.GameUser;
using GameObjectsLib;
using GameObjectsLib.Game;
using GameObjectsLib.GameMap;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkCommsDotNet.Connections.UDP;
using Newtonsoft.Json;
using ProtoBuf;
using WinformsUI.HelperControls;

namespace WinformsUI.GameSetup.Singleplayer
{
    public partial class SingleplayerNewGameSettingsControl : UserControl
    {
        readonly MyHumanPlayerControl myHumanPlayerControl;
        
        public User User
        {
            get { return myHumanPlayerControl.User; }
            set
            {
                myHumanPlayerControl.User = value;
            }
        }

        public event Action<GameObjectsLib.Game.Game> OnGameStarted;
        public SingleplayerNewGameSettingsControl()
        {
            InitializeComponent();
            
            myHumanPlayerControl = new MyHumanPlayerControl()
            {
                User = new LocalUser("Me"),
                Parent = myPlayerPanel,
                Dock = DockStyle.Fill,
            };
            myHumanPlayerControl.Show();

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
            try
            {
                Map map
                    = Map.Create(mapSettingsControl.MapName, mapSettingsControl.PlayersLimit, mapSettingsControl.MapTemplatePath);
                ICollection<Player> players = aiPlayerSettingsControl.GetPlayers();
                players.Add(myHumanPlayerControl.GetPlayer());

                GameObjectsLib.Game.Game game = GameObjectsLib.Game.Game.Create(GameType.SinglePlayer, map, players);
                
                OnGameStarted?.Invoke(game);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("One or more components required to start the game are missing! Please, reinstall the game!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
