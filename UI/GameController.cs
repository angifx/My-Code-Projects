using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine; 

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;
    public TMP_Text progressText;

    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;
    private Coroutine progressBarCoroutine;


    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    private int survivedLevelsCount;

    public static event Action OnReset;
    public static event Action OnLevelLoad;

    public List<Transform> spawnPoints;
    public List<Collider2D> confinerShapes; 

    public CinemachineCamera cinemachineCamera;
    private Rigidbody2D playerRb;

    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        UpdateProgressText();
        playerRb = player.GetComponent<Rigidbody2D>();
        PlayerHealth.OnPlayerDied += GameOverScreen; 
        gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        BossControllerr.OnBossDefeated += WinGame; 
        gameOverScreen.SetActive(false); 
    }

    public void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        MusicManager.PauseBackgroundMusic(); 
        Time.timeScale = 0; 
        string message = "FINISHED " + survivedLevelsCount + " LEVEL";
        if (survivedLevelsCount != 1) message += "S";

        if (survivedText != null)
        {
            survivedText.text = message;
        }
        else
        {
            Debug.Log("GameOverScreen() wurde aufgerufen!");
        }
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        MusicManager.PlayBackgroundMusic(true); 
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset.Invoke();
        Time.timeScale = 1; 
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        if (progressBarCoroutine != null)
            StopCoroutine(progressBarCoroutine);

        if (loadCanvas != null)
            loadCanvas.SetActive(false);

        levels[level].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = spawnPoints[level].position;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        var confiner = cinemachineCamera.GetComponent<Unity.Cinemachine.CinemachineConfiner2D>();

        if (confiner != null && confinerShapes != null && level < confinerShapes.Count)
        {
            confiner.BoundingShape2D = confinerShapes[level];
            confiner.InvalidateBoundingShapeCache();
        }

        currentLevelIndex = level;

        progressAmount = 0;
        progressSlider.value = 0;
        UpdateProgressText();

        if (wantSurvivedIncrease)
            survivedLevelsCount++;

        OnLevelLoad?.Invoke();
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex, false);
    }
    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        UpdateProgressText(); 

        if (progressBarCoroutine != null)
            StopCoroutine(progressBarCoroutine);

        progressBarCoroutine = StartCoroutine(AnimateProgressBar(progressAmount));

        if (progressAmount >= 25)
        {
            LoadNextLevel();
        }
    }

    private IEnumerator AnimateProgressBar(int targetValue)
    {
        float startValue = progressSlider.value;
        float elapsed = 0f;
        float duration = 0.5f; 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            progressSlider.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            yield return null;
        } 

        progressSlider.value = targetValue;
    }

    void UpdateProgressText()
    {
        if (progressText != null)
            progressText.text = $"{progressAmount}/{(int)progressSlider.maxValue}";
    }

    public void WinGame()
    {
        survivedLevelsCount++;
        gameOverScreen.SetActive(true);
        MusicManager.PauseBackgroundMusic();
        Time.timeScale = 0;

        string message = "FINISHED! LEVEL 1";

        if (survivedText != null)
        {
            survivedText.text = message;
        }
        else
        {
            Debug.Log("WinGame() wurde aufgerufen!");
        }
    }
    void OnDestroy()
    {
        BossControllerr.OnBossDefeated -= WinGame;
    }
}