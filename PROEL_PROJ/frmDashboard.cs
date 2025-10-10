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
    public partial class frmDashboard : Form
    {
        public frmDashboard()
        {
            InitializeComponent();
            
        }

        string connectionString = Classes.ConString();
        private void frmDashboard_Load(object sender, EventArgs e)
        {
            Classes.ApplySidebarStyle(btnDashboard);
            Classes.ApplySidebarStyle(btnStudents);
            Classes.ApplySidebarStyle(btnTeacher);
            Classes.ApplySidebarStyle(btnLogs);
            Classes.ApplySidebarStyle(btnLogOut);
            Classes.ApplySidebarStyle(btnCourse);

            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();

                string queryStudents = @"
        SELECT 
            p.ProfileID, 
            p.FirstName, 
            p.LastName, 
            p.Age, 
            p.Gender, 
            p.Email, 
            r.RoleName
        FROM Profiles p
        INNER JOIN Roles r ON p.RoleID = r.RoleID
        INNER JOIN Students s ON p.ProfileID = s.ProfileID
        WHERE r.RoleName = 'STUDENT' AND p.Status = 'ACTIVE'
        ORDER BY p.ProfileID DESC";

                SqlDataAdapter sqlDa = new SqlDataAdapter(queryStudents, sqlCon);
                DataTable dt = new DataTable();
                sqlDa.Fill(dt);

                dgvTotal_Stud.DataSource = dt;
            }




            LoadPopulationCharts();
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud update = new frmUpdate_Stud();
            this.Hide();
            update.ShowDialog();
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
            frmTeachers teachers = new frmTeachers();
            this.Hide();
            teachers.ShowDialog();
        }

        private void btnLogOut_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Would you like to Log out?", "Confirmation",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                frmLogIn logIn = new frmLogIn();
                this.Hide();
                logIn.ShowDialog();
            }
            else { }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {

        }

        private void btnStudents_Click_1(object sender, EventArgs e)
        {
            frmUpdate_Stud update = new frmUpdate_Stud();
            this.Hide();
            update.ShowDialog();
        }

        private void btnTeacher_Click_1(object sender, EventArgs e)
        {
            frmTeachers teachers = new frmTeachers();
            this.Hide();
            teachers.ShowDialog();
        }

        private void btnCourse_Click(object sender, EventArgs e)
        {
            frmCourse frmCourse= new frmCourse();
            this.Hide();
            frmCourse.ShowDialog();
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            frmLogs frmLogs = new frmLogs();
            this.Hide();
            frmLogs.ShowDialog();
        }

        private void LoadPopulationCharts()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Classes.ConString()))
                {
                    con.Open();

                    // ---- Teachers ----
                    SqlCommand cmdTeachers = new SqlCommand(@"
                SELECT 
                    SUM(CASE WHEN p.Status='ACTIVE' THEN 1 ELSE 0 END) AS ActiveCount,
                    SUM(CASE WHEN p.Status='INACTIVE' THEN 1 ELSE 0 END) AS InactiveCount
                FROM Profiles p
                INNER JOIN Instructors i ON p.ProfileID = i.ProfileID
            ", con);

                    SqlDataReader reader = cmdTeachers.ExecuteReader();
                    if (reader.Read())
                    {
                        int active = Convert.ToInt32(reader["ActiveCount"]);
                        int inactive = Convert.ToInt32(reader["InactiveCount"]);

                        chartTeachers.Series.Clear();
                        chartTeachers.Series.Add("Teachers");
                        chartTeachers.Series["Teachers"].Points.Clear();
                        chartTeachers.Series["Teachers"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

                        chartTeachers.Series["Teachers"].Points.AddXY("Active", active);
                        chartTeachers.Series["Teachers"].Points.AddXY("Inactive", inactive);

                        chartTeachers.Series["Teachers"].IsValueShownAsLabel = true;
                    }
                    reader.Close();

                    // ---- Students ----
                    SqlCommand cmdStudents = new SqlCommand(@"
                SELECT 
                    SUM(CASE WHEN p.Status='ACTIVE' THEN 1 ELSE 0 END) AS ActiveCount,
                    SUM(CASE WHEN p.Status='INACTIVE' THEN 1 ELSE 0 END) AS InactiveCount
                FROM Profiles p
                INNER JOIN Students s ON p.ProfileID = s.ProfileID
            ", con);

                    reader = cmdStudents.ExecuteReader();
                    if (reader.Read())
                    {
                        int active = Convert.ToInt32(reader["ActiveCount"]);
                        int inactive = Convert.ToInt32(reader["InactiveCount"]);

                        chartStudents.Series.Clear();
                        chartStudents.Series.Add("Students");
                        chartStudents.Series["Students"].Points.Clear();
                        chartStudents.Series["Students"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

                        chartStudents.Series["Students"].Points.AddXY("Active", active);
                        chartStudents.Series["Students"].Points.AddXY("Inactive", inactive);

                        chartStudents.Series["Students"].IsValueShownAsLabel = true;
                    }
                    reader.Close();

                    // ---- Courses ----
                    SqlCommand cmdCourses = new SqlCommand(@"
                SELECT 
                    SUM(CASE WHEN Status='ACTIVE' THEN 1 ELSE 0 END) AS ActiveCount,
                    SUM(CASE WHEN Status='INACTIVE' THEN 1 ELSE 0 END) AS InactiveCount
                FROM Courses
            ", con);

                    reader = cmdCourses.ExecuteReader();
                    if (reader.Read())
                    {
                        int active = Convert.ToInt32(reader["ActiveCount"]);
                        int inactive = Convert.ToInt32(reader["InactiveCount"]);

                        chartCourses.Series.Clear();
                        chartCourses.Series.Add("Courses");
                        chartCourses.Series["Courses"].Points.Clear();
                        chartCourses.Series["Courses"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

                        chartCourses.Series["Courses"].Points.AddXY("Active", active);
                        chartCourses.Series["Courses"].Points.AddXY("Inactive", inactive);

                        chartCourses.Series["Courses"].IsValueShownAsLabel = true;
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading chart data:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
