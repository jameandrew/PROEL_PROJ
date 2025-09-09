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
    public partial class frmUpdate_Stud : Form
    {
        private Classes classes;
        public frmUpdate_Stud()
        {
            InitializeComponent();
            classes = new Classes();
        }

        string connectionString = Classes.ConString();

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            frmDashboard dashboard = new frmDashboard();
            this.Hide();
            dashboard.ShowDialog();
        }

        private void dgvUpdate_Stud_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvUpdate_Stud.IsCurrentCellDirty)
            {
                dgvUpdate_Stud.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvUpdate_Stud_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvUpdate_Stud.Columns[e.ColumnIndex].Name == "cmbStatus")
            {
                int profileId = Convert.ToInt32(dgvUpdate_Stud.Rows[e.RowIndex].Cells["ProfileID"].Value);
                string newStatus = dgvUpdate_Stud.Rows[e.RowIndex].Cells["cmbStatus"].Value.ToString();

                classes.UpdateStatus(profileId, newStatus);
                classes.RefreshDB(dgvUpdate_Stud);
            }
        }

        private void frmUpdate_Stud_Load(object sender, EventArgs e)
        {
            classes.LoadData(connectionString,dgvUpdate_Stud);
        }
    }
}
