using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil
{
    public static class Reflection
    {
        //http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof (Byte),
            typeof (SByte),
            typeof (Int16),
            typeof (UInt16),
            typeof (Int32),
            typeof (UInt32),
            typeof (Int64),
            typeof (UInt64),
            typeof (Decimal),
            typeof (Single),
            typeof (Double),
            typeof (BigInteger)
        };

        /// <summary>
        /// Check to see if a type is used for storing a number
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="includeNullable">Check for nullable types that store numbers</param>
        /// <returns>Returns whether the type is used for storing a number</returns>
        public static bool IsNumericType(this Type type, bool includeNullable = true)
        {
            return NumericTypes.Contains(type) || (includeNullable &&
                                                   NumericTypes.Contains(Nullable.GetUnderlyingType(type)));
        }
    }
}
