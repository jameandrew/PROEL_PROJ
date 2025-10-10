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
    public partial class frmAssign : Form
    {
        private int teacherID;
        private string connectionString = Classes.ConString();

        public frmAssign(int id, string fname, string lname, string department)
        {
            InitializeComponent();
            teacherID = id;
            txtTeachID.Text = id.ToString();
            txtFname.Text = fname;
            txtLname.Text = lname;
            txtDepartment.Text = department;

            LoadCoursesByDepartment(department);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (cmbCourse.SelectedValue == null)
            {
                MessageBox.Show("Please select a course.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_AssignTeacherCourse", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                cmd.Parameters.AddWithValue("@CourseID", (int)cmbCourse.SelectedValue);

                Logs.Record(
                    "Assign Course",
                    $"Teacher {txtFname.Text} {txtLname.Text} was assigned to course {cmbCourse.Text} by {Logs.CurrentUserName}.",
                    txtFname.Text, txtLname.Text
                );

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    MessageBox.Show("✅ Course successfully assigned!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("⚠ Teacher might already be assigned to this course.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void LoadCoursesByDepartment(string departmentName)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT c.CourseID, c.CourseName 
                                 FROM Courses c
                                 INNER JOIN Departments d ON c.DepartmentID = d.DepartmentID
                                 WHERE d.DepartmentName = @DepartmentName
                                 AND c.Status = 'ACTIVE'";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DepartmentName", departmentName);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbCourse.DataSource = dt;
                cmbCourse.DisplayMember = "CourseName";
                cmbCourse.ValueMember = "CourseID";
            }
        }

        private void frmAssign_Load(object sender, EventArgs e)
        {
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmTeachers teachers = new frmTeachers();
            this.Hide();
            teachers.ShowDialog();
        }
    }
}
