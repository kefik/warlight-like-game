﻿using System;
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
    public partial class AIPlayerControl : UserControl
    {
        public AIPlayerControl()
        {
            InitializeComponent();
        }

        public Color PlayerColor
        {
            get { return this.colorButton.BackColor; }
            private set { colorButton.BackColor = value; }
        }
    }
}
