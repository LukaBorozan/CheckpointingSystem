using SaveSystem;
using System;
using UnityEngine;

/*
Extended correctness test. 
*/
public class Experiment2 : MonoBehaviour
{
    [SerializeField]
    private int NUMBER_OF_EXPERIMENTS = 10000; 

    private void Start()
    {
        string path = $"{Application.persistentDataPath}/Experiment2.xml";
        Debug.Log(path);

        TestData blank = new();
        bool correct = true;

        for (int i = 0; i < NUMBER_OF_EXPERIMENTS; ++i)
        {
            TestData saveData = TestData.Randomize();
            TestData loadData = new();

            SaveManager.BeginWrite(path);
            SaveManager.WriteSaveable(saveData, blank);
            SaveManager.EndWrite();

            SaveManager.BeginRead(path);
            Type type = SaveManager.ReadNextSaveableType();
            SaveManager.ReadSaveable(loadData);
            SaveManager.EndRead();

            if (!TestDataComparer.Equals(saveData, loadData))
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            Debug.Log($"CORRECT!");
        }
        else
        {
            Debug.Log($"INCORRECT!");
        }  
    }
}