using System;
using System.Windows.Forms;
using ConquestObjectsLib;
using ConquestObjectsLib.GameMap;

namespace WinformsUI.HelperControls
{
    public partial class AISettingsControl : UserControl
    {
        int rowsOccupied;
        // TODO: map number of player limits problem
        public AISettingsControl()
        {
            InitializeComponent();
        }

        public void AddPlayerRow(Player player)
        {
            
        }

        public void RemovePlayerRow(int index)
        {
            
        }
        /// <summary>
        /// Player was added or removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayersNumberChanged(object sender, System.EventArgs e)
        {
            if (aiPlayersNumberNumericUpDown.Value > rowsOccupied) // added player
            {
                rowsOccupied++;
                aiPlayersTableLayoutPanel.Controls.Add(new AIPlayerControl("PC"));
            }
            else if (aiPlayersNumberNumericUpDown.Value < rowsOccupied) // removed player
            {
                rowsOccupied--;
                aiPlayersTableLayoutPanel.Controls.RemoveAt(rowsOccupied);
                
            }
        }
    }
}
