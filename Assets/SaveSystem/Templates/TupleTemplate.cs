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
    public static class TupleTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            type = value.GetType();
            var types = type.GetGenericArguments();
            var props = GetTupleGetters(type);

            for (int i = 0; i < props.Count; ++i)
            {
                var item = props[i](value);
                SaveManager.WriteValue(types[i], "Item", item);
            }
        }

        public static object ReadValue(Type type)
        {
            var types = type.GetGenericArguments();
            int length = types.Length;
            var values = new object[length];
            
            for (int i = 0; i < length; ++i)
            {
                SaveManager.ReadUntilFirstElement();
                values[i] = SaveManager.ReadValue(types[i]);
            }

            return CreateTupleInstance(type, values);
        }

#region Tuple Constructor
        private static Dictionary<Type, Func<object[], object>> genericTupleConstructors = new();

        public static object CreateTupleInstance(Type type, params object[] args)
        {
            if (!genericTupleConstructors.TryGetValue(type, out var constructor))
            {
                var types = type.GetGenericArguments();
                var info = type.GetConstructor(types);
                var arg = Expression.Parameter(typeof(object[]));
                var exp = new Expression[types.Length];
                
                for (int i = 0; i < args.Length; ++i)
                {
                    var arr = Expression.ArrayIndex(arg, Expression.Constant(i));
                    exp[i] = Expression.Convert(arr, types[i]);
                }

                var con = Expression.New(info, exp);
                var res = Expression.Convert(con, typeof(object));

                constructor = Expression.Lambda<Func<object[], object>>(res, arg).Compile();
                genericTupleConstructors[type] = constructor;
            }

            return constructor(args);
        }
#endregion

#region Tuple Getters
        private static Dictionary<Type, List<Func<object, object>>> tupleGetterDict = new();

        private static List<Func<object, object>> GetTupleGetters(Type type)
        {
            if (!tupleGetterDict.TryGetValue(type, out var list))
            {
                list = BuildTupleGetters(type);
                tupleGetterDict[type] = list;
            }
            return list;
        }

        private static List<Func<object, object>> BuildTupleGetters(Type type)
        {
            var list = new List<Func<object, object>>();
            var prop = type.GetProperty("System.Runtime.CompilerServices.ITuple.Item", SaveableTypes.flags);
            int n = type.GetGenericArguments().Length;

            for (int i = 0; i < n; ++i)
            {
                list.Add(BuildTupleGetter(type, i, prop));
            }

            return list;
        }

        private static Func<object, object> BuildTupleGetter(Type type, int i, PropertyInfo info)
        {
            var arg = Expression.Parameter(typeof(object));
            var par = Expression.Convert(arg, type);
            var get = Expression.Call(par, info.GetGetMethod(true), Expression.Constant(i, typeof(int)));
            var res = Expression.Convert(get, typeof(object));
            return Expression.Lambda<Func<object, object>>(res, arg).Compile();
        }
#endregion
    }
}