using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using SaveSystem;

public class Experiment11 : MonoBehaviour
{
    const string PATH = "D:/My Documents/luka/MIPRO2026";
    [SerializeField] private string FOLDER = "nested_variable_instances";
    [SerializeField] private int STEP_COUNT = 100;
    [SerializeField] private int STEP_SIZE = 100;
    [SerializeField] private int LENGTH = 1000;

    private void Start()
    {
        List<(int, int)> experiments = new();
        TestNesting blank = new();

        for (int i = STEP_SIZE; i <= STEP_COUNT * STEP_SIZE; i += STEP_SIZE)
        {
            experiments.Add((LENGTH, i));
        }

        string logPath = $"{PATH}/{FOLDER}/log.txt";

        if (File.Exists(logPath))
        {
            File.Delete(logPath);
        }

        using var writer = File.AppendText(logPath);

        for (int i = 0; i < experiments.Count; ++i)
        {
            (int maxLen, int instanceNum) = experiments[i];
            string path = $"{PATH}/{FOLDER}/{maxLen}-{instanceNum}.xml";
            List<TestNesting> saveDatas = new();
            List<TestNesting> loadDatas = new();

            writer.WriteLine($"{i + 1}: {maxLen} {instanceNum}");

            for (int j = 0; j < instanceNum; ++j)
            {
                saveDatas.Add(TestNesting.Randomize(maxLen));
                loadDatas.Add(new());
            }

            var saveWatch = System.Diagnostics.Stopwatch.StartNew();
            SaveManager.BeginWrite(path);
            for (int j = 0; j < saveDatas.Count; ++j)
            {
                SaveManager.WriteSaveable(saveDatas[j]);
            }
            SaveManager.EndWrite();
            saveWatch.Stop();
            
            var loadWatch = System.Diagnostics.Stopwatch.StartNew();
            SaveManager.BeginRead(path);
            for (int j = 0; j < loadDatas.Count; ++j)
            {
                Type type = SaveManager.ReadNextSaveableType();
                SaveManager.ReadSaveable(loadDatas[j]);
            }
            SaveManager.EndRead();
            saveWatch.Stop();

            writer.WriteLine($"{saveWatch.ElapsedMilliseconds / 1000.0} {loadWatch.ElapsedMilliseconds / 1000.0}");
            writer.Flush();
        }
    }
}