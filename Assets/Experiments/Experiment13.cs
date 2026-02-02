using SaveSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor.Overlays;
using UnityEngine;

public class Experiment13 : MonoBehaviour
{
    const int COUNT = 10000000;

    private void Start()
    {
        object instance = new TestData();
        Type type = typeof(TestData);

        MemberInfo info = type.GetField("intField");

        var reflectionReadWatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < COUNT; ++i)
        {
            type.GetField("intField").GetValue(instance);
        }
        reflectionReadWatch.Stop();

        var cacheReadWatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < COUNT; ++i)
        {
            SaveManager.GetMemberValue(type, info, instance);
        }
        cacheReadWatch.Stop();

        var reflectionWriteWatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < COUNT; ++i)
        {
            type.GetField("intField").SetValue(instance, 2);
        }
        reflectionWriteWatch.Stop();

        var cacheWriteWatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < COUNT; ++i)
        {
            SaveManager.SetMemberValue(type, info, instance, 2);
        }
        cacheWriteWatch.Stop();

        UnityEngine.Debug.Log($"{reflectionReadWatch.ElapsedMilliseconds / 1000.0} {reflectionWriteWatch.ElapsedMilliseconds / 1000.0}");
        UnityEngine.Debug.Log($"{cacheReadWatch.ElapsedMilliseconds / 1000.0} {cacheWriteWatch.ElapsedMilliseconds / 1000.0}");
    }
}