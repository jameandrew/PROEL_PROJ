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
            Classes.ApplySidebarStyle(btnUpdate);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);

            //show the database
            using (SqlConnection sqlCOn = new SqlConnection(connectionString))
            {
                sqlCOn.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT \r\n    r.Firstname + ' ' + r.Lastname AS Fullname,\r\n    r.Age,\r\n    r.Gender,\r\n    r.Email,\r\n    r.Username,\r\n    r.Password,\r\n    ro.RoleName\r\nFROM Profiles r\r\nINNER JOIN Roles ro ON r.RoleID = ro.RoleID WHERE ro.RoleName <> 'ADMIN' AND ro.RoleName <> 'INSTRUCTOR';", sqlCOn);
                DataTable dt = new DataTable();
                sqlDa.Fill(dt);

                dgvTotal_Stud.DataSource = dt;
            }
        }
    }
}
