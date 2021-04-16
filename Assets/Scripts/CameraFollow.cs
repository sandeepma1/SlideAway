using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private float lerpRate;
    private bool gameOver = false;
    private Vector3 offset;

    private void Awake()
    {
        offset = ball.transform.position - transform.position;
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
        gameOver = false;
    }

    private void OnGameOver()
    {
        gameOver = true;
    }

    private void Update()
    {
        if (!gameOver)
        {
            Vector3 pos = transform.position;
            Vector3 targetPos = ball.transform.position - offset;
            pos = Vector3.Lerp(pos, targetPos, lerpRate * Time.deltaTime);
            transform.position = pos;
        }
    }
}