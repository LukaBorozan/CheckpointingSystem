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
    public static class QuaternionTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            Quaternion quaternion = (Quaternion)value;
            SaveManager.WriteValue(typeof(float), "X", quaternion.x);
            SaveManager.WriteValue(typeof(float), "Y", quaternion.y);
            SaveManager.WriteValue(typeof(float), "Z", quaternion.z);
            SaveManager.WriteValue(typeof(float), "W", quaternion.w);
        }

        public static object ReadValue(Type type)
        {
            SaveManager.ReadUntilFirstElement();
            var x = SaveManager.ReadValue(typeof(float));
            SaveManager.ReadUntilFirstElement();
            var y = SaveManager.ReadValue(typeof(float));
            SaveManager.ReadUntilFirstElement();
            var z = SaveManager.ReadValue(typeof(float));
            SaveManager.ReadUntilFirstElement();
            var w = SaveManager.ReadValue(typeof(float));
            return new Quaternion((float)x, (float)y, (float)z, (float)w);
        }
    }
}