using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WiXUtil
{
    public class Impersonator : IDisposable
    {
        //http://stackoverflow.com/questions/5023607/how-to-use-logonuser-properly-to-impersonate-domain-user-from-workgroup-client

        private WindowsImpersonationContext impersonationContext;
        private IntPtr userHandle = IntPtr.Zero;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_INTERACTIVE = 2;

        public bool LoggedIn { get; private set; }

        // obtains user token
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        // closes open handes returned by LogonUser
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);

        public Impersonator(string domain, string username, string password)
        {
            try
            {
                LoggedIn = LogonUser(username, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref userHandle);
                if (!LoggedIn)
                {
                    Debug.Write(string.Format("Login error code was \"{0}\"", Marshal.GetLastWin32Error()));
                    return;
                }

                // Begin impersonating the user
                impersonationContext = WindowsIdentity.Impersonate(userHandle);
            }
            catch
            {
                LoggedIn = false;
            }
        }

        public void Dispose()
        {
            if (impersonationContext != null)
            {
                impersonationContext.Undo();
            }

            if (userHandle != IntPtr.Zero)
            {
                CloseHandle(userHandle);
            }

            LoggedIn = false;
        }
    }
}
