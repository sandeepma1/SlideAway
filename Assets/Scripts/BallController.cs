using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static Action OnGameOver;
    public static Action OnSpawnPlatform;
    public static Action OnUpdateScore;
    [SerializeField] private Transform sphearBallMesh;
    [SerializeField] private Light ballSpotLight;
    [SerializeField] private GameObject particle;
    [SerializeField] private Material floorMaterial;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float speed;
    private bool started;
    private bool gameOver;
    private Rigidbody rb;
    private float scoreHue;

    private bool isTurnedLeft;
    private int pos;
    private int tempPos;

    private void Awake()
    {
        UiStartPanel.OnGameStart += OnGameStart;
    }

    private void Start()
    {
        speed = AppData.minSpeed;
        rb = GetComponent<Rigidbody>();
        started = false;
        gameOver = false;
        AppData.currentScore = 0;
        UpdateScoreAndBallSpeed(0);
    }

    private void OnDestroy()
    {
        UiStartPanel.OnGameStart -= OnGameStart;
    }

    private void OnGameStart()
    {
        rb.velocity = new Vector3(speed, 0, 0);
        started = true;
        InvokeRepeating("VeryLateUpdate", 0.1f, 0.1f);
    }

    private void Update()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, 1.0f) && !gameOver)
        {
            GameOver();
        }

        if (Input.GetMouseButtonDown(0) && !gameOver)
        {
            SwitchDirections();
            UpdateScoreAndBallSpeed(1);
        }
    }

    private void LateUpdate()
    {
        if (gameOver)
        {
            return;
        }
        if (started)
        {
            if (isTurnedLeft) // Rotate sphere as per direction
            {
                sphearBallMesh.Rotate(Time.deltaTime * (speed * 100), 0, 0, Space.World);
                rb.velocity = new Vector3(0, 0, speed);
            }
            else
            {
                sphearBallMesh.Rotate(0, 0, Time.deltaTime * (speed * -100), Space.World);
                rb.velocity = new Vector3(speed, 0, 0);
            }
        }
    }

    private void VeryLateUpdate()
    {
        pos = ((int)transform.position.x + (int)transform.position.z) / 2;
        if (pos > tempPos)
        {
            OnSpawnPlatform?.Invoke();
            tempPos = pos;
        }
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
        gameOver = true;
        rb.velocity = new Vector3(0, -fallSpeed, 0);
        ballSpotLight.intensity = 0;

        StopCoroutine(OnGameOverCoroutine());
        StartCoroutine(OnGameOverCoroutine());
    }

    private IEnumerator OnGameOverCoroutine()
    {
        yield return new WaitForSeconds(3);
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        transform.position = new Vector3(0, 100, 0);
    }

    private void SwitchDirections()
    {
        if (rb.velocity.z > 0)//right
        {
            isTurnedLeft = false;
        }
        else if (rb.velocity.x > 0)//left
        {
            isTurnedLeft = true;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Diamond"))
        {
            collider.gameObject.SetActive(false);
            UpdateScoreAndBallSpeed(2);
            UiGemsSpawnCanvas.OnSpawnGem?.Invoke(transform);
        }
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
}