using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SaveSystem
{
    public partial class SaveManager 
    {
        #region Members
        private Dictionary<Type, Dictionary<string, MemberInfo[]>> stringToGetter = new();

        public static MemberInfo[] GetMemberInfo(Type type, string name)
        {
            if (!Singleton.stringToGetter.TryGetValue(type, out var memberDict))
            {
                memberDict = new();
                Singleton.stringToGetter[type] = memberDict;
            }
            if (!memberDict.TryGetValue(name, out var info))
            {
                info = type.GetMember(name, SaveableTypes.flags);
                memberDict[name] = info;
            }
            return info;
        }
        #endregion

        #region Saveable Members
        
        private Dictionary<Type, Dictionary<string, MemberInfo>> memberDict = new();

        private Dictionary<string, MemberInfo> GetSaveableMembers(Type type)
        {
            if (!memberDict.TryGetValue(type, out var memberInfoDict))
            {
                memberInfoDict = BuildMemberDict(type, ProcessMemberInfo);
                memberDict[type] = memberInfoDict;
            }
            return memberInfoDict;
        }

        private bool TryGetSaveableMember(Type type, string name, out MemberInfo info)
        {
            if (!memberDict.TryGetValue(type, out var memberInfoDict))
            {
                memberInfoDict = BuildMemberDict(type, ProcessMemberInfo);
                memberDict[type] = memberInfoDict;
            }
            return memberInfoDict.TryGetValue(name, out info);
        }

        private Dictionary<string, MemberInfo> BuildMemberDict(Type type, Action<MemberInfo, Dictionary<string, MemberInfo>> callback)
        {
            var dict = new Dictionary<string, MemberInfo>();

            while (type != null && !SaveableTypes.terminalTypes.Contains(type))
            {
                foreach (var info in type.GetProperties(SaveableTypes.flags))
                {
                    callback(info, dict);
                }
                foreach (var info in type.GetFields(SaveableTypes.flags))
                {
                    callback(info, dict);
                }
                type = type.BaseType;
            }
            return dict;
        }

        private void ProcessMemberInfo(MemberInfo info, Dictionary<string, MemberInfo> dict)
        {
            if (Attribute.IsDefined(info, typeof(SaveAttribute)))
            {
                var attribute = (SaveAttribute)Attribute.GetCustomAttribute(info, typeof(SaveAttribute));

                if (string.IsNullOrEmpty(attribute.Name))
                {
                    dict[info.Name] = info;
                }
                else
                {
                    dict[attribute.Name] = info;
                }
            }
        }
        #endregion

        #region Member Getters
        private Dictionary<Type, Dictionary<MemberInfo, Func<object, object>>> memberGetters = new();

        public static object GetMemberValue(Type type, MemberInfo info, object instance)
        {
            if (!Singleton.memberGetters.TryGetValue(type, out var memberDict))
            {
                memberDict = new();
                Singleton.memberGetters[type] = memberDict;
            }
            if (!memberDict.TryGetValue(info, out var func))
            {
                if (info is PropertyInfo)
                {
                    func = Singleton.BuildPropertyGetter(type, (PropertyInfo)info);
                }
                else // if (info is FieldInfo)
                {
                    func = Singleton.BuildFieldGetter(type, (FieldInfo)info);
                }
                memberDict[info] = func;
            }
            return memberDict[info](instance);
        }

        private Func<object, object> BuildPropertyGetter(Type type, PropertyInfo info)
        {
            var arg = Expression.Parameter(typeof(object));
            var par = Expression.Convert(arg, type);
            var get = Expression.Call(par, info.GetGetMethod(true));
            var res = Expression.Convert(get, typeof(object));
            return Expression.Lambda<Func<object, object>>(res, arg).Compile();
        }

        private Func<object, object> BuildFieldGetter(Type type, FieldInfo info)
        {
            var arg = Expression.Parameter(typeof(object));
            var par = Expression.Convert(arg, type);
            var get = Expression.Field(par, info);
            var res = Expression.Convert(get, typeof(object));
            return Expression.Lambda<Func<object, object>>(res, arg).Compile();
        }
        #endregion

        #region Member Setters
        private Dictionary<Type, Dictionary<MemberInfo, Action<object, object>>> memberSetters = new();

        public static void SetMemberValue(Type type, MemberInfo info, object instance, object value)
        {
            if (!Singleton.memberSetters.TryGetValue(type, out var memberDict))
            {
                memberDict = new();
                Singleton.memberSetters[type] = memberDict;
            }
            if (!memberDict.TryGetValue(info, out var func))
            {
                if (info is PropertyInfo)
                {
                    func = Singleton.BuildPropertySetter(type, (PropertyInfo)info);
                }
                else // if (info is FieldInfo)
                {
                    func = Singleton.BuildFieldSetter(type, (FieldInfo)info);
                }
                memberDict[info] = func;
            }
            memberDict[info](instance, value);
        }

        private Action<object, object> BuildPropertySetter(Type type, PropertyInfo info)
        {
            var arg1 = Expression.Parameter(typeof(object));
            var arg2 = Expression.Parameter(typeof(object));
            var par1 = Expression.Convert(arg1, type);
            var par2 = Expression.Convert(arg2, info.PropertyType);
            var set = Expression.Call(par1, info.GetSetMethod(true), par2);
            return Expression.Lambda<Action<object, object>>(set, arg1, arg2).Compile();
        }

        private Action<object, object> BuildFieldSetter(Type type, FieldInfo info)
        {
            var arg1 = Expression.Parameter(typeof(object));
            var arg2 = Expression.Parameter(typeof(object));
            var par1 = Expression.Convert(arg1, type);
            var par2 = Expression.Convert(arg2, info.FieldType);
            var fld = Expression.Field(par1, info);
            var set = Expression.Assign(fld, par2);
            return Expression.Lambda<Action<object, object>>(set, arg1, arg2).Compile();
        }
        #endregion
       
        #region Generic Methods
/*
        private Dictionary<MethodInfo, Func<object, object[], object>> genericMethods = new();

        /// <summary>
        /// Evaluate method described by MethodInfo faster than with Invoke by compiled expression caching.
        /// </summary>
        public static object EvaluateGenericMethod(MethodInfo methodInfo, object instance, params object[] arguments)
        {
            // Check whether the method is cached.
            if (!Singleton.genericMethods.TryGetValue(methodInfo, out var func))
            {
                // Methods are cached as functions of type object[] -> object which are generated as wrappers around
                // the method calls.

                // Fetching the instance argument.
                var funcInstanceArg = Expression.Parameter(typeof(object));

                // Converting the instance argument.
                var methodInstanceArg = Expression.Convert(funcInstanceArg, instance.GetType());

                // Fetching the function argument.
                var funcArgs = Expression.Parameter(typeof(object[]));

                // Fetching method parameter types.
                var methodArgTypes = methodInfo.GetParameters();

                // Parameter expressions used to call the method.
                Expression[] methodArgs = new Expression[methodArgTypes.Length];

                // Each argument of the method corresponds to an element in the funcArgs array.
                for (int i = 0; i < arguments.Length; ++i)
                {
                    // Fetching the funcArgs element at index i.
                    var funcArg = Expression.ArrayAccess(funcArgs, Expression.Constant(i));

                    // Fetching the corresponding method argument type.
                    var methodArgType = methodArgTypes[i].ParameterType;

                    // Converting the function argument into the method argument.
                    methodArgs[i] = Expression.Convert(funcArg, methodArgType); 
                }

                // Calling the method using the converted arguments.
                var methodResult = Expression.Call(methodInstanceArg, methodInfo, methodArgs);

                // Converting the result which method returns into an object.
                var funcResult = Expression.Convert(methodResult, typeof(object));

                // Compiling the expression and obtaining the function.
                func = Expression.Lambda<Func<object, object[], object>>(funcResult, funcInstanceArg, funcArgs).Compile();

                // Caching the function.
                Singleton.genericMethods[methodInfo] = func;
            }

            // Evaluating the function.
            return func(instance, arguments);
        }*/

        private Dictionary<MethodBase, object> methodCache = new();

        // MethodBase info, object instance, params object[] arguments
        public static void EvaluateGenericAction(MethodInfo info)
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, null, null);
                Singleton.methodCache[info] = func;
            }
            ((Action)func)();
        }

        public static object EvaluateGenericFunction(MethodInfo info)
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, null, null);
                Singleton.methodCache[info] = func;
            }
            return ((Func<object>)func)();
        }

        public static void EvaluateGenericMemberAction(MethodInfo info, object instance)
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, instance, null);
                Singleton.methodCache[info] = func;
            }
            ((Action<object>)func)(instance);
        }

        public static object EvaluateGenericMemberFunction(MethodInfo info, object instance)
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, instance, null);
                Singleton.methodCache[info] = func;
            }
            return ((Func<object, object>)func)(instance);
        }

        public static void EvaluateGenericAction(MethodInfo info, params object[] arguments) 
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, null, arguments);
                Singleton.methodCache[info] = func;
            }
            ((Action<object[]>)func)(arguments);
        }

        public static object EvaluateGenericFunction(MethodInfo info, params object[] arguments) 
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, null, arguments);
                Singleton.methodCache[info] = func;
            }
            return ((Func<object[], object>)func)(arguments);
        }

        public static void EvaluateGenericMemberAction(MethodInfo info, object instance, params object[] arguments) 
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, instance, arguments);
                Singleton.methodCache[info] = func;
            }
            ((Action<object, object[]>)func)(instance, arguments);
        }

        public static object EvaluateGenericMemberFunction(MethodInfo info, object instance, params object[] arguments) 
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, instance, arguments);
                Singleton.methodCache[info] = func;
            }
            return ((Func<object, object[], object>)func)(instance, arguments);
        }

        public static object EvaluateGenericConstructor(ConstructorInfo info)
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, null, null);
                Singleton.methodCache[info] = func;
            }
            return ((Func<object>)func)();
        }

        public static object EvaluateGenericConstructor(ConstructorInfo info, params object[] arguments)
        {
            if (!Singleton.methodCache.TryGetValue(info, out var func))
            {
                func = BuildMethodCaller(info, null, arguments);
                Singleton.methodCache[info] = func;
            }
            return ((Func<object[], object>)func)(arguments);
        }

        private static object BuildMethodCaller(MethodBase info, object instance, object[] arguments)
        {
            // Fetching the instance argument.
            var funcInstanceArg = instance == null ? null : 
                Expression.Parameter(typeof(object));

            // Converting the instance argument.
            var methodInstanceArg = instance == null ? null : 
                Expression.Convert(funcInstanceArg, instance.GetType());

            // Fetching the function argument.
            var funcArgs = arguments == null ? null : 
                Expression.Parameter(typeof(object[]));

            // Fetching method parameter types.
            var methodArgTypes = arguments == null ? null : 
                info.GetParameters();

            // Parameter expressions used to call the method.
            Expression[] methodArgs = arguments == null ? null : 
                new Expression[methodArgTypes.Length];

            // Each argument of the method corresponds to an element in the funcArgs array.
            for (int i = 0; i < arguments?.Length; ++i)
            {
                // Fetching the funcArgs element at index i.
                var funcArg = Expression.ArrayAccess(funcArgs, Expression.Constant(i));

                // Fetching the corresponding method argument type.
                var methodArgType = methodArgTypes[i].ParameterType;

                // Converting the function argument into the method argument.
                methodArgs[i] = Expression.Convert(funcArg, methodArgType); 
            }

            Expression methodResult = null;
            bool hasReturnType = false;

            if (info.IsConstructor)
            {
                if (arguments == null)
                {
                    methodResult = Expression.New((ConstructorInfo)info);
                }
                else
                {
                    methodResult = Expression.New((ConstructorInfo)info, methodArgs);
                }

                hasReturnType = true;
            }
            else 
            {
                if (arguments == null && instance == null)
                {
                    methodResult = Expression.Call((MethodInfo)info);
                }
                else if (arguments == null && instance != null)
                {
                    methodResult = Expression.Call(methodInstanceArg, (MethodInfo)info);
                }
                else if (arguments != null && instance == null)
                {
                    methodResult = Expression.Call((MethodInfo)info, methodArgs);
                }
                else // if (arguments != null && instance != null)
                {
                    methodResult = Expression.Call(methodInstanceArg, (MethodInfo)info, methodArgs);
                }

                hasReturnType = ((MethodInfo)info).ReturnType != typeof(void);
            }

            var funcResult = !hasReturnType ? methodResult :
                Expression.Convert(methodResult, typeof(object));

            if (hasReturnType)
            {
                if (arguments == null && instance == null)
                {
                    return Expression.Lambda<Func<object>>(funcResult).Compile();
                }
                else if (arguments == null && instance != null)
                {
                    return Expression.Lambda<Func<object, object>>(funcResult, funcInstanceArg).Compile();
                }
                else if (arguments != null && instance == null)
                {
                    return Expression.Lambda<Func<object[], object>>(funcResult, funcArgs).Compile();
                }
                else // if (arguments != null && instance != null)
                {
                    return Expression.Lambda<Func<object, object[], object>>(funcResult, funcInstanceArg, funcArgs).Compile();
                }
            }
            else
            {
                if (arguments == null && instance == null)
                {
                    return Expression.Lambda<Action>(funcResult).Compile();
                }
                else if (arguments == null && instance != null)
                {
                    return Expression.Lambda<Action<object>>(funcResult, funcInstanceArg).Compile();
                }
                else if (arguments != null && instance == null)
                {
                    return Expression.Lambda<Action<object[]>>(funcResult, funcArgs).Compile();
                }
                else // if (arguments != null && instance != null)
                {
                    return Expression.Lambda<Action<object, object[]>>(funcResult, funcInstanceArg, funcArgs).Compile();
                }
            }
        }
        #endregion
    }
}