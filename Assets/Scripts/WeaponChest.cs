using UnityEngine;

public class WeaponChest : MonoBehaviour
{
    [Header("Checkpoint")]
    public string checkpointID;

    [Header("Optional Weapon")]
    public bool givesWeapon = false;
    public string weaponID;

    [Header("UI")]
    public GameObject pressEText;

    private bool isOpened = false;
    private bool playerOnTop = false;
    private GameObject player;

    void Start()
    {
        if (SaveManager.Instance == null)
            return;

        if (!string.IsNullOrEmpty(SaveManager.Instance.data.checkpointID) &&
            SaveManager.Instance.data.checkpointID == checkpointID)
        {
            isOpened = true;
        }
    }

    void Update()
    {
        if (playerOnTop && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isOpened) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        float playerBottomY = collision.collider.bounds.min.y;
        float chestTopY = GetComponent<Collider2D>().bounds.max.y;

        if (playerBottomY >= chestTopY - 0.02f)
        {
            playerOnTop = true;
            player = collision.gameObject;

            if (pressEText != null && !pressEText.activeSelf)
                pressEText.SetActive(true);
        }
        else
        {
            ClearPlayer();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ClearPlayer();
        }
    }

    void ClearPlayer()
    {
        playerOnTop = false;
        player = null;

        if (pressEText != null)
            pressEText.SetActive(false);
    }

    void OpenChest()
    {
        isOpened = true;

        // حفظ Checkpoint في JSON
        SaveManager.Instance.data.checkpointID = checkpointID;
        SaveManager.Instance.data.checkpointPosition = transform.position; // ✅ السطر الناقص

        if (givesWeapon && player != null && !string.IsNullOrEmpty(weaponID))
        {
            if (!SaveManager.Instance.data.ownedWeapons.Contains(weaponID))
            {
                SaveManager.Instance.data.ownedWeapons.Add(weaponID);

                PlayerWeapons pw = player.GetComponent<PlayerWeapons>();
                if (pw != null)
                    pw.AddWeapon(weaponID);
            }
        }

        SaveManager.Instance.Save();

        if (pressEText != null)
            pressEText.SetActive(false);

        Debug.Log("Checkpoint saved (JSON): " + checkpointID);
    }

}
