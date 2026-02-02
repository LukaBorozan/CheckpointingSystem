using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SaveSystem
{
    public static class HashSetTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            var info = SaveManager.GetMemberInfo(type, "Count")[0];
            var length = SaveManager.GetMemberValue(type, info, value);
            SaveManager.WriteAttribute("Length", length.ToString());
            SaveManager.WriteValue(typeof(IEnumerable), value);
        }

        public static object ReadValue(Type type)
        {
            if (!SaveManager.TryReadAttribute("Length", out var attribute))
            {
                return null;
            }

            int length = int.Parse(attribute);
            var set = Activator.CreateInstance(type, length);
            var elementType = type.GetGenericArguments()[0];
            var info = (MethodInfo)SaveManager.GetMemberInfo(type, "Add")[0];

            while (SaveManager.ReadUntilFirstElement())
            {
                var value = SaveManager.ReadValue(elementType);
                SaveManager.EvaluateGenericMemberFunction(info, set, value);
                // SaveManager.EvaluateGenericMethod(info, set, value);
            }

            return set;
        }
    }
}