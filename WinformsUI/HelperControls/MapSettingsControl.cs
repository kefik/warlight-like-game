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
        }

        public MapType GameMap
        {
            get
            {
                switch (this.mapComboBox.Text)
                {
                    case nameof(World):
                        return MapType.World;
                    default:
                        return MapType.None;
                }
            }
        }
    }
}
