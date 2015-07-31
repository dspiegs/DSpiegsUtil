using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil.Reports
{
    public class ReportErrorArgs : EventArgs
    {
        public ReportErrorArgs(string error)
        {
            Error = error;
        }
        public string Error { get; private set; }
    }

    public interface IReport
    {
        /// <summary>
        /// Geneates HTML tables for each collection in the report.
        /// </summary>        
        /// /// <param name="fullHtml">Set to true to include return a full html document</param>
        /// <returns>HTML of report in a string</returns>
        string GetHTMLTables(bool fullHtml = false);

        event EventHandler<ReportErrorArgs> ErrorThrown;
    }
}
