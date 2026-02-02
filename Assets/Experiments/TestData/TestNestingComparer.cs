using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace SaveSystem
{
    internal static class TestNestingComparer
    {
        public static bool Equals(TestNesting a, TestNesting b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {  
                return false; 
            }

            return AreEqual(a.data, b.data);
        }

        private static bool AreEqual
        (
            Dictionary<List<(string, int)>, HashSet<(List<char>, Dictionary<int, (float, bool)>)>> a,
            Dictionary<List<(string, int)>, HashSet<(List<char>, Dictionary<int, (float, bool)>)>> b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Count != b.Count) return false;

            foreach (var kvpA in a)
            {
                var matchingKey = b.Keys.FirstOrDefault(k => ListTupleEquals(k, kvpA.Key));
                if (matchingKey == null) return false;

                if (!HashSetValueEquals(kvpA.Value, b[matchingKey]))
                    return false;
            }

            return true;
        }

        static bool ListTupleEquals(List<(string, int)> x, List<(string, int)> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Count != y.Count) return false;

            for (int i = 0; i < x.Count; i++)
            {
                if (!x[i].Equals(y[i])) return false;
            }

            return true;
        }

        static bool HashSetValueEquals(
            HashSet<(List<char>, Dictionary<int, (float, bool)>)> x,
            HashSet<(List<char>, Dictionary<int, (float, bool)>)> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Count != y.Count) return false;

            foreach (var itemX in x)
            {
                bool found = y.Any(itemY => ValueTupleDeepEquals(itemX, itemY));
                if (!found) return false;
            }

            return true;
        }

        static bool ValueTupleDeepEquals(
            (List<char>, Dictionary<int, (float, bool)>) a,
            (List<char>, Dictionary<int, (float, bool)>) b)
        {
            return SequenceEqualNullSafe(a.Item1, b.Item1)
                && DictionaryEquals(a.Item2, b.Item2);
        }

        static bool SequenceEqualNullSafe(List<char> x, List<char> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.SequenceEqual(y);
        }

        static bool DictionaryEquals(
            Dictionary<int, (float, bool)> x,
            Dictionary<int, (float, bool)> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Count != y.Count) return false;

            foreach (var kvp in x)
            {
                if (!y.TryGetValue(kvp.Key, out var val)) return false;
                if (!kvp.Value.Equals(val)) return false;
            }

            return true;
        }
    }
}
