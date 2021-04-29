using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour
{
    private bool isGameOver = false;

    private void Awake()
    {
        PlayerController.OnGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        PlayerController.OnGameOver -= OnGameOver;
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