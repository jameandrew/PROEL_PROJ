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
    public partial class frmCourse : Form
    {
        public frmCourse()
        {
            InitializeComponent();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.ShowDialog();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud stud = new frmUpdate_Stud();
            this.Hide();
            stud.ShowDialog();
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
            frmTeachers teachers = new frmTeachers();
            this.Hide();
            teachers.ShowDialog();
        }
    }
}
