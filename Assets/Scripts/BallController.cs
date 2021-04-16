using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static Action OnGameStart;
    public static Action OnGameOver;
    public static Action<int> OnScoreUpdated;
    public static Action<Vector3, bool> OnLastDropPosition;
    [SerializeField] private Light ballSpotLight;
    [SerializeField] private GameObject particle;
    [SerializeField] private float speed;
    [SerializeField] private float fallSpeed;
    private bool started;
    private bool gameOver;
    private Rigidbody rb;
    private int score;
    private Vector3 lastDropPosition;
    private bool isSpawnedLeft;

    private void Awake()
    {
        UiManager.OnResumeStart += OnResumeStart;
        UiManager.OnResumed += OnResumed;
        OnLastDropPosition += LastDropPosition;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        started = false;
        gameOver = false;
        score = 0;
        OnScoreUpdated?.Invoke(score);
        PlayerPrefs.SetInt("score", score);
    }

    private void OnDestroy()
    {
        UiManager.OnResumeStart -= OnResumeStart;
        UiManager.OnResumed -= OnResumed;
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

        if (!Physics.Raycast(transform.position, Vector3.down, 1.0f))
        {
            GameOver();
        }

        if (Input.GetMouseButtonDown(0) && !gameOver)
        {
            SwitchDirections();
            score++;
            OnScoreUpdated?.Invoke(score);
        }
    }

    private void LastDropPosition(Vector3 lastDropPosition, bool isSpawnedLeft)
    {
        this.isSpawnedLeft = isSpawnedLeft;
        this.lastDropPosition = lastDropPosition;
    }

    private void GameOver()
    {
        gameOver = true;
        rb.velocity = new Vector3(0, -fallSpeed, 0);
        ballSpotLight.intensity = 0;
        PlayerPrefs.SetInt("score", score);
        OnGameOver?.Invoke();
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
        if (rb.velocity.z > 0)
        {
            rb.velocity = new Vector3(speed, 0, 0);
        }
        else if (rb.velocity.x > 0)
        {
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
            score += 2;
            OnScoreUpdated?.Invoke(score);
        }
    }
}