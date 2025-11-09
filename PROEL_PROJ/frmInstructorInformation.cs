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
    public partial class frmInstructorInformation : Form
    {
        public frmInstructorInformation()
        {
            InitializeComponent();
        }
        string connectionString = Classes.ConString();
        private void frmInstructorInformation_Load(object sender, EventArgs e)
        {
            lblUser.Text = Classes.FullName;
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnTeachersInfo);
            Classes.ApplySidebarStyle(btnLogOut);

            LoadTeacherInfo();
        }

        private void LoadTeacherInfo()
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
                            d.DepartmentName,
                            i.HireDate
                        FROM Instructors i
                        INNER JOIN Profiles p ON i.ProfileID = p.ProfileID
                        INNER JOIN Users u ON p.ProfileID = u.ProfileID
                        INNER JOIN Departments d ON i.DepartmentID = d.DepartmentID
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
                        txtDepartment.Text = reader["DepartmentName"].ToString();
                        txtHireDate.Text = Convert.ToDateTime(reader["HireDate"]).ToString("MM/dd/yyyy");

                        MessageBox.Show("Teacher information loaded successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Teacher information not found.", "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading teacher information: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
