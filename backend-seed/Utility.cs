using NLog;
using System;
using System.Linq;

namespace backend_seed
{
    /// <summary>
    /// The Utility class is a catch-all for various helper functions and utilities.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Sets the logging level of the LogManager to the specified level, disabling all lower logging levels.
        /// </summary>
        /// <param name="level">The desired logging level.</param>
        public static void SetLoggingLevel(string level)
        {
            try
            {
                // i'm pretty sure this is the first legitimate use case i've seen for a select case with fallthrough.
                switch (level.ToLower())
                {
                    case "fatal":
                        DisableLoggingLevel(LogLevel.Error);
                        goto case "error";
                    case "error":
                        DisableLoggingLevel(LogLevel.Warn);
                        goto case "warn";
                    case "warn":
                        DisableLoggingLevel(LogLevel.Info);
                        goto case "info";
                    case "info":
                        DisableLoggingLevel(LogLevel.Debug);
                        goto case "debug";
                    case "debug":
                        DisableLoggingLevel(LogLevel.Trace);
                        goto case "trace";
                    case "trace":
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown while setting log level: " + ex, ex);
            }

        }

        /// <summary>
        /// Disables the specified logging level witin the LogManager.
        /// </summary>
        /// <param name="level">The level to disable.</param>
        public static void DisableLoggingLevel(LogLevel level)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
                rule.DisableLoggingForLevel(level);

            LogManager.ReconfigExistingLoggers();
        }

        /// <summary>
        /// Returns the specified assembly attribute of the specified assembly.
        /// </summary>
        /// <typeparam name="T">The assembly attribute to return.</typeparam>
        /// <param name="ass">The assembly from which to retrieve the attribute.</param>
        /// <returns>The retrieved attribute.</returns>
        public static T GetAssemblyAttribute<T>(this System.Reflection.Assembly ass) where T : Attribute
        {
            object[] attributes = ass.GetCustomAttributes(typeof(T), false);
            if (attributes == null || attributes.Length == 0)
                return null;
            return attributes.OfType<T>().SingleOrDefault();
        }

        /// <summary>
        /// Retrieves the setting corresponding to the specified setting from the app.exe.config file.
        /// </summary>
        /// <param name="key">The setting to retrieve.</param>
        /// <returns>The string value of the retrieved setting.</returns>
        public static string GetSetting(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Updates the setting corresponding to the specified setting within the app.exe.config file with the specified value.
        /// </summary>
        /// <param name="key">The setting to update.</param>
        /// <param name="value">The value to which the setting should be set.</param>
        public static void UpdateSetting(string key, string value)
        {
            System.Configuration.Configuration configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();

            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Evaluates Environment.OSVersion.Platform to determine the current platform.
        /// </summary>
        /// <returns>"Windows" or "UNIX", depending on the platform on which the app is currently running.</returns>
        public static string GetPlatformName()
        {
            int p = (int)Environment.OSVersion.Platform;
            return ((p == 4) || (p == 6) || (p == 128) ? "UNIX" : "Windows");
        }

        /// <summary>
        /// Uninstalls the application as a Windows Service.
        /// </summary>
        /// <returns>True if the installation succeeded, false otherwise.</returns>
        public static bool ModifyService(string action)
        {
            try
            {
                if (action == "uninstall")
                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/u", System.Reflection.Assembly.GetExecutingAssembly().Location });
                else
                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { System.Reflection.Assembly.GetExecutingAssembly().Location });
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
