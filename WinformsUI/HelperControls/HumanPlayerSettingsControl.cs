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
    public partial class HumanPlayerSettingsControl : UserControl
    {
        /// <summary>
        /// Property giving count of players currently in the table.
        /// </summary>
        public int PlayersCount
        {
            get { return playersTableLayoutPanel.Controls.Count; }
        }
        /// <summary>
        /// Limit for players number.
        /// </summary>
        public int PlayersLimit { get; set; }

        public HumanPlayerSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds player to table to the last position.
        /// </summary>
        public void AddPlayer(User user)
        {
            if (PlayersLimit <= PlayersCount) throw new Exception(); // TODO: cannot be added exception

            HumanPlayerControl control = new HumanPlayerControl(user)
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            }; // TODO: generate unique name
            playersTableLayoutPanel.Controls.Add(control);
        }
        /// <summary>
        /// Removes the last player from the table.
        /// </summary>
        public void RemovePlayer()
        {
            if (PlayersCount == 0) throw new Exception();

            playersTableLayoutPanel.Controls.RemoveAt(PlayersCount - 1);
        }
        /// <summary>
        /// Removes player at given index from the table.
        /// </summary>
        /// <param name="index">Index specifying which row to remove.</param>
        public void RemovePlayer(int index)
        {
            if (index >= PlayersCount || index < 0) throw new ArgumentException();

            playersTableLayoutPanel.Controls.RemoveAt(index);
        }

        /// <summary>
        /// Calculates and returns players from controls saved in the table.
        /// </summary>
        /// <returns>Returns player from controls saved in the table.</returns>
        public ICollection<Player> GetPlayers()
        {
            ICollection<Player> players = new List<Player>();
            foreach (var control in playersTableLayoutPanel.Controls)
            {
                HumanPlayerControl humanPlayerControl = control as HumanPlayerControl;

                players.Add(humanPlayerControl.GetPlayer()); // TODO: may throw exception
            }

            return players;
        }
    }
}
