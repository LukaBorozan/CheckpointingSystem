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
    public static class GuidTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            SaveManager.WriteValue(typeof(string), ((Guid)value).ToString("N"));
        }

        public static object ReadValue(Type type)
        {
            string value = (string)SaveManager.ReadValue(typeof(string));
            return Guid.ParseExact(value, "N");
        }
    }
}