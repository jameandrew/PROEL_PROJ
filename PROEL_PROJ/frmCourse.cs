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
    public partial class frmCourse : Form
    {
        Course course = new Course();
        public frmCourse()
        {
            InitializeComponent();
        }

        string connectionString = Course.ConString();
      
        private void frmCourse_Load(object sender, EventArgs e)
        {
            lblUser.Text = Classes.FullName;
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);
            Classes.ApplySidebarStyle(btnCourse);
            Classes.ApplySidebarStyle(btnReports);
            Classes.ApplySidebarStyle(btnAdd);
            Classes.ApplySidebarStyle(btnDelete);

            course.LoadCourses(connectionString, dgvCourse);
            Course.ShowCount(lblActive, lblPending, lblInactive);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddCourse addCourse = new frmAddCourse();
            this.Hide();
            addCourse.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Would you like to delete this Course?", "Confirmation",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                int courseid = Convert.ToInt32(dgvCourse.CurrentRow.Cells["CourseID"].Value);
                course.UpdateStatus(courseid, "INACTIVE");
                Logs.Record("Delete Course", $"Course set to INACTIVE by {Logs.CurrentUserName}");
                MessageBox.Show("Course set to INACTIVE.");
                Course.ShowCount(lblActive, lblPending, lblInactive);
                course.LoadCourses(connectionString, dgvCourse);
            }
            else { }
                
        }

        private void dgvCourse_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCourse.Rows[e.RowIndex];

                int courseid = Convert.ToInt32(row.Cells["CourseID"].Value);
                string coursename = row.Cells["CourseName"].Value.ToString();
                string coursecode = row.Cells["CourseCode"].Value.ToString();
                int credit = Convert.ToInt32(row.Cells["Credits"].Value);
                string department = row.Cells["DepartmentName"].Value.ToString();
                string description = row.Cells["Description"].Value.ToString();
                string status = row.Cells["Status"].Value.ToString();

                frmUpdate_Course updateForm = new frmUpdate_Course(
                    courseid, coursename, coursecode, credit, department, description, status);

                updateForm.ShowDialog();
            }
            Course.ReLoadCourse(dgvCourse);
        }

        private void dgvCourse_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCourse.Columns[e.ColumnIndex].Name == "btnAction" && e.RowIndex >= 0)
            {
                int courseId = Convert.ToInt32(dgvCourse.Rows[e.RowIndex].Cells["CourseID"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_ViewCourseDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CourseID", courseId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        string details = "";

                        if (reader.Read())
                        {
                            details =
                                $"📘 COURSE INFORMATION\n" +
                                $"Course ID: {reader["CourseID"]}\n" +
                                $"Name: {reader["CourseName"]}\n" +
                                $"Code: {reader["CourseCode"]}\n" +
                                $"Credits: {reader["Credits"]}\n" +
                                $"Department: {reader["DepartmentName"]}\n" +
                                $"Status: {reader["Status"]}\n\n";
                        }

                        if (reader.NextResult())
                        {
                            details += "👩‍🏫 ASSIGNED INSTRUCTORS:\n";
                            bool hasInstructors = false;

                            while (reader.Read())
                            {
                                hasInstructors = true;
                                details +=
                                    $"- {reader["InstructorName"]} (Hired: {Convert.ToDateTime(reader["HireDate"]).ToString("yyyy-MM-dd")}, " +
                                    $"Assigned: {Convert.ToDateTime(reader["DateAssigned"]).ToString("yyyy-MM-dd")})\n";
                            }

                            if (!hasInstructors)
                                details += "No instructors assigned yet.\n";
                        }

                        if (reader.NextResult())
                        {
                            details += "\n👨‍🎓 ENROLLED STUDENTS:\n";
                            bool hasStudents = false;

                            while (reader.Read())
                            {
                                hasStudents = true;
                                details +=
                                    $"- {reader["StudentName"]} (Enrolled: {Convert.ToDateTime(reader["EnrollmentDate"]).ToString("yyyy-MM-dd")})\n";
                            }

                            if (!hasStudents)
                                details += "No students enrolled yet.";
                        }

                        MessageBox.Show(details, "Course Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnStudents_Click_1(object sender, EventArgs e)
        {
            frmUpdate_Stud frmUpdate_Stud = new frmUpdate_Stud();
            this.Hide();
            frmUpdate_Stud.ShowDialog();
        }

        private void btnTeacher_Click_1(object sender, EventArgs e)
        {
            frmTeachers frmTeachers = new frmTeachers();
            this.Hide();
            frmTeachers.ShowDialog();
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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            Course.SearchCourse(dgvCourse, txtSearch.Text.Trim());
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            frmReports frmReports = new frmReports();
            this.Hide();
            frmReports.ShowDialog();
        }
    }
}
