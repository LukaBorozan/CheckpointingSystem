using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SaveSystem
{
    public partial class SaveManager 
    {
        public static string GetTypeString(Type t)
        {
            return t == null ? null : $"{t}, {t.Assembly.GetName().Name}";
        }

        private static Type GetMemberType(MemberInfo info)
        {
            if (info is FieldInfo)
            {
                return ((FieldInfo)info).FieldType;
            }
            else
            {
                return ((PropertyInfo)info).PropertyType;
            }
        }

        private static bool IsSystemType(Type type)
        {
            return type == typeof(Type) || type.IsSubclassOf(typeof(Type));
        }
    }
}