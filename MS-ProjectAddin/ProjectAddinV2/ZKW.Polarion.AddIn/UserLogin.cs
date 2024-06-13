using System;
using System.Windows.Forms;

namespace ZKW.Polarion.AddIn
{
    public partial class UserLogin : Form
    {
        public string USER = string.Empty;
        public string PASSWORD = string.Empty;

        public UserLogin()
        {
            InitializeComponent();
        }

        private void BtOK_Click(object sender, EventArgs e)
        {
            USER = userField.Text;
            PASSWORD = passwordField.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
