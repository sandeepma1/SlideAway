using MarchingBytes;
using UnityEngine;

public class PlatformDeleter : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        EasyObjectPool.instance.ReturnObjectToPool(collision.gameObject);
    }
}