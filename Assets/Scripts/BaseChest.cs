using UnityEngine;

public abstract class BaseChest : MonoBehaviour
{
    protected bool isOpened = false;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (isOpened) return;

        if (other.CompareTag("Player"))
        {
            Open(other);
        }
    }

    protected virtual void Open(Collider player)
    {
        isOpened = true;

        // تعطيل الكوليدر بعد الفتح
        GetComponent<Collider>().enabled = false;

        Debug.Log("Chest opened");
    }
}
