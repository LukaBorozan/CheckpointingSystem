using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System;

namespace SaveSystem
{
    public partial class SaveManager
    {
        #region Read
        private StreamReader rfile;
        private Stack<XmlReader> readers = new();

        public static void BeginRead(string path)
        {
            if (SaveManager.isSaving || SaveManager.isLoading)
            {
                return;
            }

            try
            {
                Singleton.rfile = new(path);
            }
            catch
            {
                return;
            }

            XmlReaderSettings settings = new();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            var reader = XmlReader.Create(Singleton.rfile, settings);
            reader.MoveToContent();
            Singleton.readers.Push(reader);
        }

        public static void EndRead()
        {
            if (SaveManager.isSaving || !SaveManager.isLoading)
            {
                return;
            }

            Singleton.readers.Clear();
            Singleton.rfile.Close();
            Singleton.rfile = null;
        }
        #endregion

        #region Read Saveable
        public static bool ReadUntilFirstElement()
        {
            return ReadUntilFirstElement(Singleton.readers.Peek());
        }        

        public static Type ReadNextSaveableType()
        {
            var reader = Singleton.readers.Peek();
            while (ReadUntilFirstElement(reader))
            {
                if (reader.LocalName.Equals("Saveable"))
                {
                    var attribute = reader.GetAttribute("Type");
                    return Type.GetType(attribute);
                }
            }
            return null;
        }

        public static string ReadAttribute(string name)
        {
            var reader = Singleton.readers.Peek();
            return reader.GetAttribute(name);
        }

        public static bool TryReadAttribute(string name, out string attribute)
        {
            attribute = ReadAttribute(name);
            return !string.IsNullOrEmpty(attribute);
        }

        public static void ReadSaveable(ISaveable saveable)
        {
            var reader = Singleton.readers.Peek().ReadSubtree();
            
            while (ReadUntilFirstElement(reader))
            {
                Type type = saveable.GetType();

                if (Singleton.TryGetSaveableMember(type, reader.LocalName, out var info))
                {
                    Singleton.readers.Push(reader);
                    var value = ReadValue(GetMemberType(info));
                    SetMemberValue(type, info, saveable, value);
                    Singleton.readers.Pop();
                }
            }

            saveable.OnLoad();
        }

        public static object ReadValue(Type type)
        {
            if (type == null)
            {
                return null;
            }

            var reader = Singleton.readers.Peek().ReadSubtree();

            if (reader == null || !ReadUntilFirstElement(reader) || reader.IsEmptyElement)
            {
                return null;
            }

            Singleton.readers.Push(reader);

            object obj = null;

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (SaveableTypes.loadHandlers.TryGetValue(genericType, out var func))
                {
                    obj = func(type);
                }
            }
            else if (type.IsArray)
            {
                if (SaveableTypes.loadHandlers.TryGetValue(typeof(Array), out var func))
                {
                    obj = func(type);
                }
            }
            else if (type.IsEnum)
            {
                obj = Enum.Parse(type, (string)Singleton.ReadNativeValue(typeof(string)));
            }
            else if (IsSystemType(type))
            {
                var typeName = (string)ReadValue(typeof(string));
                obj = Type.GetType(typeName);
            }
            else if (SaveableTypes.loadHandlers.TryGetValue(type, out var loadFunc))
            {
                obj = loadFunc(type);
            }
            
            Singleton.readers.Pop();
            return obj;
        }

        private object ReadNativeValue(Type type)
        {
            return readers.Peek().ReadElementContentAs(type, null);    
        }

        private static bool ReadUntilFirstElement(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Init
        private void InitLoad()
        {
            SaveableTypes.loadHandlers.Add(typeof(short), ReadNativeValue); 
            SaveableTypes.loadHandlers.Add(typeof(ushort), ReadNativeValue); 
            SaveableTypes.loadHandlers.Add(typeof(int), ReadNativeValue); 
            SaveableTypes.loadHandlers.Add(typeof(uint), ReadNativeValue);
            SaveableTypes.loadHandlers.Add(typeof(long), ReadNativeValue);
            SaveableTypes.loadHandlers.Add(typeof(ulong), ReadNativeValue);
            SaveableTypes.loadHandlers.Add(typeof(string), ReadNativeValue);
            SaveableTypes.loadHandlers.Add(typeof(bool), ReadNativeValue);
            SaveableTypes.loadHandlers.Add(typeof(byte), ReadNativeValue);
            SaveableTypes.loadHandlers.Add(typeof(float), ReadNativeValue);
            SaveableTypes.loadHandlers.Add(typeof(double), ReadNativeValue);
        }
        #endregion
    }
}