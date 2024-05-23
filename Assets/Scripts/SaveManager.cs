using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    public static void Save(int slotNumber)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(GetFilePath(slotNumber));

        SaveData data = new SaveData();
        formatter.Serialize(file, data);
        file.Close();
    }
    
    public static SaveData Load(int slotNumber)
    {
        string filePath = GetFilePath(slotNumber);
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            SaveData data = (SaveData)formatter.Deserialize(file);
            file.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + filePath);
            return null;
        }
    }

    private static string GetFilePath(int slotNumber)
    {
        return Application.persistentDataPath + "/save" + slotNumber + ".dat";
    }
}