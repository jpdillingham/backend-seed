using System.ServiceProcess;

namespace backend_seed
{
    /// <summary>
    /// This class allows the program to run as a Windows service by extending the ServiceBase class.
    /// </summary>
    partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is called by the Windows service manager to start the service.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // set the working directory for the application to the location of the executable.
            // if this is not set here, the application believes it is running from %windir%\system32\.
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            Program.Start(args);
        }

        /// <summary>
        /// This method is called by the Windows service manager to stop the service.
        /// </summary>
        protected override void OnStop()
        {
            Program.Stop();
        }
    }
}
