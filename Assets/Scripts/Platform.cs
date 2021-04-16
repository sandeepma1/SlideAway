using System;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool isSpawnedLeft;

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Ball")
        {
            BallController.OnLastDropPosition?.Invoke(transform.position, isSpawnedLeft);
        }
    }
}