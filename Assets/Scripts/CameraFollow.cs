using System;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private float lerpRate;
    private bool gameOver = false;
    private Vector3 offset;
    private const float normalYPos = 20;
    private const float shopYPos = 13;
    private const float animSpeed = 0.25f;

    private void Awake()
    {
        offset = ball.transform.position - transform.position;
        BallController.OnGameOver += OnGameOver;
        UiStartPanel.OnGameStart += OnGameStart;
        UiShopCanvas.OnIsShopMenuVisible += OnIsShopMenuVisible;
    }

    private void OnDestroy()
    {
        BallController.OnGameOver -= OnGameOver;
        UiStartPanel.OnGameStart -= OnGameStart;
        UiShopCanvas.OnIsShopMenuVisible -= OnIsShopMenuVisible;
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

    private void OnIsShopMenuVisible(bool isVisible)
    {
        gameOver = isVisible;
        if (isVisible)
        {
            transform.DOMoveY(shopYPos, animSpeed);
        }
        else
        {
            transform.DOMoveY(normalYPos, animSpeed);
        }
    }

    private void OnGameStart()
    {
        gameOver = false;
    }

    private void OnGameOver()
    {
        gameOver = true;
    }
}