using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Configuration;
using System.Resources;
using System.Reflection;
using System.Diagnostics;

namespace SMC_ServicesMonitorCentral
{
    public partial class Smc : Form
    {

        #region [ Declarations ]

        ServiceController[] services = null;
        ServiceController[] monitoredServices = new ServiceController[10];

        //ComponentResourceManager rm = new System.ComponentModel.ComponentResourceManager(typeof(Smc));

        List<KeyValuePair<ServiceController, bool>> monitoredServs = new List<KeyValuePair<ServiceController, bool>>(10);

        Bitmap started = null;
        Bitmap stopped = null;
        Bitmap pending = null;
        Bitmap tick = null;
        Bitmap cross = null;

        Timer timer1 = new Timer();
        Timer timer2 = new Timer();


        WebExtreme.Configuration.ConfigAdapter conf = new WebExtreme.Configuration.ConfigAdapter();
        WebExtreme.Logging.Logger log = new WebExtreme.Logging.Logger();
        WebExtreme.Security.Encryption enc = new WebExtreme.Security.Encryption();
        
        string machineName = "", serviceName = "";
        int index = 0;

        bool exists = false;
        bool isInitializing = true;
        bool auto = false;

        string user, pass;

        #endregion 

        

        #region [ Constructor ]
        public Smc()
        {
            try
            {
                InitializeComponent();

                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Interval = Convert.ToInt32(ConfigurationSettings.AppSettings["timerInterval"]);

                timer2.Tick += new EventHandler(timer2_Tick);
                timer2.Interval = 10000;

                //started = (Bitmap)rm.GetObject("running");
                started = new Bitmap("Resources/running.ico");
                stopped = new Bitmap("Resources/stopped.ico");
                pending = new Bitmap("Resources/pending.ico");
                tick = new Bitmap("Resources/tick.ico");
                cross = new Bitmap("Resources/cross.ico");

                auto = Convert.ToBoolean(ConfigurationSettings.AppSettings["autoRestartMonitoredServices"]);
            }
            catch (Exception ex)
            {
                Log("Smc() [Constructor] ", ex);
            }
        }
       
        #endregion

        #region [ Form_Load ]
        private void Smc_Load(object sender, EventArgs e)
        {
            try
            {

                textBoxUser.Text = ConfigurationSettings.AppSettings["user"];
                textBoxPass.Text = enc.DecodePasswordFromConfigurationFile();
                    
               


                #region [ get and set monitored services from configuration ]

                for (int i = 0; i < 10; i++)
                {
                    if (ConfigurationSettings.AppSettings["service" + i.ToString() + "Name"].Trim().Length > 0 && ConfigurationSettings.AppSettings["service" + i.ToString() + "Machine"].Trim().Length > 0)
                    {
                        monitoredServs.Add(
                            new KeyValuePair<ServiceController, bool>(
                            new ServiceController(
                            ConfigurationSettings.AppSettings["service" + i.ToString() + "Name"],
                            ConfigurationSettings.AppSettings["service" + i.ToString() + "Machine"].ToUpper()),
                            Convert.ToBoolean(ConfigurationSettings.AppSettings["service" + i.ToString() + "mail"].Trim().Length == 0 ? "false" : ConfigurationSettings.AppSettings["service" + i.ToString() + "mail"])));
                    }
                }

                #endregion 

                #region [ get and set interface settings from configuration file ]
                textBoxMachineName.Text = ConfigurationSettings.AppSettings["machineName"].ToUpper();
                textBoxServiceName.Text = ConfigurationSettings.AppSettings["serviceName"];
                this.Height = Convert.ToInt32(ConfigurationSettings.AppSettings["formHeight"]);
                this.Width = Convert.ToInt32(ConfigurationSettings.AppSettings["formWidth"]);
                checkBoxTimerEnable.Checked = Convert.ToBoolean(ConfigurationSettings.AppSettings["doRefresh"]);
                isInitializing = false;
                #endregion


                machineName = textBoxMachineName.Text.ToUpper();
                serviceName = textBoxServiceName.Text;
               
                timer2.Start();

                FillGrid2();



                if (auto)
                {
                    buttonAuto.BackColor = Color.Chartreuse;                  
                }
                else
                {
                    buttonAuto.BackColor = Color.Red;                    
                }
                
                
            }
            catch (Exception ex)
            {
                Log("Smc_Load", ex);
            }

        }
        #endregion 




