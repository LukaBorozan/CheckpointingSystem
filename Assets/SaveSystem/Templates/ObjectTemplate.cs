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
    public static class ObjectTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            type = value.GetType();
            SaveManager.WriteAttribute("Type", SaveManager.GetTypeString(type));
            SaveManager.WriteValue(type, value);
        }

        public static object ReadValue(Type type)
        {
            var attribute = SaveManager.ReadAttribute("Type");
            type = Type.GetType(attribute);
            return SaveManager.ReadValue(type);
        }
    }
}