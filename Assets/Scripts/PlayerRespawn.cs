using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    public Transform defaultSpawnPoint;
    public float respawnYOffset = 1.2f;

    void Start()
    {
        StartCoroutine(RespawnAfterLoad());
    }

    IEnumerator RespawnAfterLoad()
    {
        yield return null;
        RespawnPlayer();
    }

    void RespawnPlayer()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager not found!");
            return;
        }

        SaveData data = SaveManager.Instance.data;

        if (data != null && data.checkpointPosition != Vector3.zero)
        {
            transform.position = data.checkpointPosition + Vector3.up * respawnYOffset;
            Debug.Log("Respawned above checkpoint");
            return;
        }

        if (defaultSpawnPoint != null)
        {
            transform.position = defaultSpawnPoint.position;
            Debug.Log("Respawned at default spawn point");
        }
    }
}
