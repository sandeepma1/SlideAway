using UnityEngine;
using System.Collections;
using System;

public class TileScript : MonoBehaviour
{
    private bool isGameOver = false;

    private void Awake()
    {
        BallController.OnGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        BallController.OnGameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        isGameOver = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TileManager.Instance.SpawnTile();
            StartCoroutine(PoolObject());
        }
    }

    private IEnumerator PoolObject()
    {
        yield return new WaitForSeconds(3);
        switch (gameObject.name)
        {
            case "LeftTile":
                TileManager.Instance.LeftTiles.Push(gameObject);
                break;

            case "TopTile":
                TileManager.Instance.TopTiles.Push(gameObject);
                break;
        }
        if (!isGameOver)
        {
            gameObject.SetActive(false);
        }
    }
}