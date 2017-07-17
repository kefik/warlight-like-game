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
using ConquestObjectsLib.GameMap;
using DatabaseMappingLib;
using DatabaseMappingLib.UtilsDb;

namespace WinformsUI.HelperControls
{
    public partial class MapSettingsControl : UserControl
    {
        List<MapInfo> maps;
        public MapSettingsControl()
        {
            InitializeComponent();

            OnMapChosen += (sender, args) =>
            {
                mapPlayersLimitLabel.Text = PlayersLimit.ToString();
            };
            

        }

        /// <summary>
        /// Limit of players for given map.
        /// </summary>
        public int PlayersLimit { get; private set; }

        /// <summary>
        /// Returns name of the map loaded in this control.
        /// </summary>
        public string MapName { get; private set; }

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
            PlayersLimit = maps[mapComboBox.SelectedIndex].PlayersLimit;
            MapName = maps[mapComboBox.SelectedIndex].Name;
        }

        void RefreshComboBox()
        {
            foreach (var map in maps)
            {
                mapComboBox.Items.Add(map.Name);
            }
            this.mapComboBox.Refresh();
        }

        // TODO: verify if it rly worksh

        private void ControlLoad(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                this.Invoke(new Action(mapComboBox.Items.Clear));

                var mapInfos = new UtilsDbContext().Maps;
                this.maps = mapInfos.ToList();

                this.Invoke(new Action(RefreshComboBox));
            });
        }
    }

}

