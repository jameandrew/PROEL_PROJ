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
    public partial class frmSign_Up : Form
    {
        public frmSign_Up()
        {
            InitializeComponent();
        }

        string connectionString = Classes.ConString();

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Restriction();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand cmd = new SqlCommand("Reg_Users", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FIRSTNAME", txtFname.Text);
                cmd.Parameters.AddWithValue("@LASTNAME", txtLname.Text);
                cmd.Parameters.AddWithValue("@AGE", int.Parse(txtAge.Text));
                cmd.Parameters.AddWithValue("@GENDER", cmbGender.Text);
                cmd.Parameters.AddWithValue("@EMAIL", txtEmail.Text);
                cmd.Parameters.AddWithValue("PHONENUMBER", txtPhone.Text);
                cmd.Parameters.AddWithValue("@ADDRESS", txtAddress.Text);
                cmd.Parameters.AddWithValue("@STATUS", "PENDING");

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string userid = reader["UserID"].ToString();
                        string username = reader["Username"].ToString();
                        string password = reader["Password"].ToString();

                        MessageBox.Show($"✅ Registration Complete!\n" +
                               $"UserID: {userid}\n" +
                               $"Username: {username}\n" +
                               $"Password: {password}",
                               "Registration Success",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);

                        DialogResult result = MessageBox.Show("Do you want to go to the login form?",
                            "Confirmation",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.OK)
                        {
                            frmLogIn login = new frmLogIn();
                            login.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show("⚠ Registration failed. No data returned from stored procedure.");
                    }

                }

            }
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Would you like to Log-In?", "Conformation",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                frmLogIn logIn = new frmLogIn();
                this.Hide();
                logIn.ShowDialog();
            }
            else { }
        }

        public void Restriction()
        {
            if (string.IsNullOrEmpty(txtFname.Text) || string.IsNullOrEmpty(txtLname.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtAge.Text)
                || string.IsNullOrEmpty(txtPhone.Text))
            {
                MessageBox.Show("Please enter all fields", "Alert", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
            else { }
        }
    }
}
