using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FlightWatch
{
    public partial class EULAForm : Form
    {

        private bool agreedToEULA = false;

        public EULAForm()
        {
            InitializeComponent();
        }

        private void buttonDisagree_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void buttonAgree_Click(object sender, EventArgs e)
        {
            agreedToEULA = true;
            EULATextbox.SaveFile("EULA.txt", RichTextBoxStreamType.PlainText);
            this.Close();
        }

        private void EULAClosed(object sender, FormClosedEventArgs e)
        {
            if (!agreedToEULA)
            {
                MessageBox.Show("You must agree to the EULA in order to use FlightWatch.", "EULA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                System.Windows.Forms.Application.Exit();
            }
            
        }
    }
}
