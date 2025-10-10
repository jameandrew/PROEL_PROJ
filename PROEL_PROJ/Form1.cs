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
        private Classes classes;
        private static int loginAttempts = 0;
        private const int MaxAttempts = 3;
        public frmLogIn()
        {
            InitializeComponent();
            classes = new Classes();

            this.AcceptButton = btnLogin;
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
            using (SqlCommand cmd = new SqlCommand("Log_User", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USERNAME", username);
                cmd.Parameters.AddWithValue("@PASSWORD", plainPassword);

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string userId = (reader["UserID"].ToString());
                        string firstName = reader["Firstname"].ToString();
                        string lastName = reader["Lastname"].ToString();
                        string role = reader["RoleName"].ToString();

                        MessageBox.Show($"Log-In Successfully, Welcome: {firstName} {lastName} ({role})");

                        Logs.CurrentUserId = userId;
                        Logs.CurrentUserName = $"{firstName} {lastName}";

                        Logs.Record("Login", $"{Logs.CurrentUserName} logged in successfully.");

                        loginAttempts = 0;

                        frmDashboard dashboard = new frmDashboard();
                        this.Hide();
                        dashboard.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        loginAttempts++;
                        int remaining = MaxAttempts - loginAttempts;

                        if (remaining > 0)
                        {
                            MessageBox.Show($"Invalid username or password. You have {remaining} attempt(s) left.");
                        }
                        else
                        {
                            MessageBox.Show("Maximum login attempts reached. You are locked out.");
                            btnLogin.Enabled = false;
                        }
                    }
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

        private void frmLogIn_Load(object sender, EventArgs e)
        {
        }
    }
}
