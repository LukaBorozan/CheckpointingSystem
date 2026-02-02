using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace SaveSystem
{
    public static class ListTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            var list = value as IList;
            SaveManager.WriteAttribute("Length", list.Count.ToString());
            SaveManager.WriteValue(typeof(IEnumerable), value);
        }
        
        public static object ReadValue(Type type)
        {
            if (!SaveManager.TryReadAttribute("Length", out var attribute))
            {
                return null;
            }

            int length = int.Parse(attribute);
            var elemType = type.GetGenericArguments()[0];
            IList list = (IList)Activator.CreateInstance(type, length);

            while (SaveManager.ReadUntilFirstElement())
            {
                var value = SaveManager.ReadValue(elemType);
                list.Add(value);
            }

            return list;
        }
    }
}