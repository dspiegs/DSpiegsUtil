using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil.Compare
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class CustomCompareAttribute : Attribute
    {
        public abstract bool Compare(object a, object b);

    }
}
