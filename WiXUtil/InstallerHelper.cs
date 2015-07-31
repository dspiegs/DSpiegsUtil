using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiXUtil
{

    //these methods are for logic that is shared between the custom actions and the centralized installer
    public static class InstallerHelper
    {
        public const string URLPlaceholder = "insert-url-here";

        #region Properties
        public static bool IsWin8OrNewer
        {
            get
            {
                return (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 2)
                       || Environment.OSVersion.Version.Major > 6;
            }
        }

        public static bool IsNotXP8Or2012
        {
            get { return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor < 2; }
        }

        public static bool IsXP
        {
            get { return Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 1; }
        }
        #endregion

        public static bool TryCreateAbsoluteURI(string address, out Uri absoluteUri)
        {
            if (!Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out absoluteUri))
            {
                return false;
            }

            if (absoluteUri.IsAbsoluteUri)
            {
                return true;
            }

            address = "http://" + absoluteUri;

            if (!Uri.TryCreate(address, UriKind.Absolute, out absoluteUri))
            {
                return false;
            }

            if (absoluteUri.Port != 80 || address.EndsWith(":80"))
            {
                return false;
            }

            return true;
        }

        public static bool TrySplitDomainAndUsername(string usernameWithDomain, out string domain, out string username)
        {
            domain = null;
            username = null;

            if (String.IsNullOrWhiteSpace(usernameWithDomain))
            {
                return false;
            }

            var userAndDomain = usernameWithDomain.Split(new[] { '\\' }, 2);
            if (userAndDomain.Length != 2)
            {
                return false;
            }

            domain = userAndDomain[0];
            username = userAndDomain[1];

            return true;
        }

        public static long[] SplitLongs(string commaSplitString)
        {
            long result;
            return
                commaSplitString.Split(',')
                    .Select(x => new { islong = Int64.TryParse(x, out result), result })
                    .Where(x => x.islong)
                    .Select(x => x.result)
                    .Distinct()
                    .ToArray();
        }
    }
}
