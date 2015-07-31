using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WiXUtil
{
    public static class DirectoryPermissions
    {
        // Adds an ACL entry on the specified directory for the specified account. 
        public static void AddDirectorySecurity(string folderName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(folderName);

            // Get a DirectorySecurity object that represents the  
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            var rule = new FileSystemAccessRule(
                account,
                rights,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                controlType);

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(rule);

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);
        }
    }
}
