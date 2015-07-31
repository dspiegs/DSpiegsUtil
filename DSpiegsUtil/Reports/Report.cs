using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil.Reports
{

    public abstract class Report : IReport
    {
        public event EventHandler<ReportErrorArgs> ErrorThrown;

        protected void ThrowError(string error)
        {
            if (ErrorThrown != null)
            {
                ErrorThrown(this, new ReportErrorArgs(error));
            }
        }

        protected const string Path = "Path";
        protected const string RecID = "Rec ID";

        /// <summary>
        /// Gets or sets the Maximum Number of rows to return in reports. 0 means no limit.       
        /// </summary>
        public int MaxRows { get; set; }

        private readonly List<ValidationItem> ValidationItems = new List<ValidationItem>();

        public class ValidationItem
        {
            public IEnumerable<object> Collection { get; set; }
            public string Title { get; set; }
            public IEnumerable<ColumnInfo> ColumnInfos { get; set; }
            public bool IncludeAncestorProperties { get; set; }
        }

        /// <summary>
        /// Must be overridden. Where report is built using AddToReport
        /// </summary>
        protected abstract void BuildReport();

        /// <summary>
        /// Adds collections of items to the report
        /// </summary>
        /// <typeparam name="T">items must inherit from System.Object</typeparam>
        /// <param name="reportItems">The collection of items</param>
        /// <param name="title">The title of the report</param>
        /// <param name="columnInfos">Collection of column mapping information. If null all properties will show.</param>
        /// <param name="includeAncestorProperties">If columns are not supplied, sets if ancestor properties should show up in the report</param>
        protected virtual void AddToReport<T>(IEnumerable<T> reportItems, string title, IEnumerable<ColumnInfo> columnInfos = null,
            bool includeAncestorProperties = false) where T : class
        {
            if (reportItems == null)
            {
                return;
            }

            ValidationItems.Add(new ValidationItem
            {
                Collection = reportItems,
                Title = title,
                ColumnInfos = columnInfos,
                IncludeAncestorProperties = includeAncestorProperties
            });
        }


        /// <summary>
        /// Geneates HTML tables for each collection in the report.
        /// </summary>        
        /// /// <param name="fullHtml">Set to true to include return a full html document</param>
        /// <returns>HTML of report in a string</returns>
        public string GetHTMLTables(bool fullHtml = false)
        {
            BuildReport();

            var sb = new StringBuilder();

            foreach (var item in ValidationItems)
            {
                sb.AppendLineNotEmpty(item.ColumnInfos != null
                    ? item.Collection.ToHtmlTable(item.Title, item.ColumnInfos, MaxRows)
                    : item.Collection.ToHtmlTable(item.Title, MaxRows, item.IncludeAncestorProperties));
            }

            var html = sb.ToString();

            if (fullHtml)
            {
                html = html.AddHTMLAndBodyTags();
            }

            return html;
        }

        /*
            public object GetExcelReport()
            {
                BuildReport();
                return CreateSpreadsheet();
            }
             */
        //TODO one day
    }

    //public static class ExcelReporting{} //todo one day
}
