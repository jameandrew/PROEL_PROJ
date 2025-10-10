using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.ShowDialog();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud stud = new frmUpdate_Stud();
            this.Hide();
            stud.ShowDialog();
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
            frmTeachers teachers = new frmTeachers();
            this.Hide();
            teachers.ShowDialog();
        }

        private void frmCourse_Load(object sender, EventArgs e)
        {
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);
            Classes.ApplySidebarStyle(btnCourse);
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
            int courseid = Convert.ToInt32(dgvCourse.CurrentRow.Cells["CourseID"].Value);
            course.UpdateStatus(courseid, "INACTIVE");
            Logs.Record("Delete Course", $"Course set to INACTIVE by {Logs.CurrentUserName}");
            MessageBox.Show("Course set to INACTIVE.");
            Course.ShowCount(lblActive, lblPending, lblInactive);
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

        private void btnLogs_Click(object sender, EventArgs e)
        {
            frmLogs logs = new frmLogs();  
            this.Hide();
            logs.ShowDialog();
        }
    }
}
