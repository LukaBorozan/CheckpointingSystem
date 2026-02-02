using System.Collections;
using System.Xml;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SaveSystem
{
    public static class DateTimeTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            SaveManager.WriteValue(typeof(string), value.ToString());
        }

        public static object ReadValue(Type type)
        {
            string value = (string)SaveManager.ReadValue(typeof(string));
            return DateTime.Parse(value);
        }
    }
}