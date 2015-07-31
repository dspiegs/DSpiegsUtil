using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil.Database
{
    public class DBColumn
    {
        public DBColumn(string name, string sqlType, bool isNullable, bool isIdentity, short maxLength, int index, string fkTable, string fkColumn,
            bool hasDefaultValue, bool isPrimaryKey)
        {
            Name = name;
            SQLType = sqlType;
            CLRType = DBDataTypesHelper.GetCLRType(sqlType);
            IsNullable = isNullable;
            IsIdentity = isIdentity;
            MaxLength = maxLength;
            Index = index;
            HasDefaultValue = hasDefaultValue;
            IsPrimaryKey = isPrimaryKey;
            IsNumeric = CLRType != null && CLRType.IsNumericType();
            NameAndType = string.Format("{0} ({1}{2})", Name, SQLType, IsNullable ? "?" : string.Empty);
            if (!string.IsNullOrWhiteSpace(fkTable) && !string.IsNullOrWhiteSpace(fkColumn))
            {
                HasFKConstraint = true;
                Constraint = new Tuple<string, string>(fkTable, fkColumn);
            }
        }

        public bool HasFKConstraint { get; private set; }
        public Tuple<string, string> Constraint { get; private set; }
        public string Name { get; set; }

        public string Type
        {
            get { return DBDataTypesHelper.GetCLRTypeString(SQLType); }
            set { }
        }

        public int Index { get; private set; }
        public bool HasDefaultValue { get; private set; }
        public bool IsPrimaryKey { get; private set; }
        public string SQLType { get; private set; }
        public Type CLRType { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsNumeric { get; private set; }
        public string NameAndType { get; private set; }
        public bool IsIdentity { get; private set; }
        public short MaxLength { get; private set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SQLType != null ? SQLType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsNullable.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((DBColumn)obj);
        }

        protected bool Equals(DBColumn other)
        {
            return string.Equals(Name, other.Name) && string.Equals(SQLType, other.SQLType) && IsNullable == other.IsNullable;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", SQLType, IsNullable ? "?" : string.Empty);
        }
    }
}
