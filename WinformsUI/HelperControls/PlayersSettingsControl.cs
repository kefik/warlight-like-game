using System.Windows.Forms;

namespace WinformsUI.HelperControls
{
    public partial class PlayersSettingsControl : UserControl
    {
        int rowsOccupied;
        public PlayersSettingsControl()
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
            if (playersNumberNumericUpDown.Value > rowsOccupied) // added player
            {
                rowsOccupied++;
                // add new row
                this.playersTableLayoutPanel.SetRow(new AIPlayerControl(), rowsOccupied);
            }
            else if (playersNumberNumericUpDown.Value < rowsOccupied) // removed player
            {
                // TODO: remove last row

                rowsOccupied--;
            }
        }
    }
}
