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
        Map map;
        public MapSettingsControl()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Limit of players for given map.
        /// </summary>
        public int PlayersLimit
        {
            get
            {
                return map == null ? 0 : map.PlayersLimit;
            }
        }
        /// <summary>
        /// Type of map that will be played.
        /// </summary>
        public MapType GameMap
        {
            get
            {
                return map == null ? MapType.None : map.MapType;
            }
        }

        /// <summary>
        /// Runs when the map is chosen.
        /// </summary>
        public event EventHandler OnMapChosen
        {
            add { mapComboBox.SelectedIndexChanged += value; }
            remove { mapComboBox.SelectedIndexChanged -= value; }
        }
        private void MapChosen(object sender, EventArgs e)
        {
            switch (mapComboBox.Text)
            {
                case "World":
                    map = Map.ConstructMap(MapType.World);
                    break;
            }
        }
    }
}
