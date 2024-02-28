using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    public string date;
    public string mode;
    public TMP_Text saveName;
    public Image saveImage;

    public void LoadSave()
    {
        CrossData.isSave = true;
        CrossData.date = date;
        CrossData.mode = mode;
        CrossData.saveData = LoadSaveData($"{Application.dataPath}\\saves\\{mode}\\{date}\\{date}_Spectra.json");
        SceneManager.LoadScene(mode == "2D" ? 1 : 3);
    }

    public void NewSave(string mode)
    {
        CrossData.isSave = false;
        CrossData.mode = mode;
        SceneManager.LoadScene(mode == "2D" ? 1 : 3);
    }

    private SaveData LoadSaveData(string savePath)
    {
        // Specify the path to your save file
        string filePath = Path.Combine(Application.dataPath, savePath);

        if (File.Exists(filePath))
        {
            // Read the entire file and store its content
            string jsonContent = File.ReadAllText(filePath);

            // Deserialize the JSON content to the SaveData class
            return JsonUtility.FromJson<SaveData>(jsonContent);
        }
        else
        {
            Debug.LogError("Save file not found");
            return null;
        }
    }

    public void DeleteSave()
    {
        string saveDirectory = Path.Combine(Application.dataPath, $"saves\\{mode}\\{date}");

        if (Directory.Exists(saveDirectory))
        {
            Directory.Delete(saveDirectory, true);
            Destroy(gameObject);
        }    
        else
        {
            Debug.LogError("Save directory not found.");
        }
    }
}
