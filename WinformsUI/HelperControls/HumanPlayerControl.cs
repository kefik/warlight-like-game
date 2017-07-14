using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConquestObjectsLib;

namespace WinformsUI.HelperControls
{
    public partial class HumanPlayerControl : UserControl
    {
        HumanPlayer player;
        public HumanPlayerControl(User user)
        {
            InitializeComponent();

            player = new HumanPlayer(user, KnownColor.Aqua);
            
        }

        public HumanPlayerControl(HumanPlayer player)
        {
            InitializeComponent();

            this.player = player;
        }
        /// <summary>
        /// Accesses players controlling user.
        /// </summary>
        public User User
        {
            get { return player.User; }
        }

        /// <summary>
        /// Accesses player color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return player.Color; }
            private set
            {
                player = new HumanPlayer(player.User, value);
                colorButton.BackColor = Color.FromKnownColor(value);
            }
        }


        /// <summary>
        /// Accesses player name.
        /// </summary>
        public string PlayerName
        {
            get { return player.Name; }
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PlayerColor++;
                    break;
                case MouseButtons.Right:
                    PlayerColor--;
                    break;
            }
        }
    }
}
