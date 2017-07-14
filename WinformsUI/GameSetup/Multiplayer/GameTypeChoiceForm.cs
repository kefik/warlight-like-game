using System;
using System.Windows.Forms;
using ConquestObjectsLib;

namespace WinformsUI.GameSetup.Multiplayer
{
    /// <summary>
    /// In this form user decides what type of multiplayer game he wants to play. Will appear as dialog.
    /// </summary>
    public partial class GameTypeChoiceForm : Form
    {
        public GameTypeChoiceForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Indicates type of multiplayer game selected by the user.
        /// </summary>
        public GameType MultiplayerGameType
        {
            get
            {
                switch (multiplayerGameTypeComboBox.Text)
                {
                    case "Hotseat":
                        return GameType.MultiplayerHotseat;
                    case "Network":
                        return GameType.MultiplayerNetwork;
                    default:
                        return GameType.None;
                        
                }
            }
        }
        
        private void Cancel(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        
        private void Ok(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
