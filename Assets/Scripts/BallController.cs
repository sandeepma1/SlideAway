using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static Action OnGameOver;
    public static Action OnSpawnPlatform;
    public static Action<int> OnScoreUpdated;
    public static Action OnAddDiamond;
    [SerializeField] private Transform sphearBallMesh;
    [SerializeField] private Light ballSpotLight;
    [SerializeField] private GameObject particle;
    [SerializeField] private Material floorMaterial;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float speed;
    private bool started;
    private bool gameOver;
    private Rigidbody rb;
    private int score;
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
        if (ZPlayerPrefs.HasKey(AppData.keyHighScore))
        {
            if (score > ZPlayerPrefs.GetInt(AppData.keyHighScore))
            {
                ZPlayerPrefs.SetInt(AppData.keyHighScore, score);
            }
        }
        else
        {
            ZPlayerPrefs.SetInt(AppData.keyHighScore, score);
        }
        StopCoroutine(OnGameOverCoroutine());
        StartCoroutine(OnGameOverCoroutine());
    }

    private IEnumerator OnGameOverCoroutine()
    {
        yield return new WaitForSeconds(3);
        rb.velocity = Vector3.zero;
        print("OnGameOverCoroutine");
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
            GameObject particles = Instantiate(particle, collider.gameObject.transform.position, Quaternion.identity);
            collider.gameObject.SetActive(false);
            Destroy(particles, 1.0f);
            UpdateScoreAndBallSpeed(2);
            OnAddDiamond?.Invoke();
        }
    }

    private void UpdateScoreAndBallSpeed(int adder)
    {
        score += adder;
        OnScoreUpdated?.Invoke(score);
        scoreHue = Clamp0360((float)score) / 360.0f;
        floorMaterial.color = Color.HSVToRGB(scoreHue, AppData.floorSaturation, AppData.floorLightness);
        if (speed <= AppData.maxSpeed)
        {
            speed = ((float)score / 100.0f) + AppData.minSpeed;
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