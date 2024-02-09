using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [HideInInspector]public string filePath;
    public SaveData save;

    void Awake()
    {
        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        Debug.Log(filePath);
        save = new SaveData();
    }

    // 省略。以下のSave関数やLoad関数を呼び出して使用すること
    public void Save()
    {
        Debug.Log("セーブ開始");
        string json = JsonUtility.ToJson(save);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("セーブ完了");
    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            Debug.Log("ロード開始");
            StreamReader streamReader;
            streamReader = new StreamReader(filePath);
            string data = streamReader.ReadToEnd();
            streamReader.Close();
            save = JsonUtility.FromJson<SaveData>(data);
            Debug.Log("ロード終了");
        }
    }
}