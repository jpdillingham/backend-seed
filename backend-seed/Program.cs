using NLog;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace backend_seed
{
    public class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the program logic.  This method is responsible for processing any command line arguments,
        /// determining the run mode, and starting the application accordingly.
        /// </summary>
        /// <param name="args">The command line arguments for the program.</param>
        static void Main(string[] args)
        {
            logger.Debug("Program started with " + (args.Length > 0 ? "arguments: " + string.Join(", ", args) : "no arguments."));

            // process command line arguments.
            // valid arguments are:
            //
            //      -logLevel:[trace|debug|info|warn|error|fatal]
            //          sets the global logging level of the program to the specified level.  all logging levels to the left of the
            //          specified level are suppressed.
            //
            //      -[uninstall|install]-service
            //          installs or uninstalls the program as a Windows service
            //
            if (args.Length > 0)
            {
                // check to see if logger arguments were supplied
                string logarg = args.Where(a => Regex.IsMatch(a, "^((?i)-logLevel:)(trace|debug|info|warn|error|fatal)$")).FirstOrDefault();
                if (logarg != default(string))
                {
                    // reconfigure the logger based on the command line arguments.
                    // valid values are "fatal" "error" "warn" "info" "debug" and "trace"
                    // supplying any value will disable logging for any level beneath that level, from left to right as positioned above
                    logger.Debug("Reconfiguring logger to log level '" + logarg.Split(':')[1] + "'...");
                    Utility.SetLoggingLevel(logarg.Split(':')[1]);
                }

                // check to see if service install/uninstall arguments were supplied
                string servicearg = args.Where(a => Regex.IsMatch(a, "^(?i)(-(un)?install-service)$")).FirstOrDefault();
                if (servicearg != default(string))
                {
                    string action = servicearg.Split('-')[1];
                    logger.Info("Attempting to " + action + " Windows Service...");

                    if (Utility.ModifyService(action))
                        logger.Info("Successfully " + action + "ed Windows Service.");
                    else
                        logger.Error("Failed to " + action + " Windows Service.");

                    // if we do anything with the service, do it then quit.  don't start the application if either argument was used.
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadLine();
                    return;
                }
            }

            // determine the run mode and start the application
            // if the program is runing on Windows and Environment.UserInteractive is false, it has been started as a Windows service.
            // invoke the service process which in turn invokes Start()
            if ((Utility.GetPlatformName() == "Windows") && (!Environment.UserInteractive))
            {
                logger.Info("Starting the application in service mode...");
                ServiceBase.Run(new Service());
            }
            // the program is being run as a console application on either Windows or UNIX, so just invoke Start() directly.
            else
            {
                logger.Info("Starting the application in interactive mode...");
                Start(args);
                Stop();
            }
        }

        /// <summary>
        /// The main entry point for the program's application logic.
        /// </summary>
        /// <param name="args">The command line arguments for the program, passed from Main()</param>
        public static void Start(string[] args)
        {
            try
            {
                // TODO: implement application logic here

                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal exception: ");

                // if the application is running as a Windows service, re-throw the error so that the 
                // service process will encounter it.  otherwise, the service will not stop on a fatal error.
                if ((Utility.GetPlatformName() == "Windows") && (!Environment.UserInteractive)) throw;
            }

        }

        public static void Stop()
        {
            // TODO: implement shutdown logic here

            logger.Info("Shutting down...");
        }
    }
}
