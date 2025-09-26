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
    public partial class frmDashboard : Form
    {
        public frmDashboard()
        {
            InitializeComponent();
            
        }

        string connectionString = Classes.ConString();
        private void frmDashboard_Load(object sender, EventArgs e)
        {
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);

            using (SqlConnection sqlCOn = new SqlConnection(connectionString))
            {
                sqlCOn.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT r.Firstname, r.Lastname,  r.Age, r.Gender, r.Email, ro.RoleName " +
                    "FROM Profiles r INNER JOIN Roles ro ON r.RoleID = ro.RoleID WHERE ro.RoleName <> 'ADMIN' AND ro.RoleName <> 'INSTRUCTOR' ORDER BY ProfileID DESC", sqlCOn);
                DataTable dt = new DataTable();
                sqlDa.Fill(dt);

                dgvTotal_Stud.DataSource = dt;
            }
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud update = new frmUpdate_Stud();
            this.Hide();
            update.ShowDialog();
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
            frmTeachers teachers = new frmTeachers();
            this.Hide();
            teachers.ShowDialog();
        }

        private void btnLogOut_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Would you like to Log out?", "Confirmation",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                frmLogIn logIn = new frmLogIn();
                this.Hide();
                logIn.ShowDialog();
            }
            else { }
        }
    }
}
