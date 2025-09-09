using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROEL_PROJ
{
    public class Classes
    {
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
    }
}
