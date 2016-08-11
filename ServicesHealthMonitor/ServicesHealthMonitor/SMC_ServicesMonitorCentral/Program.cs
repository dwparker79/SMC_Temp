using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

using System.Configuration;

namespace SMC_ServicesMonitorCentral
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (!wp.IsInRole(WindowsBuiltInRole.Administrator))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.UseShellExecute = true;
                psi.WorkingDirectory = Environment.CurrentDirectory;
                psi.FileName = Application.ExecutablePath;
                psi.Verb = "runas";
                Process.Start(psi);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new Smc());
        }
    }
}