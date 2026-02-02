using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace SaveSystem
{
    internal static class TestDataComparer
    {
        public static bool Equals(TestData a, TestData b)
        {
            if (a.intField != b.intField || a.intProperty != b.intProperty)
            {
                Debug.Log("int");
                return false;
            }

            if (a.charField != b.charField || a.charProperty != b.charProperty)
            {
                Debug.Log("char");
                return false;
            }

            if (a.floatField != b.floatField || a.floatProperty != b.floatProperty)
            {
                Debug.Log("float");
                return false;
            }

            if (a.boolField != b.boolField || a.boolProperty != b.boolProperty)
            {
                Debug.Log("bool");
                return false;
            }

            if (a.stringField != b.stringField || a.stringProperty != b.stringProperty)
            {
                Debug.Log("string");
                return false;
            }

            if (!CompareArray(a.array1DField, b.array1DField) || !CompareArray(a.array1DProperty, b.array1DProperty))
            {
                Debug.Log($"array1D");
                return false;
            }

            if (!CompareArray(a.array2DField, b.array2DField) || !CompareArray(a.array2DProperty, b.array2DProperty))
            {
                Debug.Log("array2D");
                return false;
            }

            if (a.enumField != b.enumField || a.enumProperty != b.enumProperty)
            {
                Debug.Log("enum");
                return false;
            }

            if (!CompareIEnumerable(a.listField, b.listField) || !CompareIEnumerable(a.listProperty, b.listProperty))
            {
                Debug.Log("list");
                return false;
            }

            if (!CompareIEnumerable(a.setField, b.setField) || !CompareIEnumerable(a.setProperty, b.setProperty))
            {
                Debug.Log("set");
                return false;
            }

            if (!CompareDictionary(a.dictionaryField, b.dictionaryField) || !CompareDictionary(a.dictionaryProperty, b.dictionaryProperty))
            {
                Debug.Log("dictionary");
                return false;
            }

            if (!CompareTuple(a.tupleField, b.tupleField) || !CompareTuple(a.tupleProperty, b.tupleProperty))
            {
                Debug.Log($"{a.tupleField} {b.tupleField}");
                Debug.Log($"{a.tupleProperty} {b.tupleProperty}");
                Debug.Log("tuple");
                return false;
            }

            if (!CompareDateTime(a.dateTimeField, b.dateTimeField) || !CompareDateTime(a.dateTimeProperty, b.dateTimeProperty))
            {
                Debug.Log("datetime");
                return false;
            }

            if (!CompareGuid(a.guidField, b.guidField) || !CompareGuid(a.guidProperty, b.guidProperty))
            {
                Debug.Log("guid");
                return false;
            }

            if (!CompareType(a.typeField, b.typeField) || !CompareType(a.typeProperty, b.typeProperty))
            {
                Debug.Log("type");
                return false;
            }

            if (!CompareObject(a.objectField, b.objectField) || !CompareObject(a.objectProperty, b.objectProperty))
            {
                Debug.Log("object");
                return false;
            }

            return true;
        }

        private static bool CompareArray<T>(T[] a, T[] b)
        {
            if (a != null && b != null)
            {
                if (a.Length != b.Length)
                {
                    return false;
                }

                for (int i = 0; i < a.Length; ++i)
                {
                    if (!a[i].Equals(b[i]))
                    {
                        return false;
                    }
                }
            }
            else if (!(a == null && b == null))
            {
                return false;
            }

            return true;
        }

        private static bool CompareArray<T>(T[][] a, T[][] b)
        {
            if (a != null && b != null)
            {
                if (a.Length != b.Length)
                {
                    return false;
                }

                for (int i = 0; i < a.Length; ++i)
                {
                    if (!CompareArray(a[i], b[i]))
                    {
                        return false;
                    }
                }
            }
            else if (!(a == null && b == null))
            {
                return false;
            }

            return true;
        }

        private static bool CompareIEnumerable<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            if (a != null && b != null)
            {
                return CompareArray(a.ToArray(), b.ToArray());
            }
            else if (!(a == null && b == null))
            {
                return false;
            }
            return true;
        }

        private static bool CompareDictionary<A, B>(Dictionary<A, B> a, Dictionary<A, B> b)
        {
            if (a != null && b != null)
            {
                return CompareArray(a.Keys.ToArray(), b.Keys.ToArray()) && CompareArray(a.Values.ToArray(), b.Values.ToArray());
            }
            else if (!(a == null && b == null))
            {
                return false;
            }
            return true;
        }

        private static bool CompareTuple((int, bool, string) a, (int, bool, string) b)
        {
            return a.Item1 == b.Item1 && a.Item2 == b.Item2 && a.Item3 == b.Item3;
        }

        private static bool CompareDateTime(DateTime a, DateTime b)
        {
            return string.Equals(a.ToString(), b.ToString());
        }

        private static bool CompareGuid(Guid a, Guid b)
        {
            return string.Equals(a.ToString(), b.ToString());
        }

        private static bool CompareType(Type a, Type b)
        {
            return Type.Equals(a, b);
        }

        private static bool CompareObject(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            Type aType = a.GetType();
            Type bType = b.GetType();

            if (aType != bType)
            {
                return false;
            }

            if (aType == typeof(int))
            {
                return (int)a == (int)b;
            }
            else if (aType == typeof(Dictionary<float, ulong>))
            {
                return CompareDictionary((Dictionary<float, ulong>)a, (Dictionary<float, ulong>)b);
            }
            else if (aType == typeof((int, bool, string)))
            {
                return CompareTuple(((int, bool, string))a, ((int, bool, string))b);
            }
            else if (aType == typeof(float[]))
            {
                return CompareArray((float[])a, (float[])b);
            }
            else if (aType == typeof(Guid))
            {
                return CompareGuid((Guid)a, (Guid)b);
            }

            return false;
        }
    }
}
