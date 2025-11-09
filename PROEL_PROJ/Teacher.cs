using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROEL_PROJ
{
    public class Teacher
    {
        private SqlDataAdapter sqlData;
        private DataTable dt;

        public static string ConString()
        {
            return @"Data Source=DESKTOP-4A3R3RB\SQLEXPRESS;
            Initial Catalog=FINAL_DB;Integrated Security=True";
        }

        public DataTable LoadDataTeacher(string query, DataGridView dgv)
        {


            using (SqlConnection con = new SqlConnection(ConString()))
            {
                sqlData = new SqlDataAdapter(@"
                SELECT 
                s.InstructorID, 
                p.ProfileID, 
                p.Firstname, 
                p.Lastname, 
                p.Age, 
                p.Gender, 
                p.Phone, 
                p.Address, 
                p.Email, 
                p.Status, 
                s.HireDate, 
                d.DepartmentName
                FROM Profiles p
                INNER JOIN Instructors s ON p.ProfileID = s.ProfileID
                INNER JOIN Departments d ON s.DepartmentID = d.DepartmentID
                WHERE p.RoleID <> 1 AND p.RoleID <> 2 
                AND p.Status = 'ACTIVE'
                ORDER BY p.ProfileID DESC;", con);

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

        public static void ShowCountTeach(Label act, Label pnd, Label inact)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();
                string query = @"
                               SELECT Status, COUNT(*) AS Total
                               FROM Profiles P
                               INNER JOIN Instructors S ON P.ProfileID = S.ProfileID
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

        public static void SearchFieldsTeach(DataGridView dgv, string keyword = "")
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();

                string query = @"
                  SELECT 
                P.ProfileID, 
                P.FirstName, 
                P.LastName, 
                P.Age, 
                P.Gender, 
                P.Phone, 
                P.Address, 
                P.Email, 
                P.Status,
                S.HireDate,
                D.DepartmentName
                FROM Profiles P
                INNER JOIN Instructors S ON P.ProfileID = S.ProfileID
                INNER JOIN Departments D ON S.DepartmentID = D.DepartmentID
                WHERE P.Status = 'ACTIVE'";

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += @"
                    AND 
                    CAST(P.ProfileID AS NVARCHAR) LIKE @keyword OR
                    P.FirstName LIKE @keyword OR
                    P.LastName LIKE @keyword OR
                    CAST(P.Age AS NVARCHAR) LIKE @keyword OR
                    P.Gender LIKE @keyword OR
                    P.Phone LIKE @keyword OR
                    P.Address LIKE @keyword OR
                    P.Email LIKE @keyword OR
                    P.Status LIKE @keyword OR
                    CAST(S.HireDate AS NVARCHAR) LIKE @keyword OR
                    D.DepartmentName LIKE @keyword";
                }
                query += " ORDER BY ProfileID DESC";
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

        public static void UpdateTeacherInfo(int profileId, string firstName, string lastName, int age,
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
    }
}
