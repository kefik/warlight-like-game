using System;
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

namespace WinformsUI.GameSetup.Singleplayer
{
    public partial class SingleplayerNewGameSettingsControl : UserControl
    {
        public event Action<ConquestObjectsLib.Game.Game> OnGameStarted;
        public SingleplayerNewGameSettingsControl()
        {
            InitializeComponent();

            previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit;
            aiPlayerSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit;
            // when the map is chosen, update maximum values
            mapSettingsControl.OnMapChosen += (o, e) =>
            {
                this.aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit;
            };
            mapSettingsControl.OnMapChosen += (o, e) =>
            {
                aiPlayerSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit;
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

        private void Create(object sender, EventArgs e)
        {
            Map map = MapFactory.GetMap(mapSettingsControl.GameMap);
            ICollection<Player> players = aiPlayerSettingsControl.GetPlayers(); /*as ICollection<Player>; */

            ConquestObjectsLib.Game.Game game = new SingleplayerGame(map, players);

            OnGameStarted?.Invoke(game);
        }
    }
}
