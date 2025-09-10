using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROEL_PROJ
{
    public class Classes
    {
        private SqlDataAdapter sqlData;
        private DataTable dt;
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
        }

        public DataTable LoadData(string query, DataGridView dgv)
        {
           
            using (SqlConnection con = new SqlConnection(ConString()))
            {

                sqlData = new SqlDataAdapter("SELECT * FROM Profiles WHERE Firstname <> 'nimad' AND Lastname <> 'nimad'", con);
                sqlData = new SqlDataAdapter("SELECT * FROM Profiles", con);
                SqlCommandBuilder builder = new SqlCommandBuilder(sqlData);

                dt = new DataTable();
                sqlData.Fill(dt);

                dgv.DataSource = dt;
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
            LoadData(ConString(), dgv);
            dgv.Refresh();
        }

        public void DisplayName(string username, Label lbl)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();
                string query = "Select P.Firstname + ' ' + P.Lastname AS Fullname From Users U Inner Join Profiles P ON U.ProfileID = P.ProfileID Where U.Username = @USERNAME";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@USERNAME", username);

                object result = cmd.ExecuteScalar();

                if(result != null)
                {
                    lbl.Text = result.ToString();
                }
                

            }
        }

    }
}
