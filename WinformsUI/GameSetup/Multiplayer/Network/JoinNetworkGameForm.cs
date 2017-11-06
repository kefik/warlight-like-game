namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.Player;

    public partial class JoinNetworkGameForm : Form
    {
        public JoinNetworkGameForm()
        {
            InitializeComponent();
        }

        public HumanPlayer GetPlayer()
        {
            return myHumanPlayerControl.GetPlayer();
        }

        private void Ok(object sender, EventArgs e)
        {
            Close();
            DialogResult = DialogResult.OK;
        }

        private void Cancel(object sender, EventArgs e)
        {
            Close();
            DialogResult = DialogResult.Cancel;
        }
    }
}
