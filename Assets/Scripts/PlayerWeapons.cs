using UnityEngine;
using System.Collections.Generic;

public class PlayerWeapons : MonoBehaviour
{
    public List<string> ownedWeapons = new List<string>();

    public void AddWeapon(string weaponID)
    {
        if (!ownedWeapons.Contains(weaponID))
        {
            ownedWeapons.Add(weaponID);
            Debug.Log("Player obtained weapon: " + weaponID);
        }
    }
}
