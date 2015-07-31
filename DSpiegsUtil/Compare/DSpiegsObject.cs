using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil.Compare
{
    public abstract class DSpiegsObject
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> Properties = new ConcurrentDictionary<Type, PropertyInfo[]>();

        private static Type listType = typeof(List<>);
        public static Type ListType
        {
            get { return listType; }
        }
      
        public virtual IEqualityComparer<DSpiegsObject> EqualityComparer { get { return EqualityComparer<DSpiegsObject>.Default; } }

        public static bool CompareLists<T>(List<T> firstList, List<T> secondList, out List<T> notInFirst, out List<T> notInSecond,
            IEqualityComparer<T> equalityComparer = null) where T : DSpiegsObject
        {
            var comparer = equalityComparer ?? EqualityComparer<T>.Default;

            if (firstList == null)
            {
                throw new ArgumentNullException("firstList cannot be null");
            }

            if (secondList == null)
            {
                notInSecond = firstList;
                notInFirst = new List<T>();
                return false;
            }

            notInFirst = secondList.Except(firstList, comparer).ToList();
            notInSecond = firstList.Except(secondList, comparer).ToList();
            return !(notInSecond.Any() || notInFirst.Any());
        }

        /// <summary>
        /// Compare two DSpiegsObjects and returns the differences
        /// </summary>
        /// <typeparam name="T">The type with the properites to compare</typeparam>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="differences"></param>
        /// <param name="equalitycomparer">An equality comparer can be used for a quick compairsion.</param>
        /// <returns></returns>
        public static bool Compare<T>(T one, T two, out List<Difference> differences, IEqualityComparer<T> equalitycomparer = null) where T : DSpiegsObject
        {
            differences = new List<Difference>();

            var comparer = equalitycomparer ?? EqualityComparer<T>.Default;
            if (comparer.Equals(one, two))
            {
                return true;
            }

            differences = (from property in GetProperties(typeof(T))
                           let oldVal = property.GetValue(one, null)
                           let newVal = property.GetValue(two, null)
                           let compareAttribute = property.GetCustomAttribute<CustomCompareAttribute>()
                           where oldVal != null || newVal != null
                           where oldVal == null || !oldVal.Equals(newVal)
                           where compareAttribute == null || !compareAttribute.Compare(oldVal, newVal)
                           select new Difference
                           {
                               OldValue = oldVal,
                               NewValue = newVal,
                               Property = property,
                               IsList = property.PropertyType.IsSubclassOfRawGeneric(listType),
                           }).ToList();

            return !differences.Any();
        }

        /// <summary>
        /// Compares this object to another DSpiegsObject
        /// </summary>
        /// <typeparam name="T">Type to reflect properties from</typeparam>
        /// <param name="dspiegsObject">Object to compare to</param>
        /// <param name="differences">Differences between the two objects</param>
        /// <param name="equalitycomparer">Optional. If not used this.EqualityComparer is used</param>
        /// <returns></returns>
        public virtual bool Compare<T>(T dspiegsObject, out List<Difference> differences, IEqualityComparer<T> equalitycomparer = null) where T : DSpiegsObject
        {
            differences = new List<Difference>();

            if (!(this is T))
            {
                return false;
            }

            return Compare((T)this, dspiegsObject, out differences, equalitycomparer ?? EqualityComparer);
        }

        private static PropertyInfo[] GetProperties(Type t)
        {
            return !Properties.ContainsKey(t)
                ? Properties.GetOrAdd(t, t.GetProperties().Where(x => x.DeclaringType != typeof(DSpiegsObject) && x.CanWrite).ToArray())
                : Properties[t];
        }

        public override int GetHashCode()
        {
            return EqualityComparer.GetHashCode(this);
        }

        public static int GetHashCode<T>(List<T> list, IEqualityComparer<T> equalityComparer)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            var comparer = equalityComparer ?? EqualityComparer<T>.Default;

            unchecked
            {
                return list.Aggregate(17, (hash, blObject) => (hash) ^ comparer.GetHashCode(blObject));
            }
        }

        public bool Equals(DSpiegsObject obj)
        {
            return EqualityComparer.Equals(this, obj);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is DSpiegsObject)) return false;
            return Equals((DSpiegsObject)obj);
        }
    }
}
