using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private float lerpRate;
    private bool gameOver = false;
    private Vector3 offset;
    private Camera mainCamera;
    private const float normalYPos = 17;//17
    private const float shopYPos = 18f;
    private const float normalZoom = 15f;//15
    private const float shopZoom = 7;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        offset = ball.transform.position - transform.position;
        PlayerController.OnGameOver += OnGameOver;
        UiStartCanvas.OnGameStart += OnGameStart;
        UiShopCanvas.OnIsShopMenuVisible += OnIsShopMenuVisible;
    }

    private void OnDestroy()
    {
        PlayerController.OnGameOver -= OnGameOver;
        UiStartCanvas.OnGameStart -= OnGameStart;
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
            mainCamera.DOOrthoSize(shopZoom, AppData.shopAnimSpeed);
            transform.DOMoveY(shopYPos, AppData.shopAnimSpeed);
        }
        else
        {
            mainCamera.DOOrthoSize(normalZoom, AppData.shopAnimSpeed);
            transform.DOMoveY(normalYPos, AppData.shopAnimSpeed);
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