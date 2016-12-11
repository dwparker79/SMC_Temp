using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Glass;
using WebExtreme;

namespace SMC_ServicesMonitorCentral
{
    public partial class EventViewer : Form
    {
        #region Declarations
        
        private SortedDictionary<DateTime, EventLogEntry> events;

        private Form parent;
        
        private LinkedList<string> serviceNames;

        /// <summary>
        /// A custom date comparer that puts the most recent date first.
        /// </summary>
        private class DateComparer : IComparer<DateTime>
        {
            public int Compare(DateTime x, DateTime y)
            {
                return -(x.CompareTo(y));
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the event viewer with the given machine name and service names.
        /// </summary>
        /// <param name="machineName">The machine to pull events from.</param>
        /// <param name="serviceNames">The services to look up.</param>
        public EventViewer(string machineName, IEnumerable<string> serviceNames, Form parent = null)
        {
            events = new SortedDictionary<DateTime, EventLogEntry>(new DateComparer());
            this.serviceNames = new LinkedList<string>(serviceNames);

            InitializeComponent();
            machine_txtbox.Text = machineName;

            this.parent = parent;
        }
        #endregion

        /// <summary>
        /// Reloads the data grid view of this event viewer with the most recent event logs.
        /// </summary>
        public void LoadEventLog()
        {
            Cursor tempC = Cursor;
            Cursor = Cursors.WaitCursor;

            events.Clear();
            EventLog[] newEvents = EventLog.GetEventLogs(machine_txtbox.Text);
            foreach (EventLog el in newEvents)
            {
                try
                {
                    foreach (EventLogEntry elentry in el.Entries)
                    {
                        try
                        {
                            if (serviceNames.Contains(elentry.Source))
                                events.Add(elentry.TimeGenerated, elentry);
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception) { }
            }

            log_GridView.Rows.Clear();

            foreach (EventLogEntry ele in events.Values)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(log_GridView);

                newRow.Cells[0].Value = ele.TimeGenerated.ToString();
                newRow.Cells[1].Value = ele.Source;
                newRow.Cells[2].Value = ele.Message;

                log_GridView.Rows.Add(newRow);
            }

            if (!Visible)
                Show();
            Cursor = tempC;
        }

        public void AddService(string newService)
        {
            if (!serviceNames.Contains(newService))
            {
                serviceNames.AddLast(newService);
                LoadEventLog();
            }
        }
        public void AddService(IEnumerable<string> newServices)
        {
            bool serviceAdded = false;
            foreach (string name in newServices)
                if (!serviceNames.Contains(name))
                {
                    serviceAdded = true;
                    serviceNames.AddLast(name);
                }
            if (serviceAdded)
                LoadEventLog();
        }
        public void RemoveService(string oldService)
        {
            if (serviceNames.Remove(oldService))
            {
                LinkedList<DataGridViewRow> toRemove = new LinkedList<DataGridViewRow>();
                foreach (DataGridViewRow row in log_GridView.Rows)
                {
                    if ((string)row.Cells["log_service_name"].Value == oldService)
                        toRemove.AddLast(row);
                }
                foreach (DataGridViewRow oldRow in toRemove)
                    log_GridView.Rows.Remove(oldRow);
            }
        }
        public void RemoveService(string[] oldServices)
        {
            foreach (string name in oldServices)
            {
                if (serviceNames.Remove(name))
                {
                    LinkedList<DataGridViewRow> toRemove = new LinkedList<DataGridViewRow>();
                    foreach (DataGridViewRow row in log_GridView.Rows)
                    {
                        if ((string)row.Cells["log_service_name"].Value == name)
                            toRemove.AddLast(row);
                    }
                    foreach (DataGridViewRow oldRow in toRemove)
                        log_GridView.Rows.Remove(oldRow);
                }
            }
        }

        private void EventViewer_Load(object sender, EventArgs e)
        {
            LoadEventLog();
        }

        private void Refresh_btn_Click(object sender, EventArgs e)
        {
            LoadEventLog();
        }

        private void machine_txtbox_TextChanged(object sender, EventArgs e)
        {
            LoadEventLog();
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
