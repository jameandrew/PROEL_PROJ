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
    public partial class frmTeachers : Form
    {
        Classes classes = new Classes();
        public frmTeachers()
        {
            InitializeComponent();
        }

        string connectionString = Classes.ConString();
        private void frmTeachers_Load(object sender, EventArgs e)
        {
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);
            Classes.ApplySidebarStyle(btnAdd);

            classes.LoadDataTeacher(connectionString, dgvUpdate_Stud);
            Classes.ShowCountTeach(lblActive, lblPending, lblInactive);

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.ShowDialog();
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {

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
            else { }
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud stud = new frmUpdate_Stud();
            this.Hide();
            stud.ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddTeach teach = new frmAddTeach();
            this.Hide();
            teach.ShowDialog();
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
            LoadTeachers();
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
                    classes.RefreshTeachDB(dgvUpdate_Stud);
                }
            }
        }

        private void LoadTeachers()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT P.ProfileID, P.FirstName, P.LastName, P.Age, 
                                P.Gender, P.Phone, P.Address, P.Email, P.Status
                         FROM Profiles P
                         INNER JOIN Instructors S ON P.ProfileID = S.ProfileID";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvUpdate_Stud.DataSource = dt;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            Classes.SearchFieldsTeach(dgvUpdate_Stud, keyword);
        }
    }
}
