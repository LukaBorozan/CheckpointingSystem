using SaveSystem;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public class Experiment12 : MonoBehaviour
{
    const string PATH = "D:/My Documents/luka/MIPRO2026";
    [SerializeField] private string FOLDER = "unity_instances";
    [SerializeField] private int COUNT = 10;

    private void Start()
    {
        GameObject prefab = Resources.Load<GameObject>("Hero");
        List<TestMonobehavior> heroes = new(COUNT * COUNT * COUNT);

        for (int i = 0; i < COUNT; ++i)
        {
            for (int j = 0; j < COUNT; ++j)
            {
                for (int k = 0; k < COUNT; ++k)
                {
                    Vector3 position = new(i, j, k);
                    GameObject obj = Instantiate(prefab, position, Quaternion.identity);
                    var component = obj.GetComponent<TestMonobehavior>();
                    component.Randomize();
                    heroes.Add(component);
                }
            }
        }

        string logPath = $"{PATH}/{FOLDER}/log.txt";

        if (File.Exists(logPath))
        {
            File.Delete(logPath);
        }

        using var writer = File.AppendText(logPath);
        string path = $"{PATH}/{FOLDER}/checkpoint.xml";

        var saveWatch = System.Diagnostics.Stopwatch.StartNew();
        SaveManager.BeginWrite(path);
        foreach (var data in heroes)
        {
            SaveManager.WriteSaveable(data);
            Destroy(data.gameObject);
        }
        SaveManager.EndWrite();
        saveWatch.Stop();

        heroes.Clear();

        for (int i = 0; i < COUNT; ++i)
        {
            for (int j = 0; j < COUNT; ++j)
            {
                for (int k = 0; k < COUNT; ++k)
                {
                    GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    var component = obj.GetComponent<TestMonobehavior>();
                    heroes.Add(component);
                }
            }
        }

        var loadWatch = System.Diagnostics.Stopwatch.StartNew();
        SaveManager.BeginRead(path);
        foreach (var data in heroes)
        {
            Type type = SaveManager.ReadNextSaveableType();
            SaveManager.ReadSaveable(data);
        }
        SaveManager.EndRead();
        loadWatch.Stop();

        writer.WriteLine($"{saveWatch.ElapsedMilliseconds / 1000.0} {loadWatch.ElapsedMilliseconds / 1000.0}");
        writer.Flush();
    }
}