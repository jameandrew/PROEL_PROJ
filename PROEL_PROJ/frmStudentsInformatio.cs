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
    public partial class frmStudentsInformatio : Form
    {
        public frmStudentsInformatio()
        {
            InitializeComponent();
        }
        string connectionString = Classes.ConString();

        private void frmStudentsInformatio_Load(object sender, EventArgs e)
        {
            lblUser.Text = Classes.FullName;
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnLogOut);

            LoadStudentsInfoView();
        }

        private void LoadStudentsInfoView()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            p.FirstName,
                            p.LastName,
                            p.Age,
                            p.Gender,
                            p.Phone,
                            p.Address,
                            p.Email,
                            p.Status,
                            s.EnrollmentDate
                        FROM Students s
                        INNER JOIN Profiles p ON s.ProfileID = p.ProfileID
                        INNER JOIN Users u ON p.ProfileID = u.ProfileID
                        WHERE u.Username = @Username";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", Classes.Username);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Assign values to your text fields/labels
                        // Replace these with your actual control names
                        txtFirstName.Text = reader["FirstName"].ToString();
                        txtLastName.Text = reader["LastName"].ToString();
                        txtAge.Text = reader["Age"].ToString();
                        txtGender.Text = reader["Gender"].ToString();
                        txtPhone.Text = reader["Phone"].ToString();
                        txtAddress.Text = reader["Address"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtStatus.Text = reader["Status"].ToString();
                        txtEnrollmentDate.Text = Convert.ToDateTime(reader["EnrollmentDate"]).ToString("MM/dd/yyyy");

                        MessageBox.Show("Student information loaded successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Student information not found.", "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student information: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmStudDashboard frmStudDashboard = new frmStudDashboard();
            this.Hide();
            frmStudDashboard.Show();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?",
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
                frmLogIn frmLogIn = new frmLogIn();
                this.Show();
            }
        }
    }
}
