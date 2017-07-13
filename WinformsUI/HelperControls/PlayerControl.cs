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
    public partial class PlayerControl : UserControl
    {
        public PlayerControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Accesses player color.
        /// </summary>
        public Color PlayerColor
        {
            get { return this.colorButton.BackColor; }
            // private set { colorButton.BackColor = value; }
        }
        

        /// <summary>
        /// Accesses player name.
        /// </summary>
        public string PlayerName
        {
            get { return playerName.Text; }
            private set { playerName.Text = value; }
        }
    }
}
