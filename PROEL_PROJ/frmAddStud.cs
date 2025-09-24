using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROEL_PROJ
{
    public partial class frmAddStud : Form
    {
        public frmAddStud()
        {
            InitializeComponent();
        }

        private void frmAddStud_Load(object sender, EventArgs e)
        {
            Classes.transparent(btnBack);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud stud = new frmUpdate_Stud();
            this.Hide();
            stud.ShowDialog();
        }
    }
}
