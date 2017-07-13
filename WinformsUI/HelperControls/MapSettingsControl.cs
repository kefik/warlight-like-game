using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsUI.HelperControls
{
    public enum Map
    {
        None, World
    }
    public partial class MapSettingsControl : UserControl
    {
        public MapSettingsControl()
        {
            InitializeComponent();
        }

        public Map GameMap
        {
            get
            {
                switch (this.mapComboBox.Text)
                {
                    case "World":
                        return Map.World;
                    default:
                        return Map.None;
                }
            }
        }
    }
}
