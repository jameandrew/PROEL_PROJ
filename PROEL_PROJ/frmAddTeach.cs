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
    public partial class frmAddTeach : Form
    {
        public frmAddTeach()
        {
            InitializeComponent();
        }

        string connectionString = Classes.ConString();

        private void frmAddTeach_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT DepartmentID, DepartmentName FROM Departments", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbDept.DataSource = dt;
                cmbDept.DisplayMember = "DepartmentName";
                cmbDept.ValueMember = "DepartmentID";
                cmbDept.SelectedIndex = -1;
            }

            Classes.transparent(btnBack);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmTeachers teachers = new frmTeachers();
            this.Hide();
            teachers.ShowDialog();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_Teacher", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FIRSTNAME", txtFname.Text);
                cmd.Parameters.AddWithValue("@LASTNAME", txtLname.Text);
                cmd.Parameters.AddWithValue("@AGE", int.Parse(txtAge.Text));
                cmd.Parameters.AddWithValue("@GENDER", cmbGender.Text);
                cmd.Parameters.AddWithValue("@EMAIL", txtEmail.Text);
                cmd.Parameters.AddWithValue("@PHONE", txtPhone.Text);
                cmd.Parameters.AddWithValue("@ADDRESS", txtAddress.Text);
                cmd.Parameters.AddWithValue("@STATUS", "ACTIVE");
                cmd.Parameters.AddWithValue("@HIREDATE", dtpEnrolldate.Value);
                cmd.Parameters.AddWithValue("@DEPARTMENTID", Convert.ToInt32(cmbDept.SelectedValue));

                connection.Open();
                Logs.Record("Add Teacher",
                    $"Teacher {txtFname.Text} {txtLname.Text} was added by {Logs.CurrentUserName}.",
                    txtFname.Text, txtLname.Text);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("✅ Teacher was Added!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("⚠ Adding Teacher failed.");
                }
            }
        }

        private void ClearFields()
        {
            txtFname.Clear();
            txtLname.Clear();
            txtAge.Clear();
            cmbGender.SelectedIndex = -1;
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            dtpEnrolldate.Value = DateTime.Now;
            cmbDept.SelectedIndex = -1;
        }
    }
}
