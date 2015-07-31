using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace DSpiegsUtil.Reports
{
    public static class HTMLReporting
    {
        public const string HTMLHeader = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">";
        public const string LineBreak = @"<br />";
        public const string Tab = @"&emsp;";

        public static string ToHtmlTable<T>(this IEnumerable<T> items, string header, int maxRowsDisplayed = 0, bool includeAncestorProperties = false)
            where T : class
        {
            if (items == null)
            {
                return String.Empty;
            }

            var rows = items as IList<T> ?? items.ToList();
            if (!rows.Any())
            {
                return String.Empty;
            }

            var type = typeof (T);
            if (type == typeof (object))
            {
                type = rows[0].GetType();
            }

            var name = StringUtility.HtmlEncode(header.Replace(Environment.NewLine, " ").Replace("\t", " "));

            var properties =
                (includeAncestorProperties ? type.GetProperties() : type.GetProperties().Where(prop => prop.DeclaringType == type)).ToList();

            var sb = new StringBuilder();
            int numRowsDisplayed;
            if (maxRowsDisplayed > 0 && maxRowsDisplayed < rows.Count)
            {
                numRowsDisplayed = maxRowsDisplayed;
                header += String.Format(" (Only {0} of {1} rows displayed)", numRowsDisplayed, rows.Count);
            }
            else
            {
                numRowsDisplayed = rows.Count;
            }

            sb.AppendLine(StartTable(name, rows.Count, properties.Count, header));

            sb.AppendLine("<tr>");
            foreach (var prop in properties)
            {
                sb.AppendLine(String.Format("<td style=\"font-weight: bold;\">{0}</td>", StringUtility.HtmlEncode(prop.Name.Replace("_", " "))));
            }
            sb.AppendLine("</tr>");

            foreach (var item in rows.Take(numRowsDisplayed))
            {
                sb.AppendLine("<tr>");
                foreach (var property in properties)
                {
                    var val = StringUtility.HtmlEncode(property.GetValue(item, null));
                    val = (val == String.Empty ? "&nbsp;" : val.Replace(Environment.NewLine, LineBreak).Replace("\t", Tab));
                    sb.AppendLine(String.Format("<td>{0}</td>", val));
                }
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine(LineBreak);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="header"></param>
        /// <param name="columnInfos">Raw Name of column must match a property name in "T"</param>
        /// <param name="maxRowsDisplayed"></param>
        /// <returns></returns>
        public static string ToHtmlTable<T>(this IEnumerable<T> items, string header, IEnumerable<ColumnInfo> columnInfos, int maxRowsDisplayed = 0)
            where T : class
        {
            if (items == null)
            {
                return String.Empty;
            }

            var columns = columnInfos.OrderBy(x => x.SortOrder).ToList();

            var rows = items as IList<T> ?? items.ToList();
            if (!rows.Any() || !columns.Any())
            {
                return String.Empty;
            }

            var html = new StringBuilder();

            var tableName = StringUtility.HtmlEncode(header.Replace(Environment.NewLine, " ").Replace("\t", " "));

            var exploredTypes = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

            int numRowsDisplayed;
            if (maxRowsDisplayed > 0 && maxRowsDisplayed < rows.Count)
            {
                numRowsDisplayed = maxRowsDisplayed;
                header += String.Format(" (Only {0} of {1} rows displayed)", numRowsDisplayed, rows.Count);
            }
            else
            {
                numRowsDisplayed = rows.Count;
            }

            html.AppendLine(StartTable(tableName, rows.Count, columns.Count, header));

            html.AppendLine("<tr>");
            foreach (var column in columns)
            {
                html.AppendLine(String.Format("<td style=\"font-weight: bold;\">{0}</td>", StringUtility.HtmlEncode(column.PresentationColumnName)));
            }
            html.AppendLine("</tr>");

            foreach (var row in rows.Take(numRowsDisplayed))
            {
                Dictionary<string, PropertyInfo> properties;
                var type = row.GetType();
                if (!exploredTypes.ContainsKey(type))
                {
                    properties = new Dictionary<string, PropertyInfo>();
                    foreach (var property in type.GetProperties())
                    {
                        var column = columns.FirstOrDefault(x => x.RawColumnName == property.Name);
                        if (column != null)
                        {
                            properties.Add(column.RawColumnName, property);
                        }
                    }
                    exploredTypes.Add(type, properties);
                }
                else
                {
                    properties = exploredTypes[type];
                }

                html.AppendLine("<tr>");
                foreach (var column in columns)
                {
                    var color = Color.White;
                    var val = String.Empty;
                    if (properties.ContainsKey(column.RawColumnName))
                    {
                        var rawVal = properties[column.RawColumnName].GetValue(row, null);
                        val = StringUtility.HtmlEncode(rawVal);
                        if (column.TypeParameter != TypeParameter.None)
                        {
                            color = column.GetColor(column.TypeParameter == TypeParameter.Object ? row : rawVal);
                        }
                    }
                    val = (val == String.Empty ? "&nbsp;" : val.Replace(Environment.NewLine, LineBreak).Replace("\t", Tab));
                    html.AppendLine(String.Format("<td bgcolor=\"{0}\">{1}</td>", color.Name, val));
                }
                html.AppendLine("</tr>");
            }
            html.AppendLine("</table>");
            html.AppendLine(LineBreak);
            return html.ToString();
        }

        private static string StartTable(string tableName, int rowsCount, int columnCount, string header)
        {
            return
                String.Format(
                    "<table border=\"1\" table-name=\"{0}\" table-rows=\"{1}\" cellpadding=\"2\" cellspacing=\"0\" style=\"font-family: Arial; font-size: 0.7em\">{2}<tr><td colspan=\"{3}\" style=\"color: blue; font-weight: bold;\">{4}</td></tr>",
                    tableName, rowsCount, Environment.NewLine, columnCount,
                    StringUtility.HtmlEncode(header).Replace(Environment.NewLine, LineBreak).Replace("\t", Tab));
        }

        public static string AddHTMLAndBodyTags(this string htmlString)
        {
            var html = new StringBuilder();
            html.AppendLine(HTMLHeader);
            html.AppendLine("<html>");
            html.AppendLine("<body style=\"background-color: rgb(255,255,255); font-size: 100%\">");
            html.AppendLine(htmlString);
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        public static string BuildHTMLReport<T>(IEnumerable<T> collection, string title, IEnumerable<ColumnInfo> columnInfos = null,
            bool includeAncestorProperties = false, int maxRows = 0)
            where T : class
        {
            string result = columnInfos == null
                ? collection.ToHtmlTable(title, maxRows, includeAncestorProperties)
                : collection.ToHtmlTable(title, columnInfos, maxRows);
            return result.AddHTMLAndBodyTags();
        }
    }
}
