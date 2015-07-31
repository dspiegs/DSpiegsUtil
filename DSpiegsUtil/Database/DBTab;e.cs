using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil.Database
{
    public class DBTable
    {
        public DBTable(string name, IEnumerable<DBColumn> columns, bool hasTrigger)
        {
            Name = name;
            HasTrigger = hasTrigger;
            Columns = columns as List<DBColumn> ?? columns.ToList();
            PrimaryColumns = Columns.Where(x => x.IsPrimaryKey).ToList();
            IdentityColumns = Columns.Where(x => x.IsIdentity || (x.Index == 1 && x.SQLType == "uniqueidentifier" && x.HasDefaultValue)).ToList();
            NonIdentityColumns = Columns.Except(IdentityColumns).ToList();
        }

        public List<DBColumn> PrimaryColumns { get; private set; }
        public string Name { get; private set; }
        public bool HasTrigger { get; private set; }
        public List<DBColumn> Columns { get; private set; }
        public List<DBColumn> IdentityColumns { get; private set; }
        public List<DBColumn> NonIdentityColumns { get; private set; }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                if (Columns != null)
                {
                    hashCode = Columns.Aggregate(hashCode, (current, result) => (current * 397) ^ result.GetHashCode());
                }
                else
                {
                    hashCode = (hashCode * 397) ^ 0;
                }
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
            return Equals((DBTable)obj);
        }

        protected bool Equals(DBTable other)
        {
            if (Name != other.Name)
            {
                return false;
            }
            if (other.Columns == null && Columns == null)
            {
                return true;
            }
            if ((other.Columns == null && Columns != null) || other.Columns != null && Columns == null)
            {
                return false;
            }
            if (Columns != null && Columns.Any(result => !other.Columns.Contains(result)))
            {
                return false;
            }
            if (other.Columns != null && other.Columns.Any(result => !Columns.Contains(result)))
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
