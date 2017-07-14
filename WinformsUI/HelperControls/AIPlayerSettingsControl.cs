using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Windows.Forms;
using ConquestObjectsLib;
using ConquestObjectsLib.GameMap;
using ConquestObjectsLib.GameMap.Templates;

namespace WinformsUI.HelperControls
{
    public partial class AIPlayerSettingsControl : UserControl
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
        
        public AIPlayerSettingsControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Adds player to table to the last position.
        /// </summary>
        public void AddPlayer()
        {
            if (PlayersLimit >= PlayersCount) throw new Exception(); // TODO: cannot be added exception

            AIPlayerControl control = new AIPlayerControl("PC"); // TODO: generate unique name
            playersTableLayoutPanel.Controls.Add(control);
        }
        /// <summary>
        /// Removes the last player from the table.
        /// </summary>
        public void RemovePlayer()
        {
            playersTableLayoutPanel.Controls.RemoveAt(PlayersCount - 1); // TODO: might not work
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
    }
}
