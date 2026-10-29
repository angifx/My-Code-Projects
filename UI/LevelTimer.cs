using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Header("Zeit")]
    public float startTime = 300f; // 5 Minuten
    private float currentTime;
    private bool isRunning = true;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Warning")]
    public float warningThreshold = 10f; 
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float pulseSpeed = 4f;

    [Header("Game Over")]
    public GameController gameController;

    void Start()
    {
        currentTime = startTime;
        UpdateUI();
    }

    void OnEnable()
    {
        GameController.OnReset += ResetTimer;
        GameController.OnLevelLoad += ResetTimer;
    }

    void OnDisable()
    {
        GameController.OnReset -= ResetTimer;
        GameController.OnLevelLoad -= ResetTimer;
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            TriggerGameOver();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= warningThreshold && currentTime > 0f)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; 
            timerText.color = Color.Lerp(normalColor, warningColor, pulse);
        }
        else
        {
            timerText.color = normalColor;
        }
    }

    void TriggerGameOver()
    {
        if (timerText != null) timerText.color = warningColor; 
        if (gameController != null)
        {
            gameController.GameOverScreen();
        }
    }

    void ResetTimer()
    {
        currentTime = startTime;
        isRunning = true;
        if (timerText != null) timerText.color = normalColor;
        UpdateUI();
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
    }

    public void AddTime(float seconds)
    {
        currentTime += seconds;
    }
}