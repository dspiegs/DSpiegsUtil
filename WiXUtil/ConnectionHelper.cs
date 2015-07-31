using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WiXUtil
{
    public enum ConnectionTestStatus
    {
        NotRunning,
        Running,
        Succeeded,
        Failed,
        Warning
    }

    public static class ConnectionHelper
    {
        public static ConnectionTestStatus TestWebServiceConnection(string address, string externalEndpoint, out string message)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                message = "The web address is empty";
                return ConnectionTestStatus.Failed;
            }

            UriBuilder builder;
            try
            {
                builder = new UriBuilder(address);
            }
            catch (Exception)
            {
                message = "The web address is invalid";
                return ConnectionTestStatus.Failed;
            }

            if (!Ping(builder.ToString()))
            {
                message = "Unable to connect to web address.";
                return ConnectionTestStatus.Failed;
            }

            if (!string.IsNullOrWhiteSpace(externalEndpoint))
            {
                builder.Path = externalEndpoint;
                if (!Ping(builder.ToString()))
                {
                    message = "The web application is not responding.";
                    return ConnectionTestStatus.Failed;
                }
            }

            message = "Connection was a success!";
            return ConnectionTestStatus.Succeeded;
        }

        private static bool Ping(string url)
        {
            try
            {
                var request = (HttpWebRequest) HttpWebRequest.Create(url);
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                using (request.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException exception)
            {
                var response = (HttpWebResponse) exception.Response;
                if (response == null)
                {
                    return false;
                }

                var statusCude = (int) response.StatusCode;
                return 300 > statusCude || statusCude >= 500;
            }
            catch
            {
                return false;
            }
        }

        public static ConnectionTestStatus TestConnectionString(string connectionString, out string message)
        {
            message = null;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                }
                catch
                {
                    message = "Unable to open database connection.";
                    return ConnectionTestStatus.Failed;
                }

                if (sqlConnection.State != ConnectionState.Open)
                {
                    message = "Unable to open database connection.";
                    return ConnectionTestStatus.Failed;
                }

                message = "Connection was a success!";
                return ConnectionTestStatus.Succeeded;
            }
        }

        public static ConnectionTestStatus TestConnectionStringIntegrated(string connectionString, string domain, string username,
            string password, out string message)
        {
            using (var impersonator = new Impersonator(domain, username, password))
            {
                if (!impersonator.LoggedIn)
                {
                    message = "Unable to impersonate domain user.";
                    return ConnectionTestStatus.Failed;
                }

                return TestConnectionString(connectionString, out message);
            }
        }
    }
}
