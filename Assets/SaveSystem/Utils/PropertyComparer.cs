using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SaveSystem
{
    internal class PropertyComparer : IEqualityComparer<PropertyInfo>
    {
        public bool Equals(PropertyInfo a, PropertyInfo b)
        {
            return GetHashCode(a) == GetHashCode(b);
        }

        public int GetHashCode(PropertyInfo info)
        {
            return info.Name.GetHashCode();
        }
    }
}