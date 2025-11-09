using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace PROEL_PROJ
{
    public class Reports
    {
        private static string connectionString = Classes.ConString();

        //====================== PRINT REPORTS ======================//

        // Print All Active Students
        public static void PrintActiveStudents()
        {
            try
            {
                DataTable dt = GetActiveStudents();
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No active students found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"Active_Students_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    GenerateStudentsPDF(dt, saveDialog.FileName, "Active Students Report");
                    MessageBox.Show("Report generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Print All Active Teachers
        public static void PrintActiveTeachers()
        {
            try
            {
                DataTable dt = GetActiveTeachers();
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No active teachers found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"Active_Teachers_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    GenerateTeachersPDF(dt, saveDialog.FileName, "Active Teachers Report");
                    MessageBox.Show("Report generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Print All Subjects/Courses
        public static void PrintAllSubjects()
        {
            try
            {
                DataTable dt = GetAllCourses();
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No courses found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"All_Courses_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    GenerateCoursesPDF(dt, saveDialog.FileName, "All Courses Report");
                    MessageBox.Show("Report generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Print Students Per Subject
        public static void PrintStudentsPerSubject(int courseId, string courseName)
        {
            try
            {
                DataTable dt = GetStudentsPerCourse(courseId);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No students enrolled in this course.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"Students_In_{courseName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    GenerateStudentsPerCoursePDF(dt, saveDialog.FileName, $"Students Enrolled in {courseName}");
                    MessageBox.Show("Report generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Print Students Per Teacher
        public static void PrintStudentsPerTeacher(int instructorId, string teacherName)
        {
            try
            {
                DataTable dt = GetStudentsPerTeacher(instructorId);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No students found for this teacher.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"Students_Under_{teacherName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    GenerateStudentsPerTeacherPDF(dt, saveDialog.FileName, $"Students Under {teacherName}");
                    MessageBox.Show("Report generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //====================== DATA RETRIEVAL ======================//
        private static DataTable GetActiveStudents()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        s.StudentID,
                        p.FirstName,
                        p.LastName,
                        p.Age,
                        p.Gender,
                        p.Phone,
                        p.Email,
                        p.Address,
                        s.EnrollmentDate,
                        ISNULL(d.DepartmentName, 'Not Assigned') AS Department
                    FROM Profiles p
                    INNER JOIN Students s ON p.ProfileID = s.ProfileID
                    LEFT JOIN Enrollment e ON s.StudentID = e.StudentID
                    LEFT JOIN Courses c ON e.CourseID = c.CourseID
                    LEFT JOIN Departments d ON c.DepartmentID = d.DepartmentID
                    WHERE p.Status = 'ACTIVE'
                    GROUP BY s.StudentID, p.FirstName, p.LastName, p.Age, p.Gender, 
                             p.Phone, p.Email, p.Address, s.EnrollmentDate, d.DepartmentName
                    ORDER BY p.LastName, p.FirstName";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private static DataTable GetActiveTeachers()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        i.InstructorID,
                        p.FirstName,
                        p.LastName,
                        p.Age,
                        p.Gender,
                        p.Phone,
                        p.Email,
                        d.DepartmentName,
                        i.HireDate
                    FROM Profiles p
                    INNER JOIN Instructors i ON p.ProfileID = i.ProfileID
                    INNER JOIN Departments d ON i.DepartmentID = d.DepartmentID
                    WHERE p.Status = 'ACTIVE'
                    ORDER BY p.LastName, p.FirstName";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private static DataTable GetAllCourses()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        c.CourseID,
                        c.CourseName,
                        c.CourseCode,
                        c.Credits,
                        d.DepartmentName,
                        c.Description,
                        c.Status,
                        COUNT(DISTINCT e.StudentID) AS EnrolledStudents,
                        COUNT(DISTINCT ic.InstructorID) AS AssignedInstructors
                    FROM Courses c
                    INNER JOIN Departments d ON c.DepartmentID = d.DepartmentID
                    LEFT JOIN Enrollment e ON c.CourseID = e.CourseID
                    LEFT JOIN InstructorCourses ic ON c.CourseID = ic.CourseID
                    GROUP BY c.CourseID, c.CourseName, c.CourseCode, c.Credits, 
                             d.DepartmentName, c.Description, c.Status
                    ORDER BY d.DepartmentName, c.CourseName";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private static DataTable GetStudentsPerCourse(int courseId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        s.StudentID,
                        p.FirstName,
                        p.LastName,
                        p.Gender,
                        p.Email,
                        p.Phone,
                        se.AcademicYear,
                        se.TermName
                    FROM Enrollment e
                    INNER JOIN Students s ON e.StudentID = s.StudentID
                    INNER JOIN Profiles p ON s.ProfileID = p.ProfileID
                    LEFT JOIN Semesters se ON e.SemesterID = se.SemesterID
                    WHERE e.CourseID = @CourseID AND p.Status = 'ACTIVE'
                    ORDER BY p.LastName, p.FirstName";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@CourseID", courseId);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private static DataTable GetStudentsPerTeacher(int instructorId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT DISTINCT
                        s.StudentID,
                        p.FirstName,
                        p.LastName,
                        p.Gender,
                        p.Email,
                        c.CourseName,
                        c.CourseCode
                    FROM InstructorCourses ic
                    INNER JOIN Courses c ON ic.CourseID = c.CourseID
                    INNER JOIN Enrollment e ON c.CourseID = e.CourseID
                    INNER JOIN Students s ON e.StudentID = s.StudentID
                    INNER JOIN Profiles p ON s.ProfileID = p.ProfileID
                    WHERE ic.InstructorID = @InstructorID AND p.Status = 'ACTIVE'
                    ORDER BY c.CourseName, p.LastName, p.FirstName";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@InstructorID", instructorId);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        //====================== PDF GENERATION ======================//

        private static void GenerateStudentsPDF(DataTable dt, string filename, string title)
        {
            Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, new FileStream(filename, FileMode.Create));
            doc.Open();

            AddHeader(doc, title);
            PdfPTable table = new PdfPTable(9) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 8f, 12f, 12f, 6f, 8f, 12f, 15f, 15f, 12f });

            AddTableHeader(table, new[] { "ID", "First Name", "Last Name", "Age", "Gender", "Phone", "Email", "Address", "Enrolled" });

            foreach (DataRow row in dt.Rows)
            {
                table.AddCell(CreateCell(row["StudentID"].ToString()));
                table.AddCell(CreateCell(row["FirstName"].ToString()));
                table.AddCell(CreateCell(row["LastName"].ToString()));
                table.AddCell(CreateCell(row["Age"].ToString()));
                table.AddCell(CreateCell(row["Gender"].ToString()));
                table.AddCell(CreateCell(row["Phone"].ToString()));
                table.AddCell(CreateCell(row["Email"].ToString()));
                table.AddCell(CreateCell(row["Address"].ToString()));
                table.AddCell(CreateCell(Convert.ToDateTime(row["EnrollmentDate"]).ToString("yyyy-MM-dd")));
            }

            doc.Add(table);
            AddFooter(doc, dt.Rows.Count);
            doc.Close();
        }

        private static void GenerateTeachersPDF(DataTable dt, string filename, string title)
        {
            Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, new FileStream(filename, FileMode.Create));
            doc.Open();

            AddHeader(doc, title);
            PdfPTable table = new PdfPTable(9) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 7f, 12f, 12f, 6f, 7f, 12f, 18f, 14f, 12f });

            AddTableHeader(table, new[] { "ID", "First Name", "Last Name", "Age", "Gender", "Phone", "Email", "Department", "Hire Date" });

            foreach (DataRow row in dt.Rows)
            {
                table.AddCell(CreateCell(row["InstructorID"].ToString()));
                table.AddCell(CreateCell(row["FirstName"].ToString()));
                table.AddCell(CreateCell(row["LastName"].ToString()));
                table.AddCell(CreateCell(row["Age"].ToString()));
                table.AddCell(CreateCell(row["Gender"].ToString()));
                table.AddCell(CreateCell(row["Phone"].ToString()));
                table.AddCell(CreateCell(row["Email"].ToString()));
                table.AddCell(CreateCell(row["DepartmentName"].ToString()));
                table.AddCell(CreateCell(Convert.ToDateTime(row["HireDate"]).ToString("yyyy-MM-dd")));
            }

            doc.Add(table);
            AddFooter(doc, dt.Rows.Count);
            doc.Close();
        }

        //====================== HELPER METHODS ======================//

        private static void GenerateCoursesPDF(DataTable dt, string filename, string title)
        {
            Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, new FileStream(filename, FileMode.Create));
            doc.Open();

            AddHeader(doc, title);
            PdfPTable table = new PdfPTable(8) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 8f, 20f, 10f, 8f, 15f, 20f, 8f, 10f });

            AddTableHeader(table, new[] { "ID", "Course Name", "Code", "Credits", "Department", "Description", "Status", "Enrolled" });

            foreach (DataRow row in dt.Rows)
            {
                table.AddCell(CreateCell(row["CourseID"].ToString()));
                table.AddCell(CreateCell(row["CourseName"].ToString()));
                table.AddCell(CreateCell(row["CourseCode"].ToString()));
                table.AddCell(CreateCell(row["Credits"].ToString()));
                table.AddCell(CreateCell(row["DepartmentName"].ToString()));
                table.AddCell(CreateCell(row["Description"].ToString()));
                table.AddCell(CreateCell(row["Status"].ToString()));
                table.AddCell(CreateCell(row["EnrolledStudents"].ToString()));
            }

            doc.Add(table);
            AddFooter(doc, dt.Rows.Count);
            doc.Close();
        }

        private static void GenerateStudentsPerCoursePDF(DataTable dt, string filename, string title)
        {
            Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, new FileStream(filename, FileMode.Create));
            doc.Open();

            AddHeader(doc, title);
            PdfPTable table = new PdfPTable(7) { WidthPercentage = 100 }; // now 7 columns instead of 8
            table.SetWidths(new float[] { 8f, 12f, 12f, 8f, 15f, 12f, 10f });

            AddTableHeader(table, new[] { "ID", "First Name", "Last Name", "Gender", "Email", "Phone", "Year" });

            foreach (DataRow row in dt.Rows)
            {
                table.AddCell(CreateCell(row["StudentID"].ToString()));
                table.AddCell(CreateCell(row["FirstName"].ToString()));
                table.AddCell(CreateCell(row["LastName"].ToString()));
                table.AddCell(CreateCell(row["Gender"].ToString()));
                table.AddCell(CreateCell(row["Email"].ToString()));
                table.AddCell(CreateCell(row["Phone"].ToString()));
                table.AddCell(CreateCell(row["AcademicYear"].ToString()));
            }

            doc.Add(table);
            AddFooter(doc, dt.Rows.Count);
            doc.Close();
        }

        private static void GenerateStudentsPerTeacherPDF(DataTable dt, string filename, string title)
        {
            Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, new FileStream(filename, FileMode.Create));
            doc.Open();

            AddHeader(doc, title);
            PdfPTable table = new PdfPTable(7) { WidthPercentage = 100 }; // 7 columns
            table.SetWidths(new float[] { 8f, 12f, 12f, 8f, 20f, 10f, 10f });

            AddTableHeader(table, new[] { "ID", "First Name", "Last Name", "Gender", "Email", "Course", "Code" });

            foreach (DataRow row in dt.Rows)
            {
                table.AddCell(CreateCell(row["StudentID"].ToString()));
                table.AddCell(CreateCell(row["FirstName"].ToString()));
                table.AddCell(CreateCell(row["LastName"].ToString()));
                table.AddCell(CreateCell(row["Gender"].ToString()));
                table.AddCell(CreateCell(row["Email"].ToString()));
                table.AddCell(CreateCell(row["CourseName"].ToString()));
                table.AddCell(CreateCell(row["CourseCode"].ToString()));
            }

            doc.Add(table);
            AddFooter(doc, dt.Rows.Count);
            doc.Close();
        }

        private static void AddHeader(Document doc, string title)
        {
            Paragraph header = new Paragraph(title.ToUpper(), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10f
            };
            doc.Add(header);

            Paragraph date = new Paragraph($"Generated on: {DateTime.Now:MMMM dd, yyyy hh:mm tt}", FontFactory.GetFont(FontFactory.HELVETICA, 10))
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingAfter = 20f
            };
            doc.Add(date);
        }

        private static void AddTableHeader(PdfPTable table, string[] headers)
        {
            foreach (string header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                {
                    BackgroundColor = new BaseColor(200, 200, 200),
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(cell);
            }
        }

        private static PdfPCell CreateCell(string text)
        {
            return new PdfPCell(new Phrase(text, FontFactory.GetFont(FontFactory.HELVETICA, 9)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
        }

        private static void AddFooter(Document doc, int recordCount)
        {
            Paragraph footer = new Paragraph($"Total Records: {recordCount}", FontFactory.GetFont(FontFactory.HELVETICA, 10))
            {
                Alignment = Element.ALIGN_LEFT,
                SpacingBefore = 15f
            };
            doc.Add(footer);
        }
    }
}
