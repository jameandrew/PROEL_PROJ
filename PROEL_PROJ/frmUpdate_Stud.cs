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

        private void dgvUpdate_Stud_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvStudent.IsCurrentCellDirty)
            {
                dgvStudent.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvUpdate_Stud_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvStudent.Columns[e.ColumnIndex].Name == "cmbStatus")
            {
                int profileId = Convert.ToInt32(dgvStudent.Rows[e.RowIndex].Cells["ProfileID"].Value);
                string newStatus = dgvStudent.Rows[e.RowIndex].Cells["cmbStatus"].Value.ToString();

                DialogResult result = MessageBox.Show(
                   $"Are you sure you want to change this student's status to '{newStatus}'?",
                    "Confirm Status Change",
                     MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    classes.UpdateStatus(profileId, newStatus);
                    classes.RefreshDB(dgvStudent);
                }
            }
        }

        private void frmUpdate_Stud_Load(object sender, EventArgs e)
        {
            lblUser.Text = Classes.FullName;
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);
            Classes.ApplySidebarStyle(btnAdd);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.ApplySidebarStyle(btnCourse);
            Classes.ApplySidebarStyle(btnReports);
            Classes.ApplySidebarStyle(btnDelete);
            Classes.ApplySidebarStyle(button1);
            Classes.transparent(btnSearch);

            classes.LoadDataStudent(connectionString,dgvStudent);
            Classes.ShowCountStud(lblActive, lblPending, lblInactive);
        }

        //private void LoadStudents()
        //{
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        con.Open();
        //        string query = @"SELECT 
        //                         s.StudentID,
        //                         p.ProfileID, 
        //                         p.FirstName, 
        //                         p.LastName, 
        //                         p.Age, 
        //                         p.Gender, 
        //                         p.Phone, 
        //                         p.Address, 
        //                         p.Email, 
        //                         p.Status, 
        //                         s.EnrollmentDate
        //                         FROM Profiles p
        //                         INNER JOIN Students s ON p.ProfileID = s.ProfileID
        //                         WHERE p.RoleID <> 1 AND p.RoleID <> 3
        //                         AND p.Status = 'ACTIVE'
        //                         ORDER BY ProfileID DESC";

        //        SqlDataAdapter da = new SqlDataAdapter(query, con);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        dgvStudent.DataSource = dt;

        //        if (!dgvStudent.Columns.Contains("btnAction"))
        //        {
        //            DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
        //            btnCol.HeaderText = "";
        //            btnCol.Name = "btnAction";
        //            btnCol.Text = "View";
        //            btnCol.UseColumnTextForButtonValue = true;
        //            dgvStudent.Columns.Add(btnCol);
        //        }

        //        dgvStudent.Columns["btnAction"].DisplayIndex = dgvStudent.Columns.Count - 1;
        //    }
        //}

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            Classes.SearchFields(dgvStudent, keyword);
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddStud addStud = new frmAddStud();
            this.Hide();
            addStud.ShowDialog();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud stud = new frmUpdate_Stud();
            this.Hide();
            stud.ShowDialog();
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
            frmTeachers frmTeachers= new frmTeachers();
            this.Hide();
            frmTeachers.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)    
        {
            DialogResult result = MessageBox.Show("Would you like to delete this Student?", "Confirmation",
           MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                int profileId = Convert.ToInt32(dgvStudent.CurrentRow.Cells["ProfileID"].Value);
                classes.UpdateStatus(profileId, "INACTIVE");
                Logs.Record("Delete Student", $"Student set to INACTIVE by {Logs.CurrentUserName}");

                MessageBox.Show("Student set to INACTIVE.");
                Classes.ShowCountStud(lblActive, lblPending, lblInactive);
                classes.LoadDataStudent(connectionString, dgvStudent);
            }
            else { }
                
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            frmLogs logs = new frmLogs();
            this.Hide();
            logs.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvStudent.CurrentRow == null)
            {
                MessageBox.Show("Please select a student first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int studentID = Convert.ToInt32(dgvStudent.CurrentRow.Cells["StudentID"].Value);
            string firstName = dgvStudent.CurrentRow.Cells["FirstName"].Value.ToString();
            string lastName = dgvStudent.CurrentRow.Cells["LastName"].Value.ToString();
            int age = Convert.ToInt32(dgvStudent.CurrentRow.Cells["Age"].Value);
            string gender = dgvStudent.CurrentRow.Cells["Gender"].Value.ToString();
            string phone = dgvStudent.CurrentRow.Cells["Phone"].Value.ToString();
            string email = dgvStudent.CurrentRow.Cells["Email"].Value.ToString();
            string status = dgvStudent.CurrentRow.Cells["Status"].Value.ToString();

            Logs.Record("Enroll Student", $"Student was Enrolled by {Logs.CurrentUserName}");
            frmEnrollment enrollForm = new frmEnrollment(studentID, firstName, lastName, age, gender, phone, email, status);
            enrollForm.ShowDialog();
            this.Close();
        }

        private void dgvStudent_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStudent.Rows[e.RowIndex];

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
            classes.LoadDataStudent(connectionString, dgvStudent);
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvStudent.Columns[e.ColumnIndex].Name == "btnAction")
            {
                int studentID = Convert.ToInt32(dgvStudent.Rows[e.RowIndex].Cells["StudentID"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                    SELECT 
                    s.StudentID,
                    p.ProfileID,
                    d.DepartmentID,
                    p.FirstName,
                    p.LastName,
                    p.Age,
                    p.Gender,
                    p.Phone,
                    p.Address,
                    p.Email,
                    p.Status,
                    s.EnrollmentDate,
                    ISNULL(d.DepartmentName, 'No department assigned') AS DepartmentName,
                    ISNULL(CourseList.Courses, 'No course enrolled') AS EnrolledCourses,
                    ISNULL(SemList.Semesters, 'No semester record') AS SemesterDetails
                    FROM Profiles p
                    INNER JOIN Students s ON p.ProfileID = s.ProfileID
                    LEFT JOIN Enrollment e ON s.StudentID = e.StudentID
                    LEFT JOIN Courses c ON e.CourseID = c.CourseID
                    LEFT JOIN Departments d ON c.DepartmentID = d.DepartmentID

                    LEFT JOIN (
                    SELECT e2.StudentID, STRING_AGG(c2.CourseName, ', ') AS Courses
                    FROM Enrollment e2
                    INNER JOIN Courses c2 ON e2.CourseID = c2.CourseID
                    GROUP BY e2.StudentID
                    ) AS CourseList ON s.StudentID = CourseList.StudentID

                    LEFT JOIN (
                    SELECT e3.StudentID, STRING_AGG(CONCAT(se.AcademicYear, ' - ', se.TermName), ', ') AS Semesters
                    FROM Enrollment e3
                    INNER JOIN Semesters se ON e3.SemesterID = se.SemesterID
                    GROUP BY e3.StudentID
                    ) AS SemList ON s.StudentID = SemList.StudentID

                    WHERE s.StudentID = @StudentID";



                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@StudentID", studentID);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string details =
                        $@"StudentID: {reader["StudentID"]}
                        ProfileID: {reader["ProfileID"]}
                        DepartmentID: {reader["DepartmentID"]}
                        Name: {reader["FirstName"]} {reader["LastName"]}
                        Age: {reader["Age"]}
                        Gender: {reader["Gender"]}
                        Phone: {reader["Phone"]}
                        Address: {reader["Address"]}
                        Email: {reader["Email"]}
                        Status: {reader["Status"]}
                        Enrollment Date: {Convert.ToDateTime(reader["EnrollmentDate"]).ToString("yyyy-MM-dd")}
                        Department: {reader["DepartmentName"]}
                        Courses Enrolled: {reader["EnrolledCourses"]}
                        Semesters: {reader["SemesterDetails"]}";

                        MessageBox.Show(details, "Student Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No student details found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            frmDashboard frmDashboard = new frmDashboard();
            this.Hide();
            frmDashboard.ShowDialog();
        }

        private void btnTeacher_Click_1(object sender, EventArgs e)
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

        private void btnLogs_Click_1(object sender, EventArgs e)
        {
            frmLogs frmLogs = new frmLogs();
            this.Hide();
            frmLogs.ShowDialog();
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

        private void btnReports_Click(object sender, EventArgs e)
        {
            frmReports frmReports = new frmReports();
            this.Hide();
            frmReports.ShowDialog();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
