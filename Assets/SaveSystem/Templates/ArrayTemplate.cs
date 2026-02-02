using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SaveSystem
{
    public static class ArrayTemplate
    {
        public static void WriteValue(Type type, object value)
        {
            var array = value as Array;
            SaveManager.WriteAttribute("Length", array.Length.ToString());
            SaveManager.WriteValue(typeof(IEnumerable), value);
        }

        public static object ReadValue(Type type)
        {
            if (!SaveManager.TryReadAttribute("Length", out var attribute))
            {
                return null;
            }

            int length = int.Parse(attribute);
            var elementType = type.GetElementType();
            Array array = CreateArray(elementType, length);

            int i = 0;

            while (i < length && SaveManager.ReadUntilFirstElement())
            {
                var value = SaveManager.ReadValue(elementType);
                SetValue(elementType, array, i++, value);
            }

            return array;
        }

        private static Dictionary<Type, Func<int, Array>> constructorDict = new();

        private static Array CreateArray(Type elementType, int length)
        {
            if (!constructorDict.TryGetValue(elementType, out var ctor))
            {
                var lenParam = Expression.Parameter(typeof(int), "len");
                var newArray = Expression.NewArrayBounds(elementType, lenParam);
                
                ctor = Expression.Lambda<Func<int, Array>>
                (
                    Expression.Convert(newArray, typeof(Array)), 
                    lenParam
                ).Compile();

                constructorDict[elementType] = ctor;
            }
            return ctor(length);
        }

        private static Dictionary<Type, Action<Array, int, object>> setterDict = new();

        private static void SetValue(Type elementType, Array array, int index, object value)
        {
            if (!setterDict.TryGetValue(elementType, out var func))
            {
                var arrayParam = Expression.Parameter(typeof(Array), "array");
                var indexParam = Expression.Parameter(typeof(int), "index");
                var valueParam = Expression.Parameter(typeof(object), "value");

                var castArray = Expression.Convert(arrayParam, elementType.MakeArrayType());
                var castValue = Expression.Convert(valueParam, elementType);

                var body = Expression.Assign
                (
                    Expression.ArrayAccess(castArray, indexParam),
                    castValue
                );

                func = Expression.Lambda<Action<Array, int, object>>
                (
                    body, arrayParam, indexParam, valueParam
                ).Compile();

                setterDict[elementType] = func;
            }
            func(array, index, value);
        }
    }
}