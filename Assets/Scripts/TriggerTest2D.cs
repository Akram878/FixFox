using UnityEngine;

public class TriggerTest2D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER HIT BY: " + other.name);
    }
}
