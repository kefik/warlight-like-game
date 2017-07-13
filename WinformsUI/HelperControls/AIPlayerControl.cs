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
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public partial class AIPlayerControl : UserControl
    {
        public AIPlayerControl()
        {
            difficultyComboBox.SelectedIndex = (int) Difficulty.Medium;

            InitializeComponent();
        }
        
        /// <summary>
        /// Accesses AI player color.
        /// </summary>
        public Color PlayerColor
        {
            get { return this.colorButton.BackColor; }
            private set { colorButton.BackColor = value; }
        }

        /// <summary>
        /// Accesses AI player difficulty.
        /// </summary>
        public Difficulty PlayerDifficulty
        {
            get
            {
                // what is in combo box
                switch (difficultyComboBox.Text)
                {
                    case "Easy":
                        return Difficulty.Easy;
                    case "Medium":
                        return Difficulty.Medium;
                    case "Hard":
                        return Difficulty.Hard;
                    default:
                        throw new ArgumentException();
                }
            }

        }

        /// <summary>
        /// Accesses AI player name.
        /// </summary>
        public string PlayerName
        {
            get { return aiNameLabel.Text; }
            private set { aiNameLabel.Text = value; }
        }
    }
}
