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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace PROEL_PROJ
{
    public partial class frmEnrollment : Form
    {
        private int studentID;
        private string Status;
        private string connectionString = Classes.ConString();
        public frmEnrollment(int id, string fname, string lname, int age, string gender, string phone, string email, string status)
        {
            InitializeComponent();
            studentID = id;

            lblStudentID.Text = id.ToString();
            txtFname.Text = fname;
            txtLname.Text = lname;
            txtAge.Text = age.ToString();
            cmbGender.Text = gender;
            txtPhone.Text = phone;
            txtEmail.Text = email;
            Status = status;

            LoadCourses();
            LoadDepartments();
            LoadSemesters();

            Classes.MakeStudentInfoReadOnly(txtFname, txtLname, txtAge, cmbGender, txtPhone, txtAddress, txtEmail, cmbCourse, cmbDepartment);
        }

        private void frmEnrollment_Load(object sender, EventArgs e)
        {
            Classes.ApplySidebarStyle(btnBack);
        }

        private void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT CourseID, CourseName FROM Courses ORDER BY CourseName", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbCourse.DataSource = dt;
                cmbCourse.DisplayMember = "CourseName";
                cmbCourse.ValueMember = "CourseID";
            }
        }

        private void LoadDepartments()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT DepartmentID, DepartmentName FROM Departments ORDER BY DepartmentName", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbDepartment.DataSource = dt;
                cmbDepartment.DisplayMember = "DepartmentName";
                cmbDepartment.ValueMember = "DepartmentID";
            }
        }

        private void LoadSemesters()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT AcademicYear, TermName FROM Semesters ORDER BY AcademicYear DESC", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbSemester.DisplayMember = "TermName";
                cmbSemester.ValueMember = "TermName";
                cmbSemester.DataSource = dt;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (cmbCourse.SelectedValue == null || cmbDepartment.SelectedValue == null || string.IsNullOrEmpty(cmbSemester.Text))
            {
                MessageBox.Show("Please select a course, department, and semester.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_EnrollStudent", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    cmd.Parameters.AddWithValue("@CourseID", (int)cmbCourse.SelectedValue);
                    cmd.Parameters.AddWithValue("@DepartmentID", (int)cmbDepartment.SelectedValue);
                    cmd.Parameters.AddWithValue("@AcademicYear", txtAcademicYear.Text.Trim());
                    cmd.Parameters.AddWithValue("@TermName", cmbSemester.Text.Trim());

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"✅ Student enrolled successfully in {cmbCourse.Text}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"⚠ SQL Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠ Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud updateForm = new frmUpdate_Stud();
            updateForm.Show();
            this.Close();
        }
    }
}
