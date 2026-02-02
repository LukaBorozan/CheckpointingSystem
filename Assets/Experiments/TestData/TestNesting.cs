using SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TestNesting : ISaveable
{
    [Save] public Dictionary<List<(string, int)>, HashSet<(List<char>, Dictionary<int, (float, bool)>)>> data;

    public static TestNesting Randomize(int maxLen)
    {
        int len = random.Next(1, GetSize(maxLen));
        TestNesting data = new();
        data.data = new(len);

        for (int i = 0; i < len; ++i)
        {
            int lenKey = random.Next(1, GetSize(maxLen));
            int lenVal = random.Next(1, GetSize(maxLen));
            data.data[GenerateKey(lenKey)] = GenerateValue(lenVal); 
        }

        return data;
    }

    #region Generators
    private static System.Random random = new();

    private static int GetSize(int maxLen)
    {
        int pow = (int)Mathf.Pow(maxLen, 0.4125f);
        return Mathf.Max(2, pow);
    }

    private static List<(string, int)> GenerateKey(int maxLen)
    {
        int len = random.Next(1, maxLen);
        List<(string, int)> result = new(len);

        for (int i = 0; i < len; ++i)
        {
            result.Add((GetRandomString(maxLen), random.Next()));
        }

        return result;
    }

    private static HashSet<(List<char>, Dictionary<int, (float, bool)>)> GenerateValue(int maxLen)
    {
        int len = random.Next(1, maxLen);
        HashSet<(List<char>, Dictionary<int, (float, bool)>)> result = new(len);

        for (int i = 0; i < len; ++i)
        {
            result.Add((GetRandomList(maxLen), GetRandomDict(maxLen)));
        }

        return result;
    }

    private static string GetRandomString(int maxLen)
    {
        int len = random.Next(1, maxLen);
        string str = "";

        for (int i = 0; i < len; ++i)
        {
            str += (char)random.Next(65, 90);
        }

        return str;
    }

    private static List<char> GetRandomList(int maxLen)
    {
        int len = random.Next(1, maxLen);
        List<char> result = new(len);

        for (int i = 0; i < len; ++i)
        {
            result.Add((char)random.Next(65, 90));
        }

        return result;
    }

    private static Dictionary<int, (float, bool)> GetRandomDict(int maxLen)
    {
        int len = random.Next(1, maxLen);
        Dictionary<int, (float, bool)> result = new(len);

        for (int i = 0; i < len; ++i)
        {
            result[random.Next()] = ((float)random.NextDouble(), random.Next() % 2 == 0);
        }

        return result;
    }
    #endregion
}
