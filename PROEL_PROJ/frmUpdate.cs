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
    public partial class frmUpdate : Form
    {
        private int profileId;
        private string Status;

        public frmUpdate(int id, string firstName, string lastName, int age,
                            string gender, string phone, string address,
                            string email, string status)
        {
            InitializeComponent();

            profileId = id;
            txtFname.Text = firstName;
            txtLname.Text = lastName;
            txtAge.Text = age.ToString();
            cmbGender.SelectedItem = gender;
            txtPhone.Text = phone;
            txtAddress.Text = address;
            txtEmail.Text = email;
            Status = status;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Classes.UpdateStudentInfo(
            profileId,
            txtFname.Text,
            txtLname.Text,
            Convert.ToInt32(txtAge.Text),
            cmbGender.SelectedItem.ToString(),
            txtPhone.Text,
            txtAddress.Text,
            txtEmail.Text,
            Status
            );

            Logs.Record("Updated Student",
                $"Student {txtFname.Text} {txtLname.Text} was Updated by {Logs.CurrentUserName}.",
                txtFname.Text, txtLname.Text);
            this.Close();
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            Classes.transparent(btnBack);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmUpdate_Stud frmUpdate_Stud = new frmUpdate_Stud();
            this.Hide();
            frmUpdate_Stud.ShowDialog();
        }
    }
}
