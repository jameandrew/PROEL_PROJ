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
    public partial class frmStudDashboard : Form
    {
        private string connectionString = Classes.ConString();
        private int currentStudentID;

        public frmStudDashboard()
        {
            InitializeComponent();
        }

        public frmStudDashboard(int studentID)
        {
            InitializeComponent();
            this.currentStudentID = studentID;
        }

        private void frmStudDashboard_Load(object sender, EventArgs e)
        {
            lblUser.Text = Classes.FullName;
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnLogOut);

            // Load home view by default
            LoadHomeView();
        }

        // Hook this to your Home/Dashboard button click event
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            LoadHomeView();
        }

        // Hook this to your Students Info button click event
        private void btnStudents_Click(object sender, EventArgs e)
        {
            LoadStudentsInfoView();
        }

        // Hook this to your Log Out button click event
        private void btnLogOut_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?",
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
                // Show your login form here
                // frmLogin login = new frmLogin();
                // login.Show();
            }
        }

        // Load Home View - Shows Enrollment Information
        private void LoadHomeView()
        {
            try
            {
                dgvStudent.Columns.Clear();
                dgvStudent.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvStudent.AllowUserToAddRows = false;
                dgvStudent.AllowUserToDeleteRows = false;
                dgvStudent.ReadOnly = true;
                dgvStudent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            c.CourseName AS 'Course Name',
                            c.CourseCode AS 'Course Code',
                            c.Credits AS 'Credits',
                            d.DepartmentName AS 'Department',
                            s.AcademicYear AS 'Academic Year',
                            s.TermName AS 'Term',
                            ISNULL(e.Grade, 'N/A') AS 'Grade',
                            CONVERT(VARCHAR(10), e.DateRecorded, 101) AS 'Date Enrolled'
                        FROM Students st
                        INNER JOIN Profiles p ON st.ProfileID = p.ProfileID
                        INNER JOIN Users u ON p.ProfileID = u.ProfileID
                        INNER JOIN Enrollment e ON st.StudentID = e.StudentID
                        INNER JOIN Courses c ON e.CourseID = c.CourseID
                        INNER JOIN Departments d ON c.DepartmentID = d.DepartmentID
                        INNER JOIN Semesters s ON e.SemesterID = s.SemesterID
                        WHERE u.Username = @Username
                        ORDER BY e.DateRecorded DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", Classes.Username);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvStudent.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No enrollment records found for this student.", "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading enrollment data: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load Students Info View - Shows Student Personal Information
        private void LoadStudentsInfoView()
        {
            try
            {
                dgvStudent.Columns.Clear();
                dgvStudent.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvStudent.AllowUserToAddRows = false;
                dgvStudent.AllowUserToDeleteRows = false;
                dgvStudent.ReadOnly = true;
                dgvStudent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            p.FirstName AS 'First Name',
                            p.LastName AS 'Last Name',
                            p.Age AS 'Age',
                            p.Gender AS 'Gender',
                            p.Phone AS 'Phone',
                            p.Address AS 'Address',
                            p.Email AS 'Email',
                            p.Status AS 'Status',
                            CONVERT(VARCHAR(10), s.EnrollmentDate, 101) AS 'Enrollment Date'
                        FROM Students s
                        INNER JOIN Profiles p ON s.ProfileID = p.ProfileID
                        WHERE p.Username = @Username";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", Classes.Username);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvStudent.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Student information not found.", "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student information: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStudents_Click_1(object sender, EventArgs e)
        {
            frmStudentsInformatio students = new frmStudentsInformatio();
            this.Hide();
            students.ShowDialog();
        }

        private void btnLogOut_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?",
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
                frmLogIn login = new frmLogIn();
                login.Show();
            }
        }
    }
}