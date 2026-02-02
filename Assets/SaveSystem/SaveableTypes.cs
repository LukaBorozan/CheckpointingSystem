using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;

namespace SaveSystem
{
    public static class SaveableTypes
    {
        public const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        internal static readonly Dictionary<Type, Action<Type, object>> saveHandlers = new()
        {
            {typeof(object), ObjectTemplate.WriteValue},
            {typeof(char), CharTemplate.WriteValue},
            {typeof(IEnumerable), IEnumerableTemplate.WriteValue},
            {typeof(Array), ArrayTemplate.WriteValue},
            {typeof(List<>), ListTemplate.WriteValue},
            {typeof(HashSet<>), HashSetTemplate.WriteValue},
            {typeof(Dictionary<,>), DictionaryTemplate.WriteValue},
            {typeof(ValueTuple<,,,,,,>), TupleTemplate.WriteValue},
            {typeof(ValueTuple<,,,,,>), TupleTemplate.WriteValue},
            {typeof(ValueTuple<,,,,>), TupleTemplate.WriteValue},
            {typeof(ValueTuple<,,,>), TupleTemplate.WriteValue},
            {typeof(ValueTuple<,,>), TupleTemplate.WriteValue},
            {typeof(ValueTuple<,>), TupleTemplate.WriteValue},
            {typeof(ValueTuple<>), TupleTemplate.WriteValue},
            {typeof(Vector3), Vector3Template.WriteValue},
            {typeof(Quaternion), QuaternionTemplate.WriteValue},
            {typeof(DateTime), DateTimeTemplate.WriteValue},
            {typeof(Guid), GuidTemplate.WriteValue},
        };

        internal static readonly Dictionary<Type, Func<Type, object>> loadHandlers = new()
        {
            {typeof(object), ObjectTemplate.ReadValue},
            {typeof(char), CharTemplate.ReadValue},
            {typeof(Array), ArrayTemplate.ReadValue},
            {typeof(List<>), ListTemplate.ReadValue},
            {typeof(HashSet<>), HashSetTemplate.ReadValue},
            {typeof(Dictionary<,>), DictionaryTemplate.ReadValue},
            {typeof(ValueTuple<,,,,,,>), TupleTemplate.ReadValue},
            {typeof(ValueTuple<,,,,,>), TupleTemplate.ReadValue},
            {typeof(ValueTuple<,,,,>), TupleTemplate.ReadValue},
            {typeof(ValueTuple<,,,>), TupleTemplate.ReadValue},
            {typeof(ValueTuple<,,>), TupleTemplate.ReadValue},
            {typeof(ValueTuple<,>), TupleTemplate.ReadValue},
            {typeof(ValueTuple<>), TupleTemplate.ReadValue},
            {typeof(Vector3), Vector3Template.ReadValue},
            {typeof(Quaternion), QuaternionTemplate.ReadValue},
            {typeof(DateTime), DateTimeTemplate.ReadValue},
            {typeof(Guid), GuidTemplate.ReadValue},
        };

        internal static readonly HashSet<Type> terminalTypes = new()
        {
            typeof(MonoBehaviour),
        };
    }
}