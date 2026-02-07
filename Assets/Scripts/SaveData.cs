using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string checkpointID;

    // 🔹 موقع آخر تشيك بوينت
    public Vector3 checkpointPosition;

    // اللاعب
    public int playerHP;
    public List<string> ownedWeapons = new List<string>();

    // العالم
    public List<string> killedEnemies = new List<string>();
}
