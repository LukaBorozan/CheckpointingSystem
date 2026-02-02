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
    public static class Vector3Template
    {
        public static void WriteValue(Type type, object value)
        {
            Vector3 vector = (Vector3)value;
            SaveManager.WriteValue(typeof(float), "X", vector.x);
            SaveManager.WriteValue(typeof(float), "Y", vector.y);
            SaveManager.WriteValue(typeof(float), "Z", vector.z);
        }

        public static object ReadValue(Type type)
        {
            SaveManager.ReadUntilFirstElement();
            var x = SaveManager.ReadValue(typeof(float));
            SaveManager.ReadUntilFirstElement();
            var y = SaveManager.ReadValue(typeof(float));
            SaveManager.ReadUntilFirstElement();
            var z = SaveManager.ReadValue(typeof(float));
            return new Vector3((float)x, (float)y, (float)z);
        }
    }
}