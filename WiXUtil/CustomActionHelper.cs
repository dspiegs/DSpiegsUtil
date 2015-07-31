using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.Deployment.WindowsInstaller;

namespace WiXUtil
{
    public static class CustomActionHelper
    {
        private static readonly int[] goodExitCodes = { 0, 3010, 3011 };
        public static bool SuccessfulExit(this Process process)
        {
            return goodExitCodes.Contains(process.ExitCode);
        }

        public const string ScheduledTasksExecutable = "SCHTASKS.EXE";

        #region ExtensionMethods
        public static void SendError(this Session session, string message)
        {
            var record = new Record();
            record.SetString(0, message);
            session.Message(InstallMessage.Error, record);
        }
        public static void SendMessage(this Session session, string message)
        {
            var record = new Record();
            record.SetString(0, message);
            session.Message(InstallMessage.Warning, record);
        }

        public static void SetAppSetting(this Configuration config, string key, string value)
        {
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
        }

        public static Configuration OpenWebConfigFile(string configPath)
        {
            var configFile = new FileInfo(configPath);
            var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
        }

        public static void GrantPermissions(this Session session, string applicationFolder)
        {
            try
            {
                DirectoryPermissions.AddDirectorySecurity(applicationFolder, "Everyone", FileSystemRights.FullControl, AccessControlType.Allow);
            }
            catch (Exception ex)
            {
                var error = String.Format("Unable to grant folder access to \"{0}\"", applicationFolder);
                session.Log(error);
                session.Log(ex.ToString());
                session.SendError(error);
            }
        }

        public static void GetEndpoints(this Session session, string address, bool useInt, bool useExt, string internalPath, string externalPath,
            out string internalURI, out string externalURI)
        {
            internalURI = String.Empty;
            externalURI = String.Empty;

            if (!useExt && !useInt)
            {
                session.SendError("No endpoints were set.");
            }
            else
            {
                try
                {
                    var uriBuilder = new UriBuilder(address);
                    if (useExt)
                    {
                        uriBuilder.Path = externalPath;
                        externalURI = uriBuilder.ToString();
                    }

                    if (useInt)
                    {
                        uriBuilder.Path = internalPath;
                        internalURI = uriBuilder.ToString();
                    }
                }
                catch
                {
                    session.SendError("The web service address was invalid.");
                }
            }
        }

        public static string EnsureWebConfigExists(this Session session, string sitePath)
        {
            var configPath = Path.Combine(sitePath, "Web.config");
            if (File.Exists(configPath))
            {
                File.Copy(configPath, configPath + ".old", true);
                return configPath;
            }

            session.Log("No config file. Creating one.");
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<configuration>");
            sb.AppendLine("</configuration>");
            File.WriteAllText(configPath, sb.ToString());
            return configPath;
        }

        #endregion

        public static bool TryRunCommand(string executable, string args, bool useShellExecute = false)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = executable;
                    if (!String.IsNullOrWhiteSpace(args))
                    {
                        process.StartInfo.Arguments = args;
                    }
                    process.StartInfo.UseShellExecute = useShellExecute;
                    process.Start();
                    process.WaitForExit();
                    if (!process.SuccessfulExit())
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Runs a program in a hidden window.
        /// </summary>
        /// <param name="executable">Path to program you want to run</param>
        /// <param name="args">Arguments for the application</param>
        /// <param name="session">Include a session if you want the output logged</param>
        /// <returns></returns>
        public static bool TryRunCommandHidden(string executable, string args, Session session = null)
        {
            var output = new StringBuilder();
            var error = new StringBuilder();
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = executable;
                    if (!String.IsNullOrWhiteSpace(args))
                    {
                        process.StartInfo.Arguments = args;
                    }
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.OutputDataReceived += (sender, e) => output.Append(e.Data);
                    process.ErrorDataReceived += (sender, e) => error.Append(e.Data);
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    if (!process.SuccessfulExit())
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                var standardOutput = output.ToString();
                var errorOutput = error.ToString();
                if (session != null)
                {
                    if (!String.IsNullOrWhiteSpace(standardOutput))
                    {
                        session.Log("Output:");
                        session.Log(standardOutput);
                    }

                    if (!String.IsNullOrWhiteSpace(errorOutput))
                    {
                        session.Log("Errors:");
                        session.Log(errorOutput);
                    }
                }
            }
        }
        public static bool TryRunDISM(Session session, string windowsFolder, params string[] featureNames)
        {
            try
            {
                var args = String.Format("/NoRestart /Online /Enable-Feature {0}{1}",
                    InstallerHelper.IsWin8OrNewer ? "/all " : String.Empty,
                    String.Join(" ", featureNames.Where(x => !String.IsNullOrWhiteSpace(x)).Select(name => String.Format("/FeatureName:\"{0}\"", name))));

                var filePath = Path.Combine(windowsFolder, "SysNative", "dism.exe");

                session.Log("Running DISM:");
                session.Log(filePath);
                session.Log(args);

                return TryRunCommand(filePath, args);
            }
            catch
            {
                return false;
            }
        }
    }
}
