using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using GameObjectsLib;

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
