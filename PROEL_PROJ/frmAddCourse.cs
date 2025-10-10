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
    public partial class frmAddCourse : Form
    {
        public frmAddCourse()
        {
            InitializeComponent();
        }

        string connectionString = Classes.ConString();

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Sp_AddSub", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@COURSENAME", txtCourseName.Text);
                    cmd.Parameters.AddWithValue("@COURSECODE", txtCourseCode.Text);
                    cmd.Parameters.AddWithValue("@CREDITS", int.Parse(txtCredit.Text));
                    cmd.Parameters.AddWithValue("@DESCRIPTION", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@DEPARTMENTID", Convert.ToInt32(cmbDepartment.SelectedValue));

                    connection.Open();
                    Logs.Record("Add Course",
                    $"Course {txtCourseName.Text} was added by {Logs.CurrentUserName}.",
                    txtCourseName.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("✅ Course was successfully added!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠ Adding Course failed" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtCourseName.Clear();
            txtCourseCode.Clear();
            txtCredit.Clear();
            txtDescription.Clear();
            cmbDepartment.SelectedIndex = -1;
        }

        private void frmAddCourse_Load(object sender, EventArgs e)
        {
            Course.LoadInstructors(cmbDepartment);
            Classes.transparent(btnBack);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmCourse frmCourse = new frmCourse();  
            this.Hide();
            frmCourse.ShowDialog();
        }
    }
}
