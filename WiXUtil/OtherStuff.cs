using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;
using DSpiegsUtil;

namespace WiXUtil
{
    public class OtherStuff
    {
        private static void RegisterASPNET(string windowsFolder)
        {
            var path = Path.Combine(
                windowsFolder,
                "Microsoft.NET",
                string.Format("Framework{0}", Environment.Is64BitOperatingSystem ? "64" : string.Empty),
                "v4.0.30319",
                "aspnet_regiis.exe");

            CustomActionHelper.TryRunCommand(path, "-ir");
        }

        [CustomAction]
        public static ActionResult InstallIIS(Session session)
        {
#if DEBUG
            Debugger.Launch();
#endif
            var windowsFolder = session.CustomActionData["WindowsFolder"].TrimOrEmpty();

            //http://stackoverflow.com/questions/16079030/better-way-to-install-iis7-programmatically
            if (!CustomActionHelper.TryRunDISM(session, windowsFolder,
                "IIS-ApplicationDevelopment",
                "IIS-CommonHttpFeatures",
                "IIS-DefaultDocument",
                "IIS-ISAPIExtensions",
                "IIS-ISAPIFilter",
                "IIS-ManagementConsole",
                "IIS-NetFxExtensibility",
                "IIS-RequestFiltering",
                "IIS-Security",
                "IIS-StaticContent",
                "IIS-WebServer",
                "IIS-WebServerRole",
                "IIS-ASPNET",
                InstallerHelper.IsWin8OrNewer ? "IIS-ASPNET45" : string.Empty,
                InstallerHelper.IsWin8OrNewer ? "WCF-HTTP-Activation45" : string.Empty,
                "IIS-CGI",
                "IIS-HttpLogging",
                "IIS-CustomLogging",
                "IIS-HttpTracing",
                "IIS-BasicAuthentication",
                "IIS-WindowsAuthentication",
                "IIS-HttpCompressionDynamic",
                "WCF-HTTP-Activation",
                "WCF-NonHTTP-Activation",
                "NetFx3",
                "WAS-ProcessModel",
                "WAS-WindowsActivationService",
                "WAS-NetFxEnvironment",
                "WAS-ConfigurationAPI"))
            {
                session.SendError("There were errors enabling IIS. Application and App Pool may not be created.");
            }

            if (InstallerHelper.IsNotXP8Or2012)
            {
                RegisterASPNET(windowsFolder);
            }

            return ActionResult.Success;
        }
    }
}
