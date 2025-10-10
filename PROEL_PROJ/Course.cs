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
    public class Course
    {
        private SqlDataAdapter sqlData;
        private DataTable dt;
        public static string ConString()
        {
            return @"Data Source=DESKTOP-4A3R3RB\SQLEXPRESS;
            Initial Catalog=FINAL_DB;Integrated Security=True";
        }

        public DataTable LoadCourses(string query, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
              sqlData = new SqlDataAdapter(@"
                                            SELECT 
                                            c.CourseID,
                                            c.CourseName,
                                            c.CourseCode,
                                            c.Description,
                                            c.Credits,
                                            c.Status,
                                            d.DepartmentName
                                            FROM 
                                            Courses c
                                            INNER JOIN Departments d ON c.DepartmentID = d.DepartmentID
                                            WHERE c.Status = 'ACTIVE'
                                            ORDER BY 
                                            c.CourseID DESC;", con);
                SqlCommandBuilder builder = new SqlCommandBuilder(sqlData);

                dt = new DataTable();
                sqlData.Fill(dt);
                dgv.DataSource = dt;
                if (!dgv.Columns.Contains("btnAction"))
                {
                    DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
                    btnCol.HeaderText = "Details";
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
                string query = "UPDATE Courses SET Status = @Status WHERE CourseID = @COURSEID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@COURSEID", profileId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RefreshDB(DataGridView dgv)
        {
            LoadCourses(ConString(), dgv);
            dgv.Refresh();
        }

        public static void ShowCount(Label act, Label pnd, Label inact)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();
                string query = @" SELECT Status, COUNT(*) AS Total
                               FROM COURSES
                               GROUP BY Status;
                              ";

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
                        case "Pending":
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

        public static void LoadInstructors(ComboBox cmb)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"SELECT DepartmentID, DepartmentName FROM Departments", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmb.DataSource = dt;
                cmb.DisplayMember = "DepartmentName";
                cmb.ValueMember = "DepartmentID";
                cmb.SelectedIndex = -1;
            }
        }

        public static void UpdateCourseInfo(int courseid, string coursename, string coursecode, int credit, string instructor,
                                     string description, string status)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();

                string query = @"UPDATE Courses
                         SET CourseName = @CourseName,
                             CourseCode = @CourseCode,
                             Credits = @Credits,
                              DepartmentID = @DepartmentID,
                             Description = @Description
                            WHERE CourseID = @CourseID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseid);
                    cmd.Parameters.AddWithValue("@CourseName", coursename);
                    cmd.Parameters.AddWithValue("@CourseCode", coursecode);
                    cmd.Parameters.AddWithValue("@Credits", credit);
                    cmd.Parameters.AddWithValue("@DepartmentID", instructor);
                    cmd.Parameters.AddWithValue("@Description", description);


                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Course information updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Update failed. Course not found.");
                    }
                }
            }
        }

        public static void ReLoadCourse(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(ConString()))
            {
                con.Open();
                string query = @"SELECT 
                                            c.CourseID,
                                            c.CourseName,
                                            c.CourseCode,
                                            c.Description,
                                            c.Credits,
                                            c.Status,
                                            d.DepartmentName
                                            FROM 
                                            Courses c
                                            INNER JOIN Departments d ON i.DepartmentID = d.DepartmentID
                                            ORDER BY 
                                            c.CourseID DESC;";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;
            }
        }

    }
}
