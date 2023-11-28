using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    [HideInInspector] public SaveData data;
    string filepath;
    string fileName = "Data.json";


    private void Awake()
    {
        filepath = Application.dataPath + "/" + fileName;

        if (!File.Exists(filepath))
        {
            Save(data);
        }

        data = Load(filepath);
    }

    void Save (SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        StreamWriter wr = new StreamWriter(filepath, false);
        wr.Write(json);
        wr.Close();
    }

    SaveData Load (string path)
    {
        StreamReader rd = new StreamReader(path);
        string json = rd.ReadToEnd();
        rd.Close();

        return JsonUtility.FromJson<SaveData>  (json);
    }

    private void OnDestroy()
    {
        Save(data);
    }

    float seconds;

    private void Start()
    {
        seconds = 0;
    }


    private void Update()
    {   
        seconds += Time.deltaTime;
        if (Input.GetKey(KeyCode.R))
        {
#if UNITY_STANDALONE_WIN
            System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
            Application.Quit();
#endif
        }
    }
}
