namespace WinformsUI.HelperControls
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client.Entities;
    using GameObjectsLib.GameMap;

    public partial class MapSettingsControl : UserControl
    {
        private List<MapInfo> maps;

        public MapSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Limit of players for given map.
        /// </summary>
        public int PlayersLimit
        {
            get { return ChosenMapInfo.PlayersLimit; }
        }

        public string MapName
        {
            get { return ChosenMapInfo.Name; }
        }

        private MapInfo ChosenMapInfo
        {
            get
            {
                // if maps havent been downloaded from database
                if (maps == null || maps.Count - 1 > mapComboBox.SelectedIndex)
                {
                    return new MapInfo
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
        ///     Creates and returns new instance of map based on parameters.
        /// </summary>
        /// <returns>New instance of the map.</returns>
        public Map GetMap()
        {
            return new Map(ChosenMapInfo.Id, ChosenMapInfo.Name, ChosenMapInfo.PlayersLimit,
                ChosenMapInfo.TemplatePath);
        }

        /// <summary>
        ///     Runs when the map is chosen.
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

        private void RefreshComboBox()
        {
            foreach (MapInfo map in maps)
            {
                mapComboBox.Items.Add(map.Name);
            }
            mapComboBox.Refresh();
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();

            if (this.IsDisposed)
            {
                return;
            }
            Task.Run(() =>
            {
                // clear combo box items
                Invoke(new Action(mapComboBox.Items.Clear));
                // get map infos from database
                DbSet<MapInfo> mapInfos = new UtilsDbContext().Maps;
                maps = mapInfos.ToList();
                // refresh the combo box with new items
                Invoke(new Action(RefreshComboBox));
            });
        }

        private void ControlLoad(object sender, EventArgs e)
        {
        }
    }
}

