using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSystem
{
    public static UnityEngine.Random.State gameState = new UnityEngine.Random.State();

    private static readonly string FILE_NAME = "/the_save.zzz";
    public static void SaveData(SaveData saveData)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + FILE_NAME;
        FileStream stream = new FileStream(path, FileMode.Create);


        SaveData data = saveData;

        binaryFormatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("PATH: "+path);

    }

    public static SaveData LoadData()
    {
        string path = Application.persistentDataPath + FILE_NAME;

        if(File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData retrievedData = binaryFormatter.Deserialize(stream) as SaveData;
            stream.Close();

            Debug.Log("RETRIEVED DATA IS NULL??: " + retrievedData);

            return retrievedData;
        }
        else
        {
            Debug.Log("PROBLEM Z DANYMI Z SAVE");
            return null;
        }

        
    }

}