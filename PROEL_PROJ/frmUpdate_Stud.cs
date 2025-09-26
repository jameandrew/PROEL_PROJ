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
    public partial class frmUpdate_Stud : Form
    {
        private Classes classes;
        public frmUpdate_Stud()
        {
            InitializeComponent();
            classes = new Classes();
        }

        string connectionString = Classes.ConString();

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.ShowDialog();
        }

        private void dgvUpdate_Stud_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvUpdate_Stud.IsCurrentCellDirty)
            {
                dgvUpdate_Stud.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvUpdate_Stud_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvUpdate_Stud.Columns[e.ColumnIndex].Name == "cmbStatus")
            {
                int profileId = Convert.ToInt32(dgvUpdate_Stud.Rows[e.RowIndex].Cells["ProfileID"].Value);
                string newStatus = dgvUpdate_Stud.Rows[e.RowIndex].Cells["cmbStatus"].Value.ToString();

                DialogResult result = MessageBox.Show(
                   $"Are you sure you want to change this student's status to '{newStatus}'?",
                    "Confirm Status Change",
                     MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    classes.UpdateStatus(profileId, newStatus);
                    classes.RefreshDB(dgvUpdate_Stud);
                }
            }
        }

        private void frmUpdate_Stud_Load(object sender, EventArgs e)
        {
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);
            Classes.ApplySidebarStyle(btnAdd);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.transparent(btnSearch);

            classes.LoadDataStudent(connectionString,dgvUpdate_Stud);
            Classes.ShowCountStud(lblActive, lblPending, lblInactive);
            LoadStudents();
        }

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.ShowDialog();
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
            frmTeachers teachers = new frmTeachers();
            this.Hide();
            teachers.ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddStud addStud = new frmAddStud();
            this.Hide();
            addStud.ShowDialog();
        }

        private void dgvUpdate_Stud_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUpdate_Stud.Rows[e.RowIndex];

                int profileId = Convert.ToInt32(row.Cells["ProfileID"].Value);
                string firstName = row.Cells["FirstName"].Value.ToString();
                string lastName = row.Cells["LastName"].Value.ToString();
                int age = Convert.ToInt32(row.Cells["Age"].Value);
                string gender = row.Cells["Gender"].Value.ToString();
                string phone = row.Cells["Phone"].Value.ToString();
                string address = row.Cells["Address"].Value.ToString();
                string email = row.Cells["Email"].Value.ToString();
                string status = row.Cells["Status"].Value.ToString();

                frmUpdate updateForm = new frmUpdate(
                    profileId, firstName, lastName, age, gender, phone, address, email, status);

                updateForm.ShowDialog();
            }
            LoadStudents();
        }

        private void LoadStudents()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT P.ProfileID, P.FirstName, P.LastName, P.Age, 
                                P.Gender, P.Phone, P.Address, P.Email, P.Status
                         FROM Profiles P
                         INNER JOIN Students S ON P.ProfileID = S.ProfileID";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvUpdate_Stud.DataSource = dt;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            Classes.SearchFields(dgvUpdate_Stud, keyword);
        }
    }
}
