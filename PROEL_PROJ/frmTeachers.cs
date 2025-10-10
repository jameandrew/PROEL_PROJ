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
        Teacher teach = new Teacher();
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
            Classes.ApplySidebarStyle(btnCourse);
            Classes.ApplySidebarStyle(btnAdd);
            Classes.ApplySidebarStyle(btnDelete);
            Classes.ApplySidebarStyle(button1);

            teach.LoadDataTeacher(connectionString, dgvTeacher);
            Teacher.ShowCountTeach(lblActive, lblPending, lblInactive);

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.ShowDialog();
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

        private void LoadTeachers()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
                SELECT 
                i.InstructorID,
                p.ProfileID,
                p.FirstName,
                p.LastName,
                d.DepartmentName
                FROM Instructors i
                INNER JOIN Profiles p ON i.ProfileID = p.ProfileID
                INNER JOIN Departments d ON i.DepartmentID = d.DepartmentID";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvTeacher.DataSource = dt;

                dgvTeacher.Columns["ProfileID"].Visible = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            Teacher.SearchFieldsTeach(dgvTeacher, keyword);
        }

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.ShowDialog();
        }

        private void btnStudents_Click_1(object sender, EventArgs e)
        {
            frmUpdate_Stud stud = new frmUpdate_Stud();
            this.Hide();
            stud.ShowDialog();
        }

        private void dgvTeacher_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTeacher.Rows[e.RowIndex];

                int profileId = Convert.ToInt32(row.Cells["ProfileID"].Value);
                string firstName = row.Cells["FirstName"].Value.ToString();
                string lastName = row.Cells["LastName"].Value.ToString();
                int age = Convert.ToInt32(row.Cells["Age"].Value);
                string gender = row.Cells["Gender"].Value.ToString();
                string phone = row.Cells["Phone"].Value.ToString();
                string address = row.Cells["Address"].Value.ToString();
                string email = row.Cells["Email"].Value.ToString();
                string status = row.Cells["Status"].Value.ToString();

                frmUpdate_teach updateForm = new frmUpdate_teach(
                    profileId, firstName, lastName, age, gender, phone, address, email, status);

                updateForm.ShowDialog();
            }
            LoadTeachers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int profileId = Convert.ToInt32(dgvTeacher.CurrentRow.Cells["ProfileID"].Value);
            teach.UpdateStatus(profileId, "INACTIVE");

            Logs.Record("Delete Teacher", $"Teacher set to INACTIVE by {Logs.CurrentUserName}");

            MessageBox.Show("Teacher set to INACTIVE.");
            Teacher.ShowCountTeach(lblActive, lblPending, lblInactive);
        }

        private void btnCourse_Click(object sender, EventArgs e)
        {
            frmCourse course = new frmCourse();
            this.Hide();
            course.ShowDialog();
        }

        private void dgvTeacher_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            frmLogs frmLogs = new frmLogs();
            this.Hide();
            frmLogs.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvTeacher.CurrentRow == null)
            {
                MessageBox.Show("Please select a teacher first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int instructorID = Convert.ToInt32(dgvTeacher.CurrentRow.Cells["InstructorID"].Value);
            string firstName = dgvTeacher.CurrentRow.Cells["FirstName"].Value.ToString();
            string lastName = dgvTeacher.CurrentRow.Cells["LastName"].Value.ToString();

            string departmentName = GetTeacherDepartmentName(instructorID);

            Logs.Record("Assigned Instructor", $"Instructor was assigned by {Logs.CurrentUserName}");
            frmAssign assignForm = new frmAssign(instructorID, firstName, lastName, departmentName);
            assignForm.ShowDialog();
        }

        private string GetTeacherDepartmentName(int instructorID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"SELECT d.DepartmentName
                         FROM Departments d
                         INNER JOIN Instructors i ON d.DepartmentID = i.DepartmentID
                         WHERE i.InstructorID = @InstructorID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@InstructorID", instructorID);

                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "Unknown";
            }
        }

        private void dgvTeacher_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvTeacher.Columns[e.ColumnIndex].Name == "btnAction")
            {
                int instructorID = Convert.ToInt32(dgvTeacher.Rows[e.RowIndex].Cells["InstructorID"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                    SELECT 
                    s.InstructorID,
                    p.ProfileID,
                    p.FirstName,
                    p.LastName,
                    p.Age,
                    p.Gender,
                    p.Phone,
                    p.Address,
                    p.Email,
                    p.Status,
                    s.HireDate,
                    d.DepartmentName,
                    ISNULL(CourseList.Courses, 'No course assigned') AS AssignedCourses
                    FROM Profiles p
                    INNER JOIN Instructors s ON p.ProfileID = s.ProfileID
                    INNER JOIN Departments d ON s.DepartmentID = d.DepartmentID
                    LEFT JOIN 
                    (
                    SELECT ic.InstructorID, STRING_AGG(c.CourseName, ', ') AS Courses
                    FROM InstructorCourses ic
                    INNER JOIN Courses c ON ic.CourseID = c.CourseID
                    GROUP BY ic.InstructorID
                    ) AS CourseList
                    ON s.InstructorID = CourseList.InstructorID
                    WHERE s.InstructorID = @InstructorID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@InstructorID", instructorID);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string details =
                        $@"InstructorID: {reader["InstructorID"]}
                        ProfileID: {reader["ProfileID"]}
                        Name: {reader["FirstName"]} {reader["LastName"]}
                        Age: {reader["Age"]}
                        Gender: {reader["Gender"]}
                        Phone: {reader["Phone"]}
                        Address: {reader["Address"]}
                        Email: {reader["Email"]}
                        Status: {reader["Status"]}
                        Hire Date: {Convert.ToDateTime(reader["HireDate"]).ToString("yyyy-MM-dd")}
                        Department: {reader["DepartmentName"]}
                        Assigned Courses: {reader["AssignedCourses"]}";

                        MessageBox.Show(details, "Teacher Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
