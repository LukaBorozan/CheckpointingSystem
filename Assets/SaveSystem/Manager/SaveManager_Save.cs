using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

namespace SaveSystem
{
    public partial class SaveManager
    {
        #region Write
        private StreamWriter wfile;
        private XmlWriter writer;

        public static void BeginWrite(string path)
        {
            if (isSaving || isLoading)
            {
                return;
            }

            try
            {
                Singleton.wfile = new(path);
            }
            catch
            {
                return;
            }

            XmlWriterSettings settings = new();
            settings.Indent = true;
            Singleton.writer = XmlWriter.Create(Singleton.wfile, settings);
            Singleton.writer.WriteStartElement("SaveGame");
        }

        public static void EndWrite()
        {
            if (!isSaving || isLoading)
            {
                return;
            }
            
            Singleton.writer.WriteEndElement();
            Singleton.writer.Dispose();
            Singleton.wfile.Close();
            Singleton.wfile = null;
        }
        #endregion

        #region Write Saveable
        public static void WriteSaveable(ISaveable saveable, object blank = null)
        {
            if (!isSaving || isLoading || saveable == null)
            {
                return;
            }

            saveable.OnSave();

            Type type = saveable.GetType();

            Singleton.writer.WriteStartElement("Saveable");
            Singleton.writer.WriteAttributeString("Type", GetTypeString(type));

            var members = Singleton.GetSaveableMembers(type);
            foreach ((string name, MemberInfo info) in members)
            {
                Singleton.WriteMember(saveable, type, info, name, blank);
            }

            Singleton.writer.WriteEndElement();
        }

        public static void WriteAttribute(string name, string value)
        {
            Singleton.writer.WriteAttributeString(name, value);
        }

        private void WriteMember(ISaveable saveable, Type type, MemberInfo info, string name, object blank)
        {
            object value = GetMemberValue(type, info, saveable);
            if (ShouldWriteValue(type, info, value, blank))
            {
                WriteValue(GetMemberType(info), name, value);
            }
        }

        private bool ShouldWriteValue(Type type, MemberInfo info, object value, object blank)
        {
            if (blank == null)
            {
                return true;
            }
            
            var blankValue = GetMemberValue(type, info, blank);

            if (value == blankValue)
            {
                return false;
            }

            return !value.Equals(blankValue);
        }

        public static void WriteValue(Type type, string name, object value)
        {
            Singleton.writer.WriteStartElement(name);
            WriteValue(type, value);
            Singleton.writer.WriteEndElement();
        }

        public static void WriteValue(Type type, object value)
        {
            if (value == null)
            {
                Singleton.writer.WriteValue(null);
            }
            else if (type.IsArray)
            {
                if (SaveableTypes.saveHandlers.TryGetValue(typeof(Array), out var arrFunc))
                {
                    arrFunc(type, value);
                }
            }
            else if (type.IsGenericType)
            {
                var genType = type.GetGenericTypeDefinition();
                if (SaveableTypes.saveHandlers.TryGetValue(genType, out var genFunc))
                {
                    genFunc(type, value);
                }
            }
            else if (type.IsEnum)
            {
                Singleton.writer.WriteValue(((Enum)value).ToString());
            }
            else if (IsSystemType(type))
            {
                Singleton.writer.WriteValue(GetTypeString((Type)value));
            }
            else if (SaveableTypes.saveHandlers.TryGetValue(type, out var typeFunc))
            {
                typeFunc(type, value);
            }
        }

        private void WriteNativeValue(Type _, object value)
        {
            writer.WriteValue(value);
        }
        #endregion

        #region Init
        private void InitSave()
        {
            SaveableTypes.saveHandlers.Add(typeof(short), WriteNativeValue); 
            SaveableTypes.saveHandlers.Add(typeof(ushort), WriteNativeValue); 
            SaveableTypes.saveHandlers.Add(typeof(int), WriteNativeValue); 
            SaveableTypes.saveHandlers.Add(typeof(uint), WriteNativeValue);
            SaveableTypes.saveHandlers.Add(typeof(long), WriteNativeValue);
            SaveableTypes.saveHandlers.Add(typeof(ulong), WriteNativeValue);
            SaveableTypes.saveHandlers.Add(typeof(string), WriteNativeValue);
            SaveableTypes.saveHandlers.Add(typeof(bool), WriteNativeValue);
            SaveableTypes.saveHandlers.Add(typeof(byte), WriteNativeValue);
            SaveableTypes.saveHandlers.Add(typeof(float), WriteNativeValue);
            SaveableTypes.saveHandlers.Add(typeof(double), WriteNativeValue);
        }
        #endregion
    }
}