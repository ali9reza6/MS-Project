using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZKW.Polarion.AddIn
{
    public partial class Progress : Form
    {
        public void SetProgress(string anzeige)
        {
            this.Message.Text = anzeige;
            this.Message.Invalidate();
            this.Message.Update();
            this.BringToFront();
        }

        public Progress()
        {
            InitializeComponent();
        }

    }
}
