using SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TestData : ISaveable
{
    [Save] public int intField;
    [Save] public int intProperty { get; set; }


    [Save] public char charField;
    [Save] public char charProperty { get; set; }


    [Save] public float floatField;
    [Save] public float floatProperty { get; set; }


    [Save] public bool boolField;
    [Save] public bool boolProperty { get; set; }


    [Save] public string stringField;
    [Save] public string stringProperty { get; set; }


    [Save] public int[] array1DField;
    [Save] public int[] array1DProperty { get; set; }


    [Save] public short[][] array2DField;
    [Save] public short[][] array2DProperty { get; set; }


    [Save] public TestEnum enumField;
    [Save] public TestEnum enumProperty { get; set; }


    [Save] public List<float> listField;
    [Save] public List<float> listProperty { get; set; }


    [Save] public HashSet<char> setField;
    [Save] public HashSet<char> setProperty { get; set; }


    [Save] public Dictionary<float, ulong> dictionaryField;
    [Save] public Dictionary<float, ulong> dictionaryProperty { get; set; }


    [Save] public (int, bool, string) tupleField;
    [Save] public (int, bool, string) tupleProperty { get; set; }


    [Save] public DateTime dateTimeField;
    [Save] public DateTime dateTimeProperty { get; set; }


    [Save] public Guid guidField;
    [Save] public Guid guidProperty { get; set; }


    [Save] public Type typeField;
    [Save] public Type typeProperty { get; set; }


    [Save] public object objectField;
    [Save] public object objectProperty { get; set; }


    public int notBeingSavedField;
    public int notBeingSavedProperty { get; set; }

    #region Generators
    private static System.Random random = new();

    private static string GetRandomString(int maxLen = 100)
    {
        if (random.Next(0, 10) < 2)
        {
            return null;
        }

        int len = random.Next(0, maxLen);
        string str = "";

        for (int i = 0; i < len; ++i)
        {
            str += (char)random.Next(65, 90);
        }

        return str;
    }

    private static int[] GetRandomInt1DArray(int maxLen = 100)
    {
        if (random.Next(0, 10) < 2)
        {
            return null;
        }

        int len = random.Next(1, maxLen);
        int[] array = new int[len];

        for (int i = 0; i < len; ++i)
        {
            array[i] = random.Next();
        }

        return array;
    }

    private static short[] GetRandomShort1DArray(int maxLen = 100)
    {
        if (random.Next(0, 10) < 2)
        {
            return null;
        }

        int len = random.Next(1, maxLen);
        short[] array = new short[len];

        for (int i = 0; i < len; ++i)
        {
            array[i] = (short)random.Next();
        }

        return array;
    }

    private static float[] GetRandomFloat1DArray(int maxLen = 100)
    {
        if (random.Next(0, 10) < 2)
        {
            return null;
        }

        int len = random.Next(1, maxLen);
        float[] array = new float[len];

        for (int i = 0; i < len; ++i)
        {
            array[i] = (float)random.NextDouble();
        }

        return array;
    }

    private static char[] GetRandomChar1DArray(int maxLen = 100)
    {
        if (random.Next(0, 10) < 2)
        {
            return null;
        }

        int len = random.Next(1, maxLen);
        char[] array = new char[len];

        for (int i = 0; i < len; ++i)
        {
            array[i] = (char)random.Next(65, 90);
        }

        return array;
    }

    private static short[][] GetRandomShort2DArray(int maxLen1 = 20, int maxLen2 = 20)
    {
        if (random.Next(0, 10) < 2)
        {
            return null;
        }

        int len = random.Next(1, maxLen1);
        short[][] array = new short[len][];

        for (int i = 0; i < len; ++i)
        {
            array[i] = GetRandomShort1DArray(maxLen2);
        }

        return array;
    }

    private static Dictionary<float, ulong> GetRandomDictionary(int maxLen = 100)
    {
        if (random.Next(0, 10) < 2)
        {
            return null;
        }

        int len = random.Next(1, maxLen);
        Dictionary<float, ulong> dict = new(len);

        for (int i = 0; i < len; ++i)
        {
            dict.TryAdd((float)random.NextDouble(), (ulong)random.Next());
        }

        return dict;
    }

    private static (int, bool, string) GetRandomTuple(int maxLen)
    {
        return (random.Next(), random.Next(0, 1) == 0, GetRandomString(maxLen));
    }

    private static Type GetRandomType()
    {
        switch (random.Next(0, 7))
        {
            case 0: return typeof(int);
            case 1: return typeof(bool[][]);
            case 2: return typeof(string);
            case 3: return typeof(DateTime);
            case 4: return typeof(TestData);
            case 5: return typeof(List<(int, float)>);
            case 6: return typeof(Action);
            default: return null;
        }
    }

    private static List<float> GetRandomList(int maxLen = 100)
    {
        float[] array = GetRandomFloat1DArray(maxLen);
        return array == null ? null : new(array);
    }

    private static HashSet<char> GetRandomSet(int maxLen = 100)
    {
        char[] array = GetRandomChar1DArray(maxLen);
        return array == null ? null : new(array);
    }

    private static object GetRandomObject(int maxLen = 100)
    {
        int choice = random.Next(0, 5);

        if (choice == 0)
        {
            return random.Next();
        }
        else if (choice == 1)
        {
            return GetRandomDictionary(maxLen / 2);
        }
        else if (choice == 2)
        {
            return GetRandomTuple(maxLen);
        }
        else if (choice == 3)
        {
            return GetRandomFloat1DArray(maxLen);
        }
        else if (choice == 4)
        {
            return Guid.NewGuid();
        }

        return null;
    }

    private static Array enumValues = Enum.GetValues(typeof(TestEnum));
    private static TestEnum GetRandomEnumValue()
    {
        return (TestEnum)enumValues.GetValue(random.Next(0, enumValues.Length));
    }
    #endregion

    public static TestData Randomize(int maxLen = 100)
    {
        TestData data = new TestData();

        data.intField = random.Next();
        data.intProperty = random.Next();

        data.charField = (char)random.Next(65, 90);
        data.charProperty = (char)random.Next(65, 90);

        data.floatField = (float)random.NextDouble();
        data.floatProperty = (float)random.NextDouble();

        data.boolField = random.Next(0, 1) == 0;
        data.boolProperty = random.Next(0, 1) == 0;

        data.dateTimeField = DateTime.Now;
        data.dateTimeProperty = DateTime.Now;

        data.guidField = Guid.NewGuid();
        data.guidProperty = Guid.NewGuid();

        data.typeField = GetRandomType();
        data.typeProperty = GetRandomType();

        data.stringField = GetRandomString(maxLen);
        data.stringProperty = GetRandomString(maxLen);

        data.listField = GetRandomList(maxLen);
        data.listProperty = GetRandomList(maxLen);

        data.array1DField = GetRandomInt1DArray(maxLen);
        data.array1DProperty = GetRandomInt1DArray(maxLen);

        int sqrtLen = (int)(Mathf.Sqrt(maxLen) + 1);
        data.array2DField = GetRandomShort2DArray(sqrtLen);
        data.array2DProperty = GetRandomShort2DArray(sqrtLen);

        data.enumField = GetRandomEnumValue();
        data.enumProperty = GetRandomEnumValue();

        data.setField = GetRandomSet(maxLen);
        data.setProperty = GetRandomSet(maxLen);

        data.dictionaryField = GetRandomDictionary(maxLen / 2);
        data.dictionaryProperty = GetRandomDictionary(maxLen / 2);

        data.tupleField = GetRandomTuple(maxLen);
        data.tupleProperty = GetRandomTuple(maxLen);
        
        data.objectField = GetRandomObject(maxLen);
        data.objectProperty = GetRandomObject(maxLen);
        
        return data;
    }
}
