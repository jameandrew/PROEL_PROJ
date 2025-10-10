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
    public partial class frmAddStud : Form
    {
        public frmAddStud()
        {
            InitializeComponent();
        }

        string connectionString = Classes.ConString();

        private void frmAddStud_Load(object sender, EventArgs e)
        {
            Classes.transparent(btnBack);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud stud = new frmUpdate_Stud();
            this.Hide();
            stud.ShowDialog();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_AddStud", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FIRSTNAME", txtFname.Text);
                cmd.Parameters.AddWithValue("@LASTNAME", txtLname.Text);
                cmd.Parameters.AddWithValue("@AGE", int.Parse(txtAge.Text));
                cmd.Parameters.AddWithValue("@GENDER", cmbGender.Text);
                cmd.Parameters.AddWithValue("@EMAIL", txtEmail.Text);
                cmd.Parameters.AddWithValue("@PHONE", txtPhone.Text);
                cmd.Parameters.AddWithValue("@ADDRESS", txtAddress.Text);
                cmd.Parameters.AddWithValue("@STATUS", "ACTIVE");
                cmd.Parameters.AddWithValue("@ENROLLMENTDATE", dtpEnrolldate.Value);

                connection.Open();
                Logs.Record("Add Student",
                    $"Student {txtFname.Text} {txtLname.Text} was added by {Logs.CurrentUserName}.",
                    txtFname.Text, txtLname.Text);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("✅ Student was Added!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("⚠ Adding Student failed.");
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
        }
    }
}
