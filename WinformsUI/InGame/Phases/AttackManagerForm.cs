using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsUI.InGame.Phases
{
    public partial class AttackManagerForm : Form
    {
        int armyLowerLimit;

        public int ArmyLowerLimit
        {
            get { return armyLowerLimit; }
            set
            {
                if (value <= 0) throw new ArgumentException();

                armyLowerLimit = value;
                attackArmyNumericUpDown.Minimum = value;
                attackArmyNumericUpDown.Value = Math.Max(value, attackArmyNumericUpDown.Value);
            }
        }

        int armyUpperLimit;

        public int ArmyUpperLimit
        {
            get { return armyUpperLimit; }
            set
            {
                if (value <= 0) throw new ArgumentException();

                armyUpperLimit = value;
                attackArmyNumericUpDown.Maximum = value;
                attackArmyNumericUpDown.Value = Math.Min(value, attackArmyNumericUpDown.Value);
            }
        }

        public int AttackingArmy
        {
            get { return (int)attackArmyNumericUpDown.Value; }
        }

        public AttackManagerForm()
        {
            InitializeComponent();
        }

        private void Cancel(object sender, EventArgs e)
        {

            Close();

            DialogResult = DialogResult.Cancel;
        }

        private void Ok(object sender, EventArgs e)
        {

            Close();

            DialogResult = DialogResult.OK;
        }
    }
}
