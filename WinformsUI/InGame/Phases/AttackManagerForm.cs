namespace WinformsUI.InGame.Phases
{
    using System;
    using System.Windows.Forms;

    public partial class AttackManagerForm : Form
    {
        private int armyLowerLimit;

        public int ArmyLowerLimit
        {
            get { return armyLowerLimit; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }

                armyLowerLimit = value;
                attackArmyNumericUpDown.Minimum = value;
                attackArmyNumericUpDown.Value = Math.Max(value, attackArmyNumericUpDown.Value);
            }
        }

        private int armyUpperLimit;

        public int ArmyUpperLimit
        {
            get { return armyUpperLimit; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }

                armyUpperLimit = value;
                attackArmyNumericUpDown.Maximum = value;
                attackArmyNumericUpDown.Value = Math.Min(value, attackArmyNumericUpDown.Value);
            }
        }

        public int AttackingArmy
        {
            get { return (int) attackArmyNumericUpDown.Value; }
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
