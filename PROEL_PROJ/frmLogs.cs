using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROEL_PROJ
{
    public partial class frmLogs : Form
    {
        public frmLogs()
        {
            InitializeComponent();
        }

        string connectionString = Classes.ConString();
        private void frmLogs_Load(object sender, EventArgs e)
        {
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);
            Classes.ApplySidebarStyle(btnCourse);
            Logs.LoadLogs(dgvLogs);
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmDashboard frmDashboard = new frmDashboard();
            this.Hide();
            frmDashboard.ShowDialog();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud frmUpdate_Stud = new frmUpdate_Stud();
            this.Hide();
            frmUpdate_Stud.ShowDialog();
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
            frmTeachers frmTeachers = new frmTeachers();
            this.Hide();
            frmTeachers.ShowDialog();
        }

        private void btnCourse_Click(object sender, EventArgs e)
        {
            frmCourse frmCourse = new frmCourse();
            this.Hide();
            frmCourse.ShowDialog();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //string keyword = txtSearch.Text.Trim();
            //Course.SearchLogs(dgvLogs);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            Logs.SearchLogs(dgvLogs, txtSearch.Text.Trim());
        }
    }
}
