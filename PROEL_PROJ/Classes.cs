using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace PROEL_PROJ
{
    public class Classes
    {
        private SqlDataAdapter sqlData;
        private DataTable dt;

        public static string UserID { get; set; }
        public static string Username { get; set; }
        public static int ProfileID { get; set; }
        public static string FullName { get; set; }
        public static string RoleName { get; set; }

        public static string ConString()
        {
            return @"Data Source=DESKTOP-4A3R3RB\SQLEXPRESS;
            Initial Catalog=FINAL_DB;Integrated Security=True";
        }

        public static void ApplySidebarStyle(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(169, 200, 184);
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(122, 144, 117);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(82, 104, 77);
            btn.Cursor = Cursors.Hand;
        }

        public static void transparent(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.Transparent;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn.Cursor = Cursors.Hand;
        }

        public DataTable LoadDataStudent(string query, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                sqlData = new SqlDataAdapter(@"
                                               SELECT 
                                               s.StudentID,
                                               p.ProfileID, 
                                               p.FirstName, 
                                               p.LastName, 
                                               p.Age, 
                                               p.Gender, 
                                               p.Phone, 
                                               p.Address, 
                                               p.Email, 
                                               p.Status, 
                                               s.EnrollmentDate
                                               FROM Profiles p
                                               INNER JOIN Students s ON p.ProfileID = s.ProfileID
                                               WHERE p.RoleID <> 1 AND p.RoleID <> 3 AND p.Status = 'ACTIVE'
                                               ORDER BY ProfileID DESC", con);
                SqlCommandBuilder builder = new SqlCommandBuilder(sqlData);
                dt = new DataTable();
                sqlData.Fill(dt);
                dgv.DataSource = dt;

                if (!dgv.Columns.Contains("btnAction"))
                {
                    DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
                    btnCol.HeaderText = "";
                    btnCol.Name = "btnAction";
                    btnCol.Text = "View";
                    btnCol.UseColumnTextForButtonValue = true;
                    dgv.Columns.Add(btnCol);
                }

                dgv.Columns["btnAction"].DisplayIndex = dgv.Columns.Count - 1;
            }
            return dt;
        }

        public void UpdateStatus(int profileId, string status)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();
                string query = "UPDATE profiles SET Status = @Status WHERE ProfileID = @ProfileID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ProfileID", profileId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RefreshDB(DataGridView dgv)
        {
            LoadDataStudent(ConString(), dgv);
            dgv.Refresh();
        }

        public static void ShowCountStud(Label act, Label pnd, Label inact)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();
                string query = @"
                               SELECT Status, COUNT(*) AS Total
                               FROM Profiles P
                               INNER JOIN Students S ON P.ProfileID = S.ProfileID
                               GROUP BY Status;";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                int activeCount = 0;
                int pendingCount = 0;
                int inactiveCount = 0;

                while (reader.Read())
                {
                    string status = reader["Status"].ToString();
                    int total = Convert.ToInt32(reader["Total"]);
                    switch (status)
                    {
                        case "ACTIVE":
                            activeCount = total;
                            break;
                        case "PENDING":
                            pendingCount = total;
                            break;
                        case "INACTIVE":
                            inactiveCount = total;
                            break;
                    }
                }
                act.Text = activeCount.ToString();
                pnd.Text = pendingCount.ToString();
                inact.Text = inactiveCount.ToString();
            }
        }

        public static void UpdateStudentInfo(int profileId, string firstName, string lastName, int age,
                                     string gender, string phone, string address,
                                     string email, string status)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();

                string query = @"UPDATE Profiles
                         SET FirstName = @FirstName,
                             LastName = @LastName,
                             Age = @Age,
                             Gender = @Gender,
                             Phone = @Phone,
                             Address = @Address,
                             Email = @Email,
                             Status = @Status
                             
                         WHERE ProfileID = @ProfileID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Age", age);
                    cmd.Parameters.AddWithValue("@Gender", gender);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ProfileID", profileId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Student information updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Update failed. Student not found.");
                    }
                }
            }
        }

        public static void SearchFields(DataGridView dgv, string keyword = "")
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();

                string query = @"
                   SELECT P.ProfileID, P.FirstName, P.LastName, P.Age, P.Gender, 
                   P.Phone, P.Address, P.Email, P.Status, S.EnrollmentDate
                   FROM Profiles P
                   INNER JOIN Students S ON P.ProfileID = S.ProfileID";

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += @"
                    WHERE 
                    CAST(P.ProfileID AS NVARCHAR) LIKE @keyword OR
                    P.FirstName LIKE @keyword OR
                    P.LastName LIKE @keyword OR
                    CAST(P.Age AS NVARCHAR) LIKE @keyword OR
                    P.Gender LIKE @keyword OR
                    P.Phone LIKE @keyword OR
                    P.Address LIKE @keyword OR
                    P.Email LIKE @keyword OR
                    P.Status LIKE @keyword OR
                    S.ENROLLMENTDATE LIKE @keyword";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgv.DataSource = dt;
                }
            }
        }

        public static void MakeStudentInfoReadOnly(TextBox fname, TextBox lname, TextBox age, ComboBox gender, TextBox phone, TextBox address, TextBox email,
             ComboBox course, ComboBox department)
        {
            fname.ReadOnly = true;
            lname.ReadOnly = true;
            age.ReadOnly = true;
            gender.Enabled = false;
            phone.ReadOnly = true;
            address.ReadOnly = true;
            email.ReadOnly = true;

            course.Enabled = true;
            department.Enabled = true;
        }

        public static void DisplayName(TextBox user, TextBox password)
        {
            using (SqlConnection con = new SqlConnection(Classes.ConString()))
            {
                con.Open();
                string query = @"
                SELECT 
                u.UserID,
                u.Username,
                u.ProfileID,
                CONCAT(p.FirstName, ' ', p.LastName) AS FullName,
                r.RoleName
                FROM Users u
                INNER JOIN Profiles p ON u.ProfileID = p.ProfileID
                INNER JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.Username = @Username AND u.Password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", user.Text);
                    cmd.Parameters.AddWithValue("@Password", password.Text);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        UserID = reader["UserID"].ToString();
                        Username = reader["Username"].ToString();  // ADDED
                        ProfileID = Convert.ToInt32(reader["ProfileID"]);
                        FullName = reader["FullName"].ToString();
                        RoleName = reader["RoleName"].ToString();

                        MessageBox.Show($"Welcome {FullName}!", "Login Successful",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Login Failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}