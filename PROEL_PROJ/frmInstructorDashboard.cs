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
    public partial class frmTeachDashboard : Form
    {
        private string connectionString = Classes.ConString();
        private int currentInstructorID;

        public frmTeachDashboard()
        {
            InitializeComponent();
        }

        private void frmInstructorDashboard_Load(object sender, EventArgs e)
        {
            lblUser.Text = Classes.FullName;
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnTeachersInfo);
            Classes.ApplySidebarStyle(btnLogOut);

            // Get the current instructor ID
            GetInstructorID();

            // Load assigned subjects by default
            LoadAssignedSubjects();
        }

        private void GetInstructorID()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT i.InstructorID
                        FROM Instructors i
                        INNER JOIN Profiles p ON i.ProfileID = p.ProfileID
                        INNER JOIN Users u ON p.ProfileID = u.ProfileID
                        WHERE u.Username = @Username";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", Classes.Username);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        currentInstructorID = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting instructor information: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hook this to your Dashboard/Home button click event
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            LoadAssignedSubjects();
        }

        // Hook this to your Students button click event
        private void btnStudents_Click(object sender, EventArgs e)
        {
            LoadStudentsHandled();
        }

        // Hook this to your Subjects button click event
        private void btnSubjects_Click(object sender, EventArgs e)
        {
            LoadAssignedSubjects();
        }

        // Hook this to your Log Out button click event
        private void btnLogOut_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?",
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
                // Show your login form here if needed
                // frmLogin login = new frmLogin();
                // login.Show();
            }
        }

        // Load Assigned Subjects in dgvTeacher - Shows all courses assigned to this teacher
        private void LoadAssignedSubjects()
        {
            try
            {
                dgvTeacher.Columns.Clear();
                dgvTeacher.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvTeacher.AllowUserToAddRows = false;
                dgvTeacher.AllowUserToDeleteRows = false;
                dgvTeacher.ReadOnly = true;
                dgvTeacher.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            c.CourseID,
                            c.CourseName AS 'Course Name',
                            c.CourseCode AS 'Course Code',
                            c.Description AS 'Description',
                            c.Credits AS 'Credits',
                            d.DepartmentName AS 'Department',
                            c.Status AS 'Status',
                            CONVERT(VARCHAR(10), ic.DateAssigned, 101) AS 'Date Assigned',
                            (SELECT COUNT(*) 
                             FROM Enrollment e 
                             WHERE e.CourseID = c.CourseID) AS 'Enrolled Students'
                        FROM InstructorCourses ic
                        INNER JOIN Courses c ON ic.CourseID = c.CourseID
                        INNER JOIN Departments d ON c.DepartmentID = d.DepartmentID
                        WHERE ic.InstructorID = @InstructorID
                        ORDER BY ic.DateAssigned DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@InstructorID", currentInstructorID);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvTeacher.DataSource = dt;

                    // Hide CourseID column
                    if (dgvTeacher.Columns["CourseID"] != null)
                    {
                        dgvTeacher.Columns["CourseID"].Visible = false;
                    }

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No assigned subjects found.", "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading assigned subjects: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load Students Handled in dgvTeacher - Shows all students enrolled in teacher's courses
        private void LoadStudentsHandled()
        {
            try
            {
                dgvTeacher.Columns.Clear();
                dgvTeacher.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvTeacher.AllowUserToAddRows = false;
                dgvTeacher.AllowUserToDeleteRows = false;
                dgvTeacher.ReadOnly = true;
                dgvTeacher.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT DISTINCT
                            s.StudentID,
                            CONCAT(p.FirstName, ' ', p.LastName) AS 'Student Name',
                            p.Email AS 'Email',
                            p.Phone AS 'Phone',
                            c.CourseName AS 'Course Name',
                            c.CourseCode AS 'Course Code',
                            sem.AcademicYear AS 'Academic Year',
                            sem.TermName AS 'Term',
                            ISNULL(e.Grade, 'N/A') AS 'Grade',
                            CONVERT(VARCHAR(10), e.DateRecorded, 101) AS 'Date Enrolled'
                        FROM InstructorCourses ic
                        INNER JOIN Courses c ON ic.CourseID = c.CourseID
                        INNER JOIN Enrollment e ON c.CourseID = e.CourseID
                        INNER JOIN Students s ON e.StudentID = s.StudentID
                        INNER JOIN Profiles p ON s.ProfileID = p.ProfileID
                        INNER JOIN Semesters sem ON e.SemesterID = sem.SemesterID
                        WHERE ic.InstructorID = @InstructorID
                        ORDER BY p.LastName, p.FirstName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@InstructorID", currentInstructorID);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvTeacher.DataSource = dt;

                    // Hide StudentID column
                    if (dgvTeacher.Columns["StudentID"] != null)
                    {
                        dgvTeacher.Columns["StudentID"].Visible = false;
                    }

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No students found in your courses.", "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading students: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTeachersInfo_Click(object sender, EventArgs e)
        {
            frmInstructorInformation frmInstructorInformation = new frmInstructorInformation();
            this.Hide();
            frmInstructorInformation.ShowDialog();
        }
    }
}