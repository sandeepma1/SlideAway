using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static Action OnGameOver;
    public static Action OnSpawnPlatform;
    public static Action OnUpdateScore;
    public static Action OnBallSwitchDirection;
    public static Action OnBallGemTouched;
    [SerializeField] private Renderer ballMeshRenderer;
    [SerializeField] private Light ballSpotLight;
    [SerializeField] private SpriteRenderer plus3TextSprite;
    private float speed;
    private bool isGameStarted;
    // private float scoreHue;
    private bool isTurnedLeft;
    private bool isDead;
    private Vector3 dir;
    private float playerYPos;
    private Vector3 normalPos = new Vector3(0, 1.5f, 0);
    private Vector3 shopZoomPos = new Vector3(-6, 1.5f, 6);

    private void Awake()
    {
        UiShopCanvas.OnIsShopMenuVisible += OnIsShopMenuVisible;
        UiStartCanvas.OnGameStart += OnGameStart;
        UiShopCanvas.OnBallChanged += OnBallMaterialChanged;
    }

    private void OnDestroy()
    {
        UiShopCanvas.OnIsShopMenuVisible -= OnIsShopMenuVisible;
        UiStartCanvas.OnGameStart -= OnGameStart;
        UiShopCanvas.OnBallChanged -= OnBallMaterialChanged;
    }

    private void Start()
    {
        speed = AppData.minSpeed;
        isGameStarted = false;
        AppData.currentScore = 0;
        UpdateScoreAndBallSpeed(0);
        isDead = false;
        dir = Vector3.zero;
        playerYPos = transform.position.y - 0.2f;
    }

    private void Update()
    {
        if (!isGameStarted)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && !isDead)
        {
            OnBallSwitchDirection?.Invoke();
            UpdateScoreAndBallSpeed(1);
            if (dir == Vector3.forward)
            {
                dir = Vector3.left;
                isTurnedLeft = false;
            }
            else
            {
                dir = Vector3.forward;
                isTurnedLeft = true;
            }
        }
        transform.Translate(dir * (speed * Time.deltaTime));
    }

    private void FixedUpdate()
    {
        if (transform.position.y < (playerYPos))
        {
            if (!isDead)
            {
                isDead = true;
                GameOver();
            }
        }
    }

    private void LateUpdate()
    {
        if (isGameStarted)
        {
            if (isTurnedLeft) // Rotate sphere as per direction
            {
                ballMeshRenderer.transform.Rotate(Time.deltaTime * (speed * 120), 0, 0, Space.World);
            }
            else
            {
                ballMeshRenderer.transform.Rotate(0, 0, Time.deltaTime * (speed * 120), Space.World);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            OnBallGemTouched?.Invoke();
            other.gameObject.SetActive(false);
            UpdateScoreAndBallSpeed(2);
            UiGemsSpawnCanvas.OnSpawnSingleGem3D?.Invoke(transform);
            StartCoroutine(ShowPlus3Text());
        }
    }

    private Vector3 restPosition = new Vector3(0, 1, 0);
    private IEnumerator ShowPlus3Text()
    {
        plus3TextSprite.transform.localPosition = restPosition;
        plus3TextSprite.DOFade(1, 0.1f);
        plus3TextSprite.transform.DOLocalMoveY(2, 0.5f).OnComplete(() => plus3TextSprite.DOFade(0, 0.15f));

        yield return new WaitForEndOfFrame();
    }

    private void OnGameStart()
    {
        isGameStarted = true;
        dir = Vector3.left;
    }

    private void GameOver()
    {
        AnalyticsManager.GameOver();
        OnGameOver?.Invoke();
        ballSpotLight.intensity = 0;
    }

    private void UpdateScoreAndBallSpeed(int adder)
    {
        AppData.currentScore += adder;
        UiPlayerDataHud.OnUpdateScoreValue?.Invoke();
        CheckAchievements();
        //Change color of floor
        //scoreHue = Clamp0360((float)AppData.currentScore) / 360.0f;
        //floorMaterial.color = Color.HSVToRGB(scoreHue, AppData.floorSaturation, AppData.floorLightness);
        //Change speed of ball with speed
        if (speed <= AppData.maxSpeed)
        {
            speed = ((float)AppData.currentScore / 100.0f) + AppData.minSpeed;
        }
    }

    private void CheckAchievements()
    {
        switch (AppData.currentScore)
        {
            case int n when (n >= AppData.achievementValue1 && n <= AppData.achievementValue1 + 5):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_50);
                break;
            case int n when (n >= AppData.achievementValue2 && n <= AppData.achievementValue2 + 5):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_100);
                break;
            case int n when (n >= AppData.achievementValue3 && n <= AppData.achievementValue3 + 5):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_500);
                break;
            case int n when (n >= AppData.achievementValue4 && n <= AppData.achievementValue4 + 5):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_500);
                break;
            case int n when (n >= AppData.achievementValue5 && n <= AppData.achievementValue5 + 5):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_500);
                break;
            case int n when (n >= AppData.achievementValue6 && n <= AppData.achievementValue6 + 5):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_500);
                break;
            case int n when (n >= AppData.achievementValue7 && n <= AppData.achievementValue7 + 5):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_500);
                break;
            case int n when (n >= AppData.achievementValue8 && n <= AppData.achievementValue8 + 5):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_500);
                break;
            case int n when (n >= AppData.achievementValue9 && n <= AppData.achievementValue9 + 5):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_score_500);
                break;
            default:
                break;
        }
    }

    public float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
    }

    private void OnBallMaterialChanged(string id)
    {
        ballMeshRenderer.material = Resources.Load<Material>(AppData.allShopItemsMatPath + "/" + id);
    }

    private void OnIsShopMenuVisible(bool isShopVisible)
    {
        if (isShopVisible)
        {
            transform.DOMove(shopZoomPos, AppData.shopAnimSpeed);
        }
        else
        {
            transform.DOMove(normalPos, AppData.shopAnimSpeed);
        }
    }
}