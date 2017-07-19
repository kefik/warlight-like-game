using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseMapping;
using GameObjectsLib.GameMap;

namespace WinformsUI.HelperControls
{
    public partial class MapSettingsControl : UserControl
    {
        List<MapInfo> maps;
        public MapSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Limit of players for given map.
        /// </summary>
        public int PlayersLimit
        {
            get { return ChosenMapInfo.PlayersLimit; }
        }

        /// <summary>
        /// Returns name of the map loaded in this control.
        /// </summary>
        public string MapName
        {
            get { return ChosenMapInfo.Name; }
        }
        /// <summary>
        /// Specifies path to map template.
        /// </summary>
        public string MapTemplatePath
        {
            get { return ChosenMapInfo.TemplatePath; }
        }

        MapInfo ChosenMapInfo
        {
            get
            {
                // if maps havent been downloaded from database
                if (maps == null || maps.Count - 1 > mapComboBox.SelectedIndex)
                {
                    return new MapInfo()
                    {
                        Id = -1,
                        PlayersLimit = 0,
                        Name = ""
                    };
                }
                return maps[mapComboBox.SelectedIndex];
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
            mapPlayersLimitLabel.Text = ChosenMapInfo.PlayersLimit.ToString();
        }

        void RefreshComboBox()
        {
            foreach (var map in maps)
            {
                mapComboBox.Items.Add(map.Name);
            }
            this.mapComboBox.Refresh();
        }
        
        private void ControlLoad(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                // clear combo box items
                this.Invoke(new Action(mapComboBox.Items.Clear));
                // get map infos from database
                var mapInfos = new UtilsDbContext().Maps;
                this.maps = mapInfos.ToList();
                // refresh the combo box with new items
                this.Invoke(new Action(RefreshComboBox));
            });
        }
    }

}

