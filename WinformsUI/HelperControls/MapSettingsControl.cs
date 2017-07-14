using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConquestObjectsLib.GameMap;
using ConquestObjectsLib.GameMap.Templates;

namespace WinformsUI.HelperControls
{
    public partial class MapSettingsControl : UserControl
    {
        public MapSettingsControl()
        {
            InitializeComponent();

            GameMap = MapType.None;
            PlayersLimit = 0;
        }

        int playersLimit;
        /// <summary>
        /// Limit of players for given map.
        /// </summary>
        public int PlayersLimit
        {
            get { return playersLimit; }
            private set
            {
                playersLimit = value;
                mapPlayersLimitLabel.Text = value.ToString();
            }
        }
        /// <summary>
        /// Type of map that will be played.
        /// </summary>
        public MapType GameMap
        {
            get;
            private set;
        }
        
        private void MapChosen(object sender, EventArgs e)
        {
            switch (mapComboBox.Text)
            {
                case nameof(World):
                    GameMap = MapType.World;
                    PlayersLimit = World.PlayersLimit;
                    break;
                default:
                    GameMap = MapType.None;
                    PlayersLimit = 0;
                    break;
            }
        }
    }
}
