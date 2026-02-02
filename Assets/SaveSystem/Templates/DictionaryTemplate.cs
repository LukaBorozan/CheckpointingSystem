using System.Collections;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace SaveSystem
{
    public static class DictionaryTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            var dictionary = value as IDictionary;
            int length = dictionary.Count;
            
            SaveManager.WriteAttribute("Length", length.ToString());

            if (length > 0)
            {
                var args = type.GetGenericArguments();
                var keyType = args[0];
                var valueType = args[1];

                foreach (DictionaryEntry pair in dictionary)
                {
                    SaveManager.WriteValue(keyType, "Key", pair.Key);
                    SaveManager.WriteValue(valueType, "Value", pair.Value);
                }
            }
        }

        public static object ReadValue(Type type)
        {
            if (!SaveManager.TryReadAttribute("Length", out var attribute))
            {
                return null;
            }

            var length = int.Parse(attribute);
            var dictionary = (IDictionary)Activator.CreateInstance(type, length);

            if (length > 0)
            {
                var args = type.GetGenericArguments();
                var keyType = args[0];
                var valueType = args[1];

                while (length-- > 0)
                {
                    SaveManager.ReadUntilFirstElement();
                    var key = SaveManager.ReadValue(keyType);
                    SaveManager.ReadUntilFirstElement();
                    var value = SaveManager.ReadValue(valueType);
                    dictionary[key] = value;
                }
            }

            return dictionary;
        }
    }
}