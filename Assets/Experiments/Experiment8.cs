using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using SaveSystem;

public class Experiment8 : MonoBehaviour
{
    const string PATH = "D:/My Documents/luka/MIPRO2026";
    [SerializeField] private string FOLDER = "data_variable_length";
    [SerializeField] private int STEP_COUNT = 10;
    [SerializeField] private int STEP_SIZE = 100;
    [SerializeField] private int MAX_LEN = 1000;

    private void Start()
    {
        List<(int, int)> experiments = new();
        TestData blank = new();

        for (int i = STEP_SIZE; i <= STEP_COUNT * STEP_SIZE; i += STEP_SIZE)
        {
            experiments.Add((MAX_LEN, i));
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
            List<TestData> saveDatas = new();
            List<TestData> loadDatas = new();

            writer.WriteLine($"{i + 1}: {maxLen} {instanceNum}");

            long saveMemory = GC.GetTotalMemory(true);
            for (int j = 0; j < instanceNum; ++j)
            {
                saveDatas.Add(TestData.Randomize(maxLen));
            }
            saveMemory = GC.GetTotalMemory(true) - saveMemory;

            for (int j = 0; j < instanceNum; ++j)
            {
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

            writer.WriteLine($"{saveWatch.ElapsedMilliseconds / 1000.0} {loadWatch.ElapsedMilliseconds / 1000.0} {saveMemory / (1024.0 * 1024.0)}");
            writer.Flush();
        }
    }
}