using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROEL_PROJ
{
    public static class Logs
    {
        private static readonly string connectionString = Classes.ConString();

        public static string CurrentUserId { get; set; } = "No Userid";
        public static string CurrentUserName { get; set; } = "Unknown User";

        public static void Record(string action, string description, string affectedFirstName = "", string affectedLastName = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                INSERT INTO Logs (UserID, Action, FirstName, LastName, Description, DateLogged)
                VALUES (@UserID, @Action, @FirstName, @LastName, @Description, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        string[] nameParts = CurrentUserName?.Split(' ') ?? new string[0];
                        string currentFirstName = nameParts.Length > 0 ? nameParts[0] : "";
                        string currentLastName = nameParts.Length > 1 ? nameParts[1] : "";

                        string fullDescription = $"{description} (by {currentFirstName} {currentLastName})";

                        cmd.Parameters.AddWithValue("@UserID", CurrentUserId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@FirstName", affectedFirstName);
                        cmd.Parameters.AddWithValue("@LastName", affectedLastName);
                        cmd.Parameters.AddWithValue("@Description", fullDescription);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error recording log: " + ex.Message, "Logging Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void LoadLogs(DataGridView dgv)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            L.LogID,
                            L.Action,
                            L.FirstName,
                            L.LastName,
                            L.Description,
                            L.DateLogged
                        FROM Logs L
                        ORDER BY L.LogID DESC";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logs: " + ex.Message, "Load Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SearchLogs(DataGridView dgv, string keyword = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            L.LogID,
                            L.Action,
                            L.FirstName,
                            L.LastName,
                            L.Description,
                            L.DateLogged
                        FROM Logs L";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += @"
                            WHERE 
                                CAST(L.LogID AS NVARCHAR) LIKE @keyword OR
                                L.Action LIKE @keyword OR
                                L.FirstName LIKE @keyword OR
                                L.LastName LIKE @keyword OR
                                L.Description LIKE @keyword OR
                                CONVERT(NVARCHAR, L.DateLogged, 120) LIKE @keyword";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (!string.IsNullOrEmpty(keyword))
                            cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching logs: " + ex.Message, "Search Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
