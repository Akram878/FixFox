using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;
    public SaveData data = new SaveData();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.dataPath + "/save.json";
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // حفظ للملف
    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved");
    }

    // تحميل الملف
    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded");
        }
        else
        {
            data = new SaveData();
            Debug.Log("New save created");
        }
    }

    // حذف الحفظ (Restart Again)
    public void ResetSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);

        data = new SaveData();
        Debug.Log("Save reset");
    }
}
