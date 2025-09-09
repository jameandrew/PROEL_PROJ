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
    public partial class frmLogIn : Form
    {

        public frmLogIn()
        {
            InitializeComponent();
        }

        string connectionString = Classes.ConString();
        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUname.Text.Trim();
            string plainPassword = txtPword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(plainPassword))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("Log_User", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@USERNAME", txtUname.Text.Trim());
                cmd.Parameters.AddWithValue("@PASSWORD", txtPword.Text.Trim());

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    MessageBox.Show("No matching rows returned from SQL.");
                }

                if (reader.Read())
                {
                    string role = reader["RoleName"].ToString();
                    MessageBox.Show("Log-In Successfully, Welcome: " + username + " (" + role + ")");

                    frmDashboard dashboard = new frmDashboard();
                    this.Hide();
                    dashboard.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {
            lblSignUp.ForeColor = Color.LightGray;
            this.Cursor = Cursors.Hand;
        }

        private void lblSignUp_MouseLeave(object sender, EventArgs e)
        {
            lblSignUp.ForeColor= Color.Black;
            this.Cursor = Cursors.Default;
        }

        private void lblSignUp_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Would you like to Sign Up?", "Conformation",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                frmSign_Up sign_Up = new frmSign_Up();
                this.Hide();
                sign_Up.ShowDialog();
            }
        }
    }
}
