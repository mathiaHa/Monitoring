//
// Sample DataMailbox Viewer application. Delivered as Talk2M DataMailbox SDK sample code. Not supported by eWON s.a.
//
// Disclaimer: 
//    Delivered as is. eWON s.a. takes no responsibility for anything bad that can result from the use of this code.
//    Actual mileage may vary. Price does not include tax, title, and license. Some assembly required. Each sold 
//    separately. Batteries not included. Objects in mirror are closer than they appear. If conditions persist, 
//    contact a physician. Keep out of reach of children. Avoid prolonged exposure to direct sunlight. Keep in a cool 
//    dark place.
//    You've been warned!
//
// Third-party components used in these examples:
//    - JSonViewer : http://jsonviewer.codeplex.com
//    - FlUrl (nuGet package)
//

using System;
using System.Windows.Forms;

namespace DMBoxViewer
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            txtTalk2MDevId.Text = Properties.Settings.Default.Talk2MDevId;
            txtAccount.Text = Properties.Settings.Default.Account;
            txtUsername.Text = Properties.Settings.Default.Username;
            txtPassword.Text = Properties.Settings.Default.Password;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Talk2MDevId = txtTalk2MDevId.Text.Trim();
            Properties.Settings.Default.Account = txtAccount.Text.Trim();
            Properties.Settings.Default.Username = txtUsername.Text.Trim();
            Properties.Settings.Default.Password = txtPassword.Text.Trim();

            Properties.Settings.Default.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
