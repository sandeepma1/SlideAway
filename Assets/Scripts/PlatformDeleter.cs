using UnityEngine;

public class PlatformDeleter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Ball"))
        {
            Destroy(other.gameObject);
        }
    }
}