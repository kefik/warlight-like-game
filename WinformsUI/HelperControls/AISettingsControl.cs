using System;
using System.Windows.Forms;

namespace WinformsUI.HelperControls
{
    public partial class AISettingsControl : UserControl
    {
        int rowsOccupied;

        public AISettingsControl()
        {
            InitializeComponent();
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
                // add new row
                this.aiPlayersTableLayoutPanel.SetRow(new AIPlayerControl(), rowsOccupied);
            }
            else if (aiPlayersNumberNumericUpDown.Value < rowsOccupied) // removed player
            {
                // TODO: remove last row
                
                rowsOccupied--;
            }
        }
    }
}
