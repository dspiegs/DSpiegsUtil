using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil.Compare
{
    public class Difference
    {
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public PropertyInfo Property { get; set; }

        public string PropertyName
        {
            get { return Property.Name.Replace("_", string.Empty); }
        }

        public bool IsList { get; set; }

        public string OldDisplayValue
        {
            get { return OldValue == null ? "<null>" : OldValue.ToString().Replace("_", " "); }
        }

        public string NewDisplayValue
        {
            get { return NewValue == null ? "<null>" : NewValue.ToString().Replace("_", " "); }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} => {2}", PropertyName, OldDisplayValue, NewDisplayValue);
        }
    }
}
