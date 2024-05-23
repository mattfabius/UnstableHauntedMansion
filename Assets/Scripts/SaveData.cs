using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int level;
    public float[] position;

    public SaveData()
    {
        position = new float[3];
    }

    public SaveData(int lvl, Vector3 pos)
    {
        level = lvl;

        position = new float[3];
        position[0] = pos.x;
        position[1] = pos.y;
        position[2] = pos.z;
    }
}
