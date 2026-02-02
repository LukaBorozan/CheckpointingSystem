using System.Collections;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace SaveSystem
{
    public static class IEnumerableTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            var elems = value as IEnumerable;
            foreach (var elem in elems)
            {
                SaveManager.WriteValue(elem?.GetType(), "Element", elem);
            }
        }
    }
}