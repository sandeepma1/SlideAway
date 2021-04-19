using MarchingBytes;
using System;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    private Vector3 lastPosition;
    private float size;

    private void Awake()
    {
        BallController.OnGameOver += OnGameOver;
        UiStartPanel.OnGameStart += OnGameStart;
        BallController.OnSpawnPlatform += SpawnPlatform;
    }

    private void Start()
    {
        lastPosition = platform.transform.position;
        size = platform.transform.localScale.x;
        for (int i = 0; i < 7; i++)
        {
            SpawnPlatform();
        }
    }

    private void OnDestroy()
    {
        BallController.OnGameOver -= OnGameOver;
        UiStartPanel.OnGameStart -= OnGameStart;
        BallController.OnSpawnPlatform -= SpawnPlatform;
    }

    private void OnGameStart()
    {
        //InvokeRepeating("SpawnPlatform", 0.1f, 0.2f);
    }

    private void OnGameOver()
    {
        //CancelInvoke("SpawnPlatform");
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
        Spawn(pos);
    }

    private void SpawnZ()
    {
        Vector3 pos = lastPosition;
        pos.z += size;
        Spawn(pos);
    }

    private void Spawn(Vector3 pos)
    {
        if (UnityEngine.Random.Range(0, 2) < 1)
        {
            EasyObjectPool.instance.GetObjectFromPool("Diamond", pos, Quaternion.identity);
        }
        EasyObjectPool.instance.GetObjectFromPool("Platform", pos, Quaternion.identity);
        lastPosition = pos;
    }
}