using System;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] private Platform platformPrefab;
    [SerializeField] private Platform platformDiamondPrefab;
    [SerializeField] private GameObject platform;
    private Vector3 lastPosition;
    private float size;

    private void Awake()
    {
        BallController.OnGameOver += OnGameOver;
        BallController.OnGameStart += OnGameStart;
    }

    private void Start()
    {
        lastPosition = platform.transform.position;
        size = platform.transform.localScale.x;
        for (int i = 0; i < 10; i++)
        {
            SpawnPlatform();
        }
    }

    private void OnDestroy()
    {
        BallController.OnGameOver -= OnGameOver;
        BallController.OnGameStart -= OnGameStart;
    }

    private void OnGameStart()
    {
        InvokeRepeating("SpawnPlatform", 0.1f, 0.2f);
    }

    private void OnGameOver()
    {
        CancelInvoke("SpawnPlatform");
    }

    private void SpawnPlatform()
    {
        int rand = UnityEngine.Random.Range(0, 6); // 0 to 5
        if (rand < 3)
        {
            SpawnX();
        }
        else if (rand >= 3)
        {
            SpawnZ();
        }
    }

    private void SpawnX()
    {
        Vector3 pos = lastPosition;
        pos.x += size;
        Spawn(pos, false);
    }

    private void SpawnZ()
    {
        Vector3 pos = lastPosition;
        pos.z += size;
        Spawn(pos, true);
    }

    private void Spawn(Vector3 pos, bool isSpawnedLeft)
    {
        Platform platform;
        if (UnityEngine.Random.Range(0, 4) < 1)
        {
            platform = Instantiate(platformDiamondPrefab, pos, Quaternion.identity);
        }
        else
        {
            platform = Instantiate(platformPrefab, pos, Quaternion.identity);
        }
        platform.isSpawnedLeft = isSpawnedLeft;
        lastPosition = pos;
    }
}