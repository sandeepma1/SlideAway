using System;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private bool isBallCrossed = false;
    public bool isSpawnedLeft;

    private void Start()
    {
        BallController.OnGameOver += OnGameOver;
        BallController.OnGameStart += OnGameStart;
    }

    private void OnDestroy()
    {
        BallController.OnGameOver -= OnGameOver;
        BallController.OnGameStart -= OnGameStart;
    }

    private void OnGameStart()
    {
        if (isBallCrossed)
        {
            GetComponentInParent<Rigidbody>().useGravity = true;
            GetComponentInParent<Rigidbody>().isKinematic = false;
        }
    }

    private void OnGameOver()
    {
        if (isBallCrossed)
        {
            GetComponentInParent<Rigidbody>().useGravity = false;
            GetComponentInParent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Ball")
        {
            isBallCrossed = true;
            BallController.OnLastDropPosition?.Invoke(transform.position, isSpawnedLeft);
            Invoke("FallDown", 0.2f);
        }
    }

    private void FallDown()
    {
        GetComponentInParent<Rigidbody>().useGravity = true;
        GetComponentInParent<Rigidbody>().isKinematic = false;
    }
}