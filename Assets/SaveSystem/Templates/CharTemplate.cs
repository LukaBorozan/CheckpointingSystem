using System.Collections;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SaveSystem
{
    public static class CharTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            SaveManager.WriteValue(typeof(string), value.ToString());
        }

        public static object ReadValue(Type type)
        {
            return ((string)SaveManager.ReadValue(typeof(string)))[0];
        }
    }
}