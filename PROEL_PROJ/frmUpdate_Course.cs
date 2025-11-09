using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROEL_PROJ
{
    public partial class frmUpdate_Course : Form
    {
        private int profileId;
        private string Status;

        public frmUpdate_Course(int id, string coursename, string coursecode, int credit,
                            string department, string description, string status)
        {
            InitializeComponent();

            profileId = id;
            txtCourseName.Text = coursename;
            txtCourseCode.Text = coursecode;
            txtCredit.Text = credit.ToString();
            cmbDepartment.SelectedItem = department;
            txtDescription.Text = description;
            Status = status;

            Course.LoadInstructors(cmbDepartment);

            cmbDepartment.Text = department;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want update this?", "Confirmation",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                Course.UpdateCourseInfo(
                 profileId,
                 txtCourseName.Text,
                 txtCourseCode.Text,
                 Convert.ToInt32(txtCredit.Text),
                 cmbDepartment.SelectedValue.ToString(),
                 txtDescription.Text,
                 Status);

                Logs.Record("Updated Course",
                $"Course {txtCourseName.Text} was Updated by {Logs.CurrentUserName}.",
                txtCourseName.Text);
                clearfileds();
            }
            else { }
        }

        private void clearfileds()
        {
            txtCourseName.Clear();
            txtCourseCode.Clear();
            txtCredit.Clear();
            txtDescription.Clear();
            cmbDepartment.SelectedValue = -1;

        }

        private void frmUpdate_Course_Load(object sender, EventArgs e)
        {
            Course.LoadInstructors(cmbDepartment);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmCourse course = new frmCourse();
            this.Hide();
            course.ShowDialog();
        }
    }
}
