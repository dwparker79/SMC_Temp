using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SMC_ServicesMonitorCentral
{
    public partial class UserControl2 : System.Windows.Forms.DataGridView
    {
        public UserControl2()
        {
            InitializeComponent();
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch (Exception)
            {
                this.Invalidate();
            }
        }
    }
}
