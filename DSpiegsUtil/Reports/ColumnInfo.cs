using System;
using System.Drawing;

namespace DSpiegsUtil.Reports
{
    public class ColumnInfo<T> : ColumnInfo
    {
        public override sealed TypeParameter TypeParameter { get; set; }

        public Func<T, Color> ColorFunction { get; set; }

        public override Color GetColor(object value)
        {
            if (ColorFunction == null)
            {
                return Color.White;
            }

            return (value is T || value == null) ? ColorFunction((T) value) : Color.White;
        }

        public ColumnInfo(int sortOrder, string rawColumnName, string presenttionColumnName, TypeParameter typeParameter = TypeParameter.None,
            Func<T, Color> function = null)
        {
            SortOrder = sortOrder;
            RawColumnName = rawColumnName;
            PresentationColumnName = presenttionColumnName;
            TypeParameter = typeParameter;
            ColorFunction = function;
        }

        public ColumnInfo(int sortOrder, string columnName, TypeParameter typeParameter = TypeParameter.None, Func<T, Color> function = null)
        {
            SortOrder = sortOrder;
            RawColumnName = columnName;
            PresentationColumnName = columnName;
            TypeParameter = typeParameter;
            ColorFunction = function;
        }
    }

    public class ColumnInfo
    {
        public ColumnInfo()
        {
        }

        public ColumnInfo(int sortOrder, string rawColumnName, string presenttionColumnName)
        {
            SortOrder = sortOrder;
            RawColumnName = rawColumnName;
            PresentationColumnName = presenttionColumnName;
        }

        public ColumnInfo(int sortOrder, string columnName)
        {
            SortOrder = sortOrder;
            RawColumnName = columnName;
            PresentationColumnName = columnName;
        }

        public virtual TypeParameter TypeParameter
        {
            get { return TypeParameter.None; }
            set { }
        }

        public virtual Color GetColor(object value)
        {
            return Color.White;
        }

        public int SortOrder { get; set; }
        public string RawColumnName { get; set; }
        public string PresentationColumnName { get; set; }
    }
}