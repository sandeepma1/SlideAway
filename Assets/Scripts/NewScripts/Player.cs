using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static Action OnGameOver;
    public static Action OnSpawnPlatform;
    public static Action OnUpdateScore;
    [SerializeField] private Renderer ballMeshRenderer;
    [SerializeField] private Light ballSpotLight;
    [SerializeField] private Material floorMaterial;
    //[SerializeField] private float fallSpeed;
    private float speed;
    [SerializeField] private bool started;
    private float scoreHue;
    private bool isTurnedLeft;
    private bool isDead;
    private Vector3 dir;
    private float playerYPos;

    private void Awake()
    {
        UiStartPanel.OnGameStart += OnGameStart;
        UiShopCanvas.OnBallMaterialChanged += OnBallMaterialChanged;
    }

    private void OnDestroy()
    {
        UiStartPanel.OnGameStart -= OnGameStart;
        UiShopCanvas.OnBallMaterialChanged -= OnBallMaterialChanged;
    }

    private void Start()
    {
        speed = AppData.minSpeed;
        started = false;
        AppData.currentScore = 0;
        UpdateScoreAndBallSpeed(0);
        isDead = false;
        dir = Vector3.zero;
        playerYPos = transform.position.y - 0.2f;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDead)
        {
            UpdateScoreAndBallSpeed(1);
            if (dir == Vector3.forward)
            {
                dir = Vector3.left;
            }
            else
            {
                dir = Vector3.forward;
            }
        }
        float amountToMove = speed * Time.deltaTime;
        transform.Translate(dir * amountToMove);
    }

    private void FixedUpdate()
    {
        if (transform.position.y < (playerYPos))
        {
            GameOver();
        }
    }

    private void LateUpdate()
    {
        //if (gameOver)
        //{
        //    return;
        //}
        if (started)
        {
            if (isTurnedLeft) // Rotate sphere as per direction
            {
                ballMeshRenderer.transform.Rotate(Time.deltaTime * (speed * 100), 0, 0, Space.World);
            }
            else
            {
                ballMeshRenderer.transform.Rotate(0, 0, Time.deltaTime * (speed * -100), Space.World);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            UpdateScoreAndBallSpeed(2);
            UiGemsSpawnCanvas.OnSpawnGem?.Invoke(transform);
        }
    }

    private void OnGameStart()
    {
        started = true;
    }

    private void GameOver()
    {
        isDead = true;
        OnGameOver?.Invoke();
        ballSpotLight.intensity = 0;
    }

    private void UpdateScoreAndBallSpeed(int adder)
    {
        AppData.currentScore += adder;
        OnUpdateScore?.Invoke();
        CheckAchievements();
        //Change color of floor
        scoreHue = Clamp0360((float)AppData.currentScore) / 360.0f;
        floorMaterial.color = Color.HSVToRGB(scoreHue, AppData.floorSaturation, AppData.floorLightness);
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
            case int n when (n >= AppData.achievementValue1 && n <= AppData.achievementValue1 + 3):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_first_50);
                break;
            case int n when (n >= AppData.achievementValue2 && n <= AppData.achievementValue2 + 3):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_century_100);
                break;
            case int n when (n >= AppData.achievementValue3 && n <= AppData.achievementValue3 + 3):
                GpsManager.Instance.UnlockAchievement(GPGSIds.achievement_next_250);
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

    private void OnBallMaterialChanged(Material material)
    {
        ballMeshRenderer.material = material;
    }

}