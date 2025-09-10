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

        private string _username;
        Classes classes = new Classes();
        public frmDashboard(string username)
        {
            InitializeComponent();
            _username = username;
        }

        string connectionString = Classes.ConString();
        private void frmDashboard_Load(object sender, EventArgs e)
        {
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnUpdate);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);

            using (SqlConnection sqlCOn = new SqlConnection(connectionString))
            {
                sqlCOn.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT r.Firstname, r.Lastname,  r.Age, r.Gender, r.Email, ro.RoleName " +
                    "FROM Profiles r INNER JOIN Roles ro ON r.RoleID = ro.RoleID WHERE ro.RoleName <> 'ADMIN' AND ro.RoleName <> 'INSTRUCTOR';", sqlCOn);
                DataTable dt = new DataTable();
                sqlDa.Fill(dt);

                dgvTotal_Stud.DataSource = dt;
            }

            classes.DisplayName(_username, label2);
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Would you like to Log out?", "Confirmation", 
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                frmLogIn logIn = new frmLogIn();
                this.Hide();
                logIn.ShowDialog();
            }
            else{ }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud update = new frmUpdate_Stud();
            this.Hide();
            update.ShowDialog();
        }

    }
}