        #region [ timer1_Tick ]
        void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (textBoxMachineName.Text.Trim().Length == 0)
                {
                    textBoxMachineName.Text = System.Environment.MachineName;
                }                
                new System.Threading.Thread(ListServicesAsync).Start();
                                                
            }
            catch (Exception ex)
            {
                Log("timer1_Tick", ex);
            }
        }
        #endregion 
        
        #region [ timer2_Tick ]
        
        void timer2_Tick(object sender, EventArgs e)
        {
            try
            {                                
                
                foreach (KeyValuePair<ServiceController, bool> kv in monitoredServs)
                {
                    exists = false;
                    if (kv.Key != null)
                    {
                        kv.Key.Refresh();

                        // we check if the service has already been marked as stoped, then we can't and don't want to add it again
                        foreach (DataGridViewRow row in dataGridView3.Rows)
                        {
                            if (row.Cells[0].Value.ToString() == kv.Key.ServiceName && row.Cells[1].Value.ToString() == kv.Key.MachineName)
                            {
                                exists = true;
                            }
                        }
                        // if the service is stopped and hasn't been added still we added to the GridView3
                        if (!exists)
                        {                            
                            if (kv.Key.Status == ServiceControllerStatus.Stopped)
                            {
                                FillGrid3(kv);
                            }                          
                        }
                    }
                }

                // we check if the service has restarted and if its status is different from stopped, then we take it out 
                // of the list with stopped services and remove it as a row from the GridView3 control displaying them
                foreach (DataGridViewRow row in dataGridView3.Rows)
                {
                    ServiceController sc = new ServiceController(row.Cells[0].Value.ToString(),row.Cells[1].Value.ToString());
                    sc.Refresh();
                    if (sc.Status != ServiceControllerStatus.Stopped)
                    {
                        dataGridView3.Rows.RemoveAt(row.Index);
                    }
                }
                
                new System.Threading.Thread(FillGrid2).Start();
                
              
            }
            catch (Exception ex)
            {
                Log("timer2_Tick", ex);
            }
        }
        #endregion 



        #region [ FillGrid2 ]
        private void FillGrid2()
        {
            try
            {
                CheckForIllegalCrossThreadCalls = false;

                dataGridView2.Rows.Clear();

                foreach (KeyValuePair<ServiceController, bool> kv in monitoredServs)
                {
                    if (kv.Key != null)
                    {
                        DataGridViewRow row = new DataGridViewRow();

                        #region [ declare cells ]

                        DataGridViewCell cell0 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell2 = new DataGridViewImageCell();
                        DataGridViewCell cell3 = new DataGridViewImageCell(false);
                        #endregion

                        #region [ set cell values ]

                        cell0.Value = kv.Key.ServiceName;
                        cell1.Value = kv.Key.MachineName;

                        if (kv.Key.Status == ServiceControllerStatus.Running)
                        {
                            cell2.Value = started;
                        }
                        else if (kv.Key.Status == ServiceControllerStatus.Stopped)
                        {
                            cell2.Value = stopped;
                        }
                        else if (kv.Key.Status == ServiceControllerStatus.StartPending)
                        {
                            cell2.Value = pending;
                        }
                        else if (kv.Key.Status == ServiceControllerStatus.StopPending)
                        {
                            cell2.Value = pending;
                        }

                        if (kv.Value == true)
                        {
                            cell3.Value = tick;
                        }
                        else
                        {
                            cell3.Value = cross;
                        }

                        #endregion

                        #region [ add cells ]

                        row.Cells.Add(cell0);
                        row.Cells.Add(cell1);
                        row.Cells.Add(cell2);
                        row.Cells.Add(cell3);
                        #endregion

                        row.Height = 48;

                        dataGridView2.Rows.Add(row);

                        if (auto && kv.Key.Status == ServiceControllerStatus.Stopped)
                        {
                            kv.Key.Start();
                            Log(kv.Key.DisplayName + " on " + kv.Key.MachineName + " has restarted.", new Exception(kv.Key.DisplayName + " on " + kv.Key.MachineName + " has restarted "+DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()));
                            log.SendErrorHMTLMail(kv.Key.DisplayName + " on " + kv.Key.MachineName + " has restarted.", new Exception(kv.Key.DisplayName + " on " + kv.Key.MachineName + " has restarted " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("FillGrid2", ex);
            }
        }
        #endregion 

        #region [ FillGrid3 ]
        private void FillGrid3(KeyValuePair<ServiceController, bool> sc)
        {
            try
            {
                //dataGridView3.Rows.Clear();
                DataGridViewRow row = new DataGridViewRow();

                #region [ declare cells ]
                DataGridViewCell cell0 = new DataGridViewTextBoxCell();
                DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                DataGridViewCell cell2 = new DataGridViewImageCell(false);
                DataGridViewCell cell3 = new DataGridViewButtonCell();



                #endregion

                #region [ cell styles ]

                DataGridViewCellStyle styleStartButton = new DataGridViewCellStyle();
                styleStartButton.Font = new Font(new FontFamily("Verdana"), 9.0F, FontStyle.Bold);
                styleStartButton.ForeColor = Color.White;
                styleStartButton.BackColor = Color.Green;


                #endregion


                #region [ set cell values ]

                cell0.Value = sc.Key.ServiceName;
                cell1.Value = sc.Key.MachineName;
                cell3.Value = "start";

                if (sc.Key.Status == ServiceControllerStatus.Running)
                {
                    cell2.Value = started;
                }
                else if (sc.Key.Status == ServiceControllerStatus.Stopped)
                {
                    cell2.Value = stopped;
                }
                else if (sc.Key.Status == ServiceControllerStatus.StartPending)
                {
                    cell2.Value = pending;
                }
                else if (sc.Key.Status == ServiceControllerStatus.StopPending)
                {
                    cell2.Value = pending;
                }

                cell3.Style = styleStartButton;

                #endregion

                #region [ add cells ]

                row.Cells.Add(cell0);
                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                row.Cells.Add(cell3);



                #endregion

                row.Height = 48;

                dataGridView3.Rows.Add(row);

                if (sc.Value == true)
                {
                    log.SendErrorHMTLMail(sc.Key.ServiceName + " on " + sc.Key.MachineName + " has stopped excution!", new Exception());
                }
            }
            catch (Exception ex)
            {
                Log("FillGrid3", ex);
            }
        }
        #endregion 



        

        #region [ buttonListServices_Click ]
        private void glassButtonListServices_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBoxTimerEnable.Checked)
                {
                    timer1.Start();
                }

                if (textBoxMachineName.Text.Trim().Length == 0)
                {
                    textBoxMachineName.Text = System.Environment.MachineName;
                }


                ListServices(textBoxMachineName.Text, textBoxServiceName.Text);

                machineName = textBoxMachineName.Text;
                serviceName = textBoxServiceName.Text;

                conf.SaveToAppConfig("machineName", textBoxMachineName.Text);
                conf.SaveToAppConfig("serviceName", textBoxServiceName.Text);
            }
            catch (Exception ex)
            {
                Log("buttonListServices_Click", ex);
            }
        }
        #endregion

        #region [ buttonSaveCredentials_Click ]


        private void glassButtonSaveCredentials_Click(object sender, EventArgs e)
        {
            try
            {
                pass = textBoxPass.Text;
                user = textBoxUser.Text;

                enc.EncryptAndSavePassToConfigurationFile(pass);
                conf.SaveToAppConfig("user", user);

                this.Close();
                WebExtreme.Security.Processes.ExecuteWithCredentials(user, pass, "SMC_ServicesMonitorCentral.exe");
            }
            catch (Exception ex)
            {
                Log("buttonSaveCredentials_Click", ex);
            }
        }
        
        #endregion 

        #region [ buttonAuto_Click ]
        private void buttonAuto_Click(object sender, EventArgs e)
        {
            if (!auto)
            {
                buttonAuto.BackColor = Color.Chartreuse;
                auto = true;
                conf.SaveToAppConfig("autoRestartMonitoredServices", auto.ToString());
            }
            else
            {
                buttonAuto.BackColor = Color.Red;
                auto = false;
                conf.SaveToAppConfig("autoRestartMonitoredServices", auto.ToString());
            }

        }
        #endregion 





        #region [ ListServices ]
        private void ListServices(string machineName, string serviceName)
        {
            try
            {
                dataGridView1.Rows.Clear();
                // we get the services of the specified machine
                services = ServiceController.GetServices(machineName);
               

                foreach (ServiceController sc in services)
                {
                    if (sc.ServiceName.ToLower().Contains(serviceName.ToLower()))
                    {
                        // we declare a new row for each service
                        DataGridViewRow row = new DataGridViewRow();

                        #region [ declare cells ]
                        DataGridViewCell cell0 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell2 = new DataGridViewImageCell(false);
                        DataGridViewCell cell3 = new DataGridViewButtonCell();
                        DataGridViewCell cell4 = new DataGridViewButtonCell();
                   
                        #endregion

                        #region [ cell styles ]
                        DataGridViewCellStyle styleServiceName = new DataGridViewCellStyle();
                        styleServiceName.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleServiceName.ForeColor = Color.Black;

                        DataGridViewCellStyle styleStarted = new DataGridViewCellStyle();
                        styleStarted.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleStarted.ForeColor = Color.Green;

                        DataGridViewCellStyle styleStopped = new DataGridViewCellStyle();
                        styleStopped.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleStopped.ForeColor = Color.Red;

                        DataGridViewCellStyle stylePending = new DataGridViewCellStyle();
                        stylePending.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        stylePending.ForeColor = Color.Yellow;

                        DataGridViewCellStyle styleStartButton = new DataGridViewCellStyle();
                        styleStartButton.Font = new Font(new FontFamily("Verdana"),9.0F, FontStyle.Bold);
                        styleStartButton.ForeColor = Color.White;
                        styleStartButton.BackColor = Color.Green;

                        DataGridViewCellStyle styleStopButton = new DataGridViewCellStyle();
                        styleStopButton.Font = new Font(new FontFamily("Verdana"), 9.0F, FontStyle.Bold);
                        styleStopButton.ForeColor = Color.White;
                        styleStopButton.BackColor = Color.DarkRed;
                        
                        #endregion

                        #region [ set cell values ]
                        

                        cell0.Value = sc.ServiceName;
                        cell0.Style = styleServiceName;

                        cell3.Value = "start";
                        cell3.Style = styleStartButton;

                        cell4.Value = "stop";
                        cell4.Style = styleStopButton;
                       
                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = styleStarted;

                            cell2.Value = started;
                        }
                        else if (sc.Status == ServiceControllerStatus.Stopped)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = styleStopped;

                            cell2.Value = stopped;
                        }
                        else if (sc.Status == ServiceControllerStatus.StartPending)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = stylePending;

                            cell2.Value = pending;
                        }
                        else if (sc.Status == ServiceControllerStatus.StopPending)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = stylePending;

                            cell2.Value = pending;
                        }


                        #endregion

                        #region [ add cells ]

                        row.Cells.Add(cell0);
                        row.Cells.Add(cell1);
                        row.Cells.Add(cell2);
                        row.Cells.Add(cell3);
                        row.Cells.Add(cell4);
                       

                        #endregion

                        row.Height = 48;

                        dataGridView1.Rows.Add(row);

                        

                        this.Text = "Smc Services Monitor Central [" + machineName.ToUpper() + "]";
                    }
                    else if (serviceName.Trim().Length == 0)
                    {
                        DataGridViewRow row = new DataGridViewRow();

                        #region [ declare cells ]
                        DataGridViewCell cell0 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell2 = new DataGridViewImageCell(false);
                        DataGridViewCell cell3 = new DataGridViewButtonCell();
                        DataGridViewCell cell4 = new DataGridViewButtonCell();

                        #endregion

                        #region [ cell styles ]
                        DataGridViewCellStyle styleServiceName = new DataGridViewCellStyle();
                        styleServiceName.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleServiceName.ForeColor = Color.Black;
                        DataGridViewCellStyle styleStarted = new DataGridViewCellStyle();
                        styleStarted.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleStarted.ForeColor = Color.Green;

                        DataGridViewCellStyle styleStopped = new DataGridViewCellStyle();
                        styleStopped.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleStopped.ForeColor = Color.Red;

                        DataGridViewCellStyle stylePending = new DataGridViewCellStyle();
                        stylePending.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        stylePending.ForeColor = Color.Yellow;

                        DataGridViewCellStyle styleStartButton = new DataGridViewCellStyle();
                        styleStartButton.Font = new Font(new FontFamily("Verdana"), 9.0F, FontStyle.Bold);
                        styleStartButton.ForeColor = Color.White;
                        styleStartButton.BackColor = Color.Green;

                        DataGridViewCellStyle styleStopButton = new DataGridViewCellStyle();
                        styleStopButton.Font = new Font(new FontFamily("Verdana"), 9.0F, FontStyle.Bold);
                        styleStopButton.ForeColor = Color.White;
                        styleStopButton.BackColor = Color.DarkRed;

                        #endregion

                        #region [ set cell values ]


                        cell0.Value = sc.ServiceName;
                        cell0.Style = styleServiceName;

                        cell3.Value = "start";
                        cell3.Style = styleStartButton;

                        cell4.Value = "stop";
                        cell4.Style = styleStopButton;

                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = styleStarted;

                            cell2.Value = started;
                        }
                        else if (sc.Status == ServiceControllerStatus.Stopped)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = styleStopped;

                            cell2.Value = stopped;
                        }
                        else if (sc.Status == ServiceControllerStatus.StartPending)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = stylePending;

                            cell2.Value = pending;
                        }
                        else if (sc.Status == ServiceControllerStatus.StopPending)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = stylePending;

                            cell2.Value = pending;
                        }


                        #endregion

                        #region [ add cells ]

                        row.Cells.Add(cell0);
                        row.Cells.Add(cell1);
                        row.Cells.Add(cell2);
                        row.Cells.Add(cell3);
                        row.Cells.Add(cell4);


                        #endregion

                        row.Height = 48;

                        dataGridView1.Rows.Add(row);
                    }
                }                
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
                Log("ListServices(): ", ex);
            }
        }
        #endregion 

        #region [ ListServicesAsync ]
        protected void ListServicesAsync()
        {
            CheckForIllegalCrossThreadCalls = false;

            try
            {
                dataGridView1.Rows.Clear();
                services = ServiceController.GetServices(machineName);
               
                foreach (ServiceController sc in services)
                {
                    if (sc.ServiceName.ToLower().Contains(serviceName.ToLower()))
                    {
                        DataGridViewRow row = new DataGridViewRow();

                        #region [ declare cells ]
                        DataGridViewCell cell0 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell2 = new DataGridViewImageCell(false);
                        DataGridViewCell cell3 = new DataGridViewButtonCell();
                        DataGridViewCell cell4 = new DataGridViewButtonCell();

                        #endregion

                        #region [ cell styles ]
                        DataGridViewCellStyle styleServiceName = new DataGridViewCellStyle();
                        styleServiceName.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleServiceName.ForeColor = Color.Black;

                        DataGridViewCellStyle styleStarted = new DataGridViewCellStyle();
                        styleStarted.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleStarted.ForeColor = Color.Green;

                        DataGridViewCellStyle styleStopped = new DataGridViewCellStyle();
                        styleStopped.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleStopped.ForeColor = Color.Red;

                        DataGridViewCellStyle stylePending = new DataGridViewCellStyle();
                        stylePending.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        stylePending.ForeColor = Color.Yellow;

                        DataGridViewCellStyle styleStartButton = new DataGridViewCellStyle();
                        styleStartButton.Font = new Font(new FontFamily("Verdana"), 9.0F, FontStyle.Bold);
                        styleStartButton.ForeColor = Color.White;
                        styleStartButton.BackColor = Color.Green;

                        DataGridViewCellStyle styleStopButton = new DataGridViewCellStyle();
                        styleStopButton.Font = new Font(new FontFamily("Verdana"), 9.0F, FontStyle.Bold);
                        styleStopButton.ForeColor = Color.White;
                        styleStopButton.BackColor = Color.DarkRed;

                        #endregion

                        #region [ set cell values ]


                        cell0.Value = sc.ServiceName;
                        cell0.Style = styleServiceName;

                        cell3.Value = "start";
                        cell3.Style = styleStartButton;

                        cell4.Value = "stop";
                        cell4.Style = styleStopButton;

                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = styleStarted;

                            cell2.Value = started;
                        }
                        else if (sc.Status == ServiceControllerStatus.Stopped)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = styleStopped;

                            cell2.Value = stopped;
                        }
                        else if (sc.Status == ServiceControllerStatus.StartPending)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = stylePending;

                            cell2.Value = pending;
                        }
                        else if (sc.Status == ServiceControllerStatus.StopPending)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = stylePending;

                            cell2.Value = pending;
                        }


                        #endregion

                        #region [ add cells ]

                        row.Cells.Add(cell0);
                        row.Cells.Add(cell1);
                        row.Cells.Add(cell2);
                        row.Cells.Add(cell3);
                        row.Cells.Add(cell4);


                        #endregion

                        row.Height = 48;

                        dataGridView1.Rows.Add(row);



                        this.Text = "Smc Services Monitor Central [" + machineName.ToUpper() + "]";
                    }
                    else if (serviceName.Trim().Length == 0)
                    {
                        DataGridViewRow row = new DataGridViewRow();

                        #region [ declare cells ]
                        DataGridViewCell cell0 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                        DataGridViewCell cell2 = new DataGridViewImageCell(false);
                        DataGridViewCell cell3 = new DataGridViewButtonCell();
                        DataGridViewCell cell4 = new DataGridViewButtonCell();

                        #endregion

                        #region [ cell styles ]
                        DataGridViewCellStyle styleServiceName = new DataGridViewCellStyle();
                        styleServiceName.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleServiceName.ForeColor = Color.Black;
                        DataGridViewCellStyle styleStarted = new DataGridViewCellStyle();
                        styleStarted.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleStarted.ForeColor = Color.Green;

                        DataGridViewCellStyle styleStopped = new DataGridViewCellStyle();
                        styleStopped.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        styleStopped.ForeColor = Color.Red;

                        DataGridViewCellStyle stylePending = new DataGridViewCellStyle();
                        stylePending.Font = new Font(new FontFamily("Verdana"), 10.0F, FontStyle.Bold);
                        stylePending.ForeColor = Color.Yellow;

                        DataGridViewCellStyle styleStartButton = new DataGridViewCellStyle();
                        styleStartButton.Font = new Font(new FontFamily("Verdana"), 9.0F, FontStyle.Bold);
                        styleStartButton.ForeColor = Color.White;
                        styleStartButton.BackColor = Color.Green;

                        DataGridViewCellStyle styleStopButton = new DataGridViewCellStyle();
                        styleStopButton.Font = new Font(new FontFamily("Verdana"), 9.0F, FontStyle.Bold);
                        styleStopButton.ForeColor = Color.White;
                        styleStopButton.BackColor = Color.DarkRed;

                        #endregion

                        #region [ set cell values ]


                        cell0.Value = sc.ServiceName;
                        cell0.Style = styleServiceName;

                        cell3.Value = "start";
                        cell3.Style = styleStartButton;

                        cell4.Value = "stop";
                        cell4.Style = styleStopButton;

                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = styleStarted;

                            cell2.Value = started;
                        }
                        else if (sc.Status == ServiceControllerStatus.Stopped)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = styleStopped;

                            cell2.Value = stopped;
                        }
                        else if (sc.Status == ServiceControllerStatus.StartPending)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = stylePending;

                            cell2.Value = pending;
                        }
                        else if (sc.Status == ServiceControllerStatus.StopPending)
                        {
                            cell1.Value = sc.Status.ToString();
                            cell1.Style = stylePending;

                            cell2.Value = pending;
                        }


                        #endregion

                        #region [ add cells ]

                        row.Cells.Add(cell0);
                        row.Cells.Add(cell1);
                        row.Cells.Add(cell2);
                        row.Cells.Add(cell3);
                        row.Cells.Add(cell4);


                        #endregion

                        row.Height = 48;

                        dataGridView1.Rows.Add(row);
                    }
                }                
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
                Log("ListServices(): ", ex);
            }
        }
        #endregion




        #region [ dataGrids CellContentClick and CellDoubleClick EventHandlers ]

        #region [ dataGridView1_CellContentClick ]
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3)// start service button
                {
                    ServiceController sc = new ServiceController(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), textBoxMachineName.Text);
                    sc.Start();
                    ListServices(textBoxMachineName.Text, textBoxServiceName.Text);
                }
                else if (e.ColumnIndex == 4)// stop service button
                {
                    ServiceController sc = new ServiceController(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), textBoxMachineName.Text);
                    sc.Stop();
                    ListServices(textBoxMachineName.Text, textBoxServiceName.Text);
                }
               
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
                Log("dataGridView1_CellContentClick(): ", ex);
            }
        }
        #endregion 

        #region [ dataGridView1_CellDoubleClick ]
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // we say we can monitor up to 10 services (for interface issues; else window gets too big)
                if (index < 10)
                {

                    // we check if the service already exists in out list
                    foreach (KeyValuePair<ServiceController, bool> kv in monitoredServs)
                    {
                        if (kv.Key != null)
                        {
                            if (kv.Key.ServiceName == dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString() && kv.Key.MachineName == textBoxMachineName.Text)
                            {
                                toolStripStatusLabel1.Text = "Service already in list!";
                                return;
                            }
                        }
                    }
                    // we create a new ServiceController with the passed information from the clicked row
                    ServiceController s = new ServiceController(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), textBoxMachineName.Text);

                    // we create a new KeyValuePair<ServiceController, bool> with the created ServiceController
                    // and a Boolean variable controling whether we receive an email when the service stops
                    KeyValuePair<ServiceController, bool> kv0 = new KeyValuePair<ServiceController, bool>();

                    // we ask if we want to receive an email when the service stops
                    DialogResult result = MessageBox.Show("Do you want to receive an E-mail notifying you if the monitored service has stopped execution?", "Notification confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        kv0 = new KeyValuePair<ServiceController, bool>(s, true);
                    }
                    else
                    {
                        kv0 = new KeyValuePair<ServiceController, bool>(s, false);
                    }


                    // we add this service to the monitored services list of type KeyValuePair<ServiceController, bool>
                    monitoredServs.Add(kv0);

                    // we fill the second grid
                    FillGrid2();
                    index++;
                }
                else
                {
                    toolStripStatusLabel1.Text = "You can monitor up to 10 services!";
                }
            }
            catch (Exception ex)
            {
                Log("dataGridView1_CellDoubleClick", ex);
            }
        }
        #endregion 


        #region [ dataGridView2_CellContentClick ]
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Process p = new Process();

                //string executable = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\eventvwr.msc");


                p.StartInfo.FileName = "EVENTVIEWER.BAT";
                p.StartInfo.Arguments = Convert.ToString(dataGridView2.Rows[e.RowIndex].Cells[1].Value);
                p.Start();
            }
            catch (Exception ex)
            {
                Log("dataGridView2_CellContentClick", ex);
            }

        }
        #endregion

        #region [ dataGridView2_CellDoubleClick ]
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                monitoredServs.RemoveAt(e.RowIndex);
                dataGridView2.Rows.RemoveAt(e.RowIndex);
            }
            catch (Exception ex)
            {
                Log("dataGridView2_CellDoubleClick", ex);
            }
           
        }
        #endregion


        #region [ dataGridView3_CellContentClick ]
        private void dataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3)// start service button
                {
                    ServiceController sc = new ServiceController(dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString(), dataGridView3.Rows[e.RowIndex].Cells[1].Value.ToString());
                    sc.Start();
                    dataGridView3.Rows.RemoveAt(e.RowIndex);
                }
                else
                {
                    Process p = new Process();

                    //string executable = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\eventvwr.msc");


                    p.StartInfo.FileName = "EVENTVIEWER.BAT";
                    p.StartInfo.Arguments = Convert.ToString(dataGridView3.Rows[e.RowIndex].Cells[1].Value);


                    p.Start(); 
                }
            }
            catch (Exception ex)
            {
                Log("dataGridView3_CellContentClick_1", ex);
                toolStripStatusLabel1.Text = ex.Message;
            }
        }
        #endregion       

        #region [ dataGridView3_CellDoubleClick ]
        private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView3.Rows.RemoveAt(e.RowIndex);
            
        }
        #endregion 

        #endregion 




        #region [ Smc_FormClosing ]
        private void Smc_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    conf.SaveToAppConfig("service" + i.ToString() + "Name", (i >= monitoredServs.Count ? "" : monitoredServs[i].Key.ServiceName));
                    conf.SaveToAppConfig("service" + i.ToString() + "Machine", (i >= monitoredServs.Count ? "" : monitoredServs[i].Key.MachineName.ToUpper()));
                    conf.SaveToAppConfig("service" + i.ToString() + "mail", (i >= monitoredServs.Count ? "" : monitoredServs[i].Value.ToString()));
                }
            }
            catch (Exception ex)
            {
                Log("Smc_FormClosing", ex);
            }
        }
        #endregion 



        #region [ layout related ]

        #region [ Smc_ResizeEnd ]
        private void Smc_ResizeEnd(object sender, EventArgs e)
        {
            try
            {
                conf.SaveToAppConfig("formHeight", this.Height.ToString());
                conf.SaveToAppConfig("formWidth", this.Width.ToString());
            }
            catch (Exception ex)
            {
                Log("Smc_ResizeEnd", ex);
            }

        }
         #endregion 

        #region [ checkBoxTimerEnable_CheckedChanged ]
        private void checkBoxTimerEnable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBoxTimerEnable.Checked && !isInitializing)
                {
                    timer1.Start();
                }
                else if (!checkBoxTimerEnable.Checked)
                {
                    timer1.Stop();
                }
                conf.SaveToAppConfig("true", checkBoxTimerEnable.Checked.ToString());
            }
            catch (Exception ex)
            {
                Log("checkBoxTimerEnable_CheckedChanged", ex);
            }
           
        }
        #endregion
       
        #region [ toolStripStatusLabel1_Click ]
        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = string.Empty;
        }
        #endregion 

        #endregion 

        #region [ Log ]
        private void Log(string methodName, Exception ex)
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationSettings.AppSettings["sendMailOnError"]))
                {
                    log.SendErrorHMTLMail(methodName, ex);
                }
                log.LogToFile(ex);
            }
            catch { }
        }
        #endregion         

    


















    }
}