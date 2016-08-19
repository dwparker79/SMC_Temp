using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using WebApplication1.ServiceLoggerReference;

namespace WebApplication1
{
    public partial class _Default : Page
    {
        protected static int X, Y;
        protected static string logText = "";
        protected static int pageNum = 1, totalPages = 0;

        private ServiceLoggerClient slc = new ServiceLoggerClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            btnReadLogsFirst.Enabled = btnReadLogsPrevious.Enabled = pageNum > 1;
            btnReadLogsLast.Enabled = btnReadLogsNext.Enabled = pageNum < totalPages;
            btnReadLogsFirst.Visible = btnReadLogsPrevious.Visible
                                     = btnReadLogsNext.Visible
                                     = btnReadLogsLast.Visible
                                     = btnReadLogsGoto.Visible
                                     = txtReadLogsGoto.Visible
                                     = totalPages > 0;
        }

        protected void btnReadLogsFirst_Click(object sender, EventArgs e)
        {
            pageNum = 1;
            LoadLogTable(slc.GetLogPage(pageNum, 20, logText.Split(',')));
            lblReadLogsPage.Text = "Page " + pageNum + " of " + totalPages;
            btnReadLogsFirst.Enabled = btnReadLogsPrevious.Enabled = false;
        }

        protected void btnReadLogsPrevious_Click(object sender, EventArgs e)
        {
            pageNum--;
            LoadLogTable(slc.GetLogPage(pageNum, 20, logText.Split(',')));
            lblReadLogsPage.Text = "Page " + pageNum + " of " + totalPages;
            btnReadLogsFirst.Enabled = btnReadLogsPrevious.Enabled = pageNum > 1;
        }

        protected void btnReadLogsNext_Click(object sender, EventArgs e)
        {
            pageNum++;
            LoadLogTable(slc.GetLogPage(pageNum, 20, logText.Split(',')));
            lblReadLogsPage.Text = "Page " + pageNum + " of " + totalPages;
            btnReadLogsLast.Enabled = btnReadLogsNext.Enabled = pageNum < totalPages;
        }

        protected void btnReadLogsLast_Click(object sender, EventArgs e)
        {
            pageNum = totalPages;
            LoadLogTable(slc.GetLogPage(pageNum, 20, logText.Split(',')));
            lblReadLogsPage.Text = "Page " + pageNum + " of " + totalPages;
            btnReadLogsLast.Enabled = btnReadLogsNext.Enabled = false;
        }

        protected void btnReadLogsGoto_Click(object sender, EventArgs e)
        {
            int tempPageNum = pageNum;
            try
            {
                pageNum = int.Parse(txtReadLogsGoto.Text);
                if (pageNum < 1 || pageNum > totalPages)
                    throw new Exception();
            }
            catch (Exception)
            {
                lblReadError.Text = "Error: page number is invalid.";
                lblReadError.Visible = true;
                pageNum = tempPageNum;
                return;
            }

            LoadLogTable(slc.GetLogPage(pageNum, 20, logText.Split(',')));
            lblReadLogsPage.Text = "Page " + pageNum + " of " + totalPages;
            btnReadLogsLast.Enabled = btnReadLogsNext.Enabled = pageNum < totalPages;
            btnReadLogsFirst.Enabled = btnReadLogsPrevious.Enabled = pageNum > 1;
        }

        protected void txtLogFilter_TextChanged(object sender, EventArgs e)
        {
            logText = txtLogFilter.Text;
            totalPages = slc.GetNumPages(20, logText.Split(','));
            if (pageNum > totalPages)
                pageNum = totalPages;
            LoadLogTable(slc.GetLogPage(pageNum, 20, logText.Split(',')));

            lblReadLogsPage.Text = "Page " + pageNum + " of " + totalPages;
            btnReadLogsFirst.Enabled = btnReadLogsPrevious.Enabled = pageNum > 1;
            btnReadLogsLast.Enabled = btnReadLogsNext.Enabled = pageNum < totalPages;
        }

        protected void btnread_Click(object sender, EventArgs e)
        {
            X = 100;
            Y = 1000;
            
            try
            {
                logText = txtLogFilter.Text;
                string[][] logs = slc.GetLogs(logText.Split(','), Environment.MachineName, 1, 20);
                lblReadError.Visible = false;
                LoadLogTable(logs);
                pageNum = 1;
                totalPages = slc.GetNumPages(20, logText.Split(','));

                lblReadLogsPage.Text = "Page " + pageNum + " of " + totalPages;

                btnReadLogsFirst.Enabled = btnReadLogsPrevious.Enabled = pageNum > 1;
                btnReadLogsLast.Enabled = btnReadLogsNext.Enabled = pageNum < totalPages;

                btnReadLogsFirst.Visible = btnReadLogsPrevious.Visible
                                         = btnReadLogsNext.Visible
                                         = btnReadLogsLast.Visible
                                         = lblReadLogsPage.Visible
                                         = btnReadLogsGoto.Visible
                                         = txtReadLogsGoto.Visible
                                         = totalPages > 0;
            }
            catch (Exception ex)
            {
                lblReadError.Text = "Error: " + ex.Message;
                lblReadError.Visible = true;
                pageNum = totalPages = 0;
            }
        }

        protected void LoadLogTable(string[][] logs)
        {
            tblReadLogs.Rows.Clear();
            if (!tblReadLogs.Visible)
                tblReadLogs.Visible = true;

            foreach (string[] log in logs)
            {
                TableRow newRow = new TableRow();
                newRow.BorderColor = System.Drawing.Color.Black;
                newRow.BorderStyle = BorderStyle.Solid;
                newRow.BorderWidth = 1;

                #region [ set cell values ]

                // date
                TableCell newCell = new TableCell();
                newCell.BorderColor = System.Drawing.Color.LightGray;
                newCell.BorderStyle = BorderStyle.Solid;
                newCell.BorderWidth = 1;
                newCell.Text = log[0];
                newRow.Cells.Add(newCell);

                // service name
                newCell = new TableCell();
                newCell.BorderColor = System.Drawing.Color.LightGray;
                newCell.BorderStyle = BorderStyle.Solid;
                newCell.BorderWidth = 1;
                newCell.Text = log[1];
                newRow.Cells.Add(newCell);

                // machine name
                newCell = new TableCell();
                newCell.BorderColor = System.Drawing.Color.LightGray;
                newCell.BorderStyle = BorderStyle.Solid;
                newCell.BorderWidth = 1;
                newCell.Text = log[2];
                newRow.Cells.Add(newCell);

                // user name
                newCell = new TableCell();
                newCell.BorderColor = System.Drawing.Color.LightGray;
                newCell.BorderStyle = BorderStyle.Solid;
                newCell.BorderWidth = 1;
                newCell.Text = log[3];
                newRow.Cells.Add(newCell);

                // category
                newCell = new TableCell();
                newCell.BorderColor = System.Drawing.Color.LightGray;
                newCell.BorderStyle = BorderStyle.Solid;
                newCell.BorderWidth = 1;
                newCell.Text = log[4];
                newRow.Cells.Add(newCell);

                // message
                newCell = new TableCell();
                newCell.BorderColor = System.Drawing.Color.LightGray;
                newCell.BorderStyle = BorderStyle.Solid;
                newCell.BorderWidth = 1;
                newCell.Text = log[5];
                newRow.Cells.Add(newCell);

                #endregion

                tblReadLogs.Rows.Add(newRow);
            }
        }
    }
}