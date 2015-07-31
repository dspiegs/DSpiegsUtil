using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DSpiegsUtil.Reports
{
    public static class ReportComparer
    {

        public static string AssertHtmlCompare(string first, string seconds)
        {
            var sp = new XmlDocument();
            sp.LoadXml(first.Replace(HTMLReporting.HTMLHeader, string.Empty).Replace(@"&nbsp;", string.Empty).Replace(HTMLReporting.Tab, string.Empty));
            var spResults =
                sp.SelectNodes("/html/body/table")
                    .Cast<XmlNode>()
                    .Select(node => new ReportReuslt
                    {
                        Name = node.Attributes["table-name"].Value,
                        Count = int.Parse(node.Attributes["table-rows"].Value),
                        Rows = node.SelectNodes("tr").Cast<XmlNode>().Select(y => y.InnerText).ToList()
                    }).ToList();

            var linq = new XmlDocument();
            linq.LoadXml(seconds.Replace(HTMLReporting.HTMLHeader, string.Empty)
                .Replace(@"&nbsp;", string.Empty)
                .Replace(HTMLReporting.Tab, string.Empty));
            var linqResults =
                linq.SelectNodes("/html/body/table")
                    .Cast<XmlNode>()
                    .Select(node => new ReportReuslt
                    {
                        Name = node.Attributes["table-name"].Value,
                        Count = int.Parse(node.Attributes["table-rows"].Value),
                        Rows = node.SelectNodes("tr").Cast<XmlNode>().Select(y => y.InnerText).ToList()
                    }).ToList();


            var different = spResults.FullOuterJoin(linqResults, s => s.Name.ToLower(), l => l.Name.ToLower(), (s, l) => new ReportCompare
            {
                Name = s == null ? l.Name : s.Name,
                FirstCount = l == null ? (int?)null : l.Count,
                SecondCount = s == null ? (int?)null : s.Count,
                NotInFirst = (l != null && s != null) ? s.Rows.Except(l.Rows).ToList() : (s != null ? s.Rows : new List<string>()),
                NotInSecond = (s != null && l != null) ? l.Rows.Except(s.Rows).ToList() : (l != null ? l.Rows : new List<string>()),
            }).Where(x => x.FirstCount != x.SecondCount || x.NotInFirst.Any() || x.NotInSecond.Any()).ToList();

            if (different.Any())
            {
                Func<int?, Color> func = x => x.GetValueOrDefault() == default(int) ? Color.Black : Color.White;
                var title = "These reports differ.";
                var columns = new List<ColumnInfo>
                {
                    new ColumnInfo(0, ReportCompare.BoundPropertyNames.Name, "Report"),
                    new ColumnInfo<int?>(1, ReportCompare.BoundPropertyNames.FirstCount, "First Count", TypeParameter.Property, func),
                    new ColumnInfo<int?>(2, ReportCompare.BoundPropertyNames.SecondCount, "Second Count", TypeParameter.Property, func),
                    new ColumnInfo(3, ReportCompare.BoundPropertyNames.NotInFirst, "Not in First"),
                    new ColumnInfo(4, ReportCompare.BoundPropertyNames.NotInSecond, "Not in Seconds")
                };
                var differentHTML = HTMLReporting.BuildHTMLReport(different, title, columns);
                return differentHTML;
            }
            return string.Empty;
        }
        internal class ReportReuslt
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public List<string> Rows { get; set; }
        }

        internal class ReportCompare
        {
            public static class BoundPropertyNames
            {
                public const string Name = "Name";
                public const string SecondCount = "SecondCount";
                public const string FirstCount = "FirstCount";
                public const string NotInFirst = "NotInFirstString";
                public const string NotInSecond = "NotInSecondString";
            }

            public string Name { get; set; }
            public int? SecondCount { get; set; }
            public int? FirstCount { get; set; }
            public List<String> NotInFirst { get; set; }

            public string NotInFirstString
            {
                get { return string.Join(Environment.NewLine, NotInFirst); }
            }

            public List<string> NotInSecond { get; set; }

            public string NotInSecondString
            {
                get { return string.Join(Environment.NewLine, NotInSecond); }
            }
        }
    }
}
