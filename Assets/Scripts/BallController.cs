using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static Action OnGameStart;
    public static Action OnGameOver;
    public static Action<int> OnScoreUpdated;
    public static Action<Vector3, bool> OnLastDropPosition;
    [SerializeField] private Transform sphearBallMesh;
    [SerializeField] private Light ballSpotLight;
    [SerializeField] private GameObject particle;
    [SerializeField] private float fallSpeed;
    [SerializeField] private Material floorMaterial;
    private bool started;
    private bool gameOver;
    private Rigidbody rb;
    private int score;
    private Vector3 lastDropPosition;
    private bool isSpawnedLeft;
    private float scoreHue;
    [SerializeField] private float speed = 8;
    private const float minSpeed = 8;
    private const float maxSpeed = 12;
    private bool isTurnedLeft;

    private void Awake()
    {
        UiContinuePanel.OnResumeStart += OnResumeStart;
        UiContinuePanel.OnResumed += OnResumed;
        OnLastDropPosition += LastDropPosition;
    }

    private void Start()
    {
        speed = minSpeed;
        rb = GetComponent<Rigidbody>();
        started = false;
        gameOver = false;
        UpdateScoreAndBallSpeed(0);
    }

    private void OnDestroy()
    {
        UiContinuePanel.OnResumeStart -= OnResumeStart;
        UiContinuePanel.OnResumed -= OnResumed;
        OnLastDropPosition -= LastDropPosition;
    }

    private void Update()
    {
        if (!started)
        {
            if (Input.GetMouseButtonDown(0))
            {
                rb.velocity = new Vector3(speed, 0, 0);
                started = true;
                OnGameStart?.Invoke();
            }
        }
        else
        {
            if (isTurnedLeft)
            {
                sphearBallMesh.Rotate(Time.deltaTime * (speed * 100), 0, 0, Space.World);
            }
            else
            {
                sphearBallMesh.Rotate(0, 0, Time.deltaTime * (speed * -100), Space.World);
            }
        }

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

    private void LastDropPosition(Vector3 lastDropPosition, bool isSpawnedLeft)
    {
        this.isSpawnedLeft = isSpawnedLeft;
        this.lastDropPosition = lastDropPosition;
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
        gameOver = true;
        rb.velocity = new Vector3(0, -fallSpeed, 0);
        ballSpotLight.intensity = 0;
        if (PlayerPrefs.HasKey("highScore"))
        {
            if (score > PlayerPrefs.GetInt("highScore"))
            {
                PlayerPrefs.SetInt("highScore", score);
            }
        }
        else
        {
            PlayerPrefs.SetInt("highScore", score);
        }
    }

    private void OnResumeStart()
    {
        rb.velocity = new Vector3(0, 0, 0);
        gameObject.transform.position = new Vector3(lastDropPosition.x, lastDropPosition.y + 2, lastDropPosition.z);
    }

    private void OnResumed()
    {
        gameOver = false;
        ballSpotLight.intensity = 1;
        OnGameStart?.Invoke();
        if (isSpawnedLeft)
        {
            rb.velocity = new Vector3(speed, 0, 0);
        }
        else
        {
            rb.velocity = new Vector3(0, 0, speed);
        }
    }

    private void SwitchDirections()
    {
        if (rb.velocity.z > 0)//right
        {
            isTurnedLeft = false;
            rb.velocity = new Vector3(speed, 0, 0);
        }
        else if (rb.velocity.x > 0)//left
        {
            isTurnedLeft = true;
            rb.velocity = new Vector3(0, 0, speed);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Diamond"))
        {
            GameObject part = Instantiate(particle, collider.gameObject.transform.position, Quaternion.identity);
            Destroy(collider.gameObject);
            Destroy(part, 1.0f);
            UpdateScoreAndBallSpeed(2);
        }
    }

    private void UpdateScoreAndBallSpeed(int adder)
    {
        score += adder;
        OnScoreUpdated?.Invoke(score);
        scoreHue = Clamp0360((float)score) / 360.0f;
        floorMaterial.color = Color.HSVToRGB(scoreHue, 0.8f, 0.75f);
        if (speed <= maxSpeed)
        {
            speed = ((float)score / 100.0f) + minSpeed;
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