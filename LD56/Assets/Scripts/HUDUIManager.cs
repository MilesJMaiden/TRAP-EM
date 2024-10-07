using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUDUIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timeText;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private Slider cooldownSlider;

    [SerializeField]
    private float initialScore = 10000f;
    private float score;
    private float elapsedTime = 0f;
    private bool isMonitoringTime = false;
    private bool isGameOver = false;

    private Inventory inventory;

    public static HUDUIManager Instance { get; private set; }

    public static bool ShouldResetHUD { get; set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("HUDUIManager instance created");
            DontDestroyOnLoad(gameObject); // Optional: Keeps the UIManager across scenes
        }
        else if (Instance != this)
        {
            Debug.Log("Destroying duplicate HUDUIManager instance");
            Destroy(gameObject);
        }

        if (!isGameOver)
        {
            Instance.gameObject.SetActive(true);
        }

    }

    void OnEnable()
    {
        FirstPersonPlayerController.OnGrappleUsed += HandleGrappleUsed;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        FirstPersonPlayerController.OnGrappleUsed -= HandleGrappleUsed;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory component not found in the scene.");
        }
        score = initialScore;
        StartMonitoringTime();
    }

    void Update()
    {
        if (isMonitoringTime)
        {
            elapsedTime += Time.deltaTime;
            CalculateScore();
            UpdateHUD();
        }

        // Debug.Log("All NPCs released: " + AllNPCsReleased() + "isGameOver: " + isGameOver);
        if (AllNPCsReleased() && !isGameOver)
        {
            Debug.Log("End of game reached");
            StopMonitoringTime();
            Debug.Log("Score: " + Mathf.Round(score));
            isGameOver = true;
            timeText.text = "Final Time: " + elapsedTime.ToString("F2");
            scoreText.text = "Final Score: " + Mathf.Round(score).ToString();
            // TransitionToGameOverMenu();
        }

    }

    private void UpdateHUD()
    {
        timeText.text = "Time: " + elapsedTime.ToString("F2");
        scoreText.text = "Score: " + Mathf.Round(score).ToString();
    }

    private void StartMonitoringTime()
    {
        Debug.Log("Monitoring time should start: " + isMonitoringTime);
        elapsedTime = 0f;
        isMonitoringTime = true;
        Debug.Log("Monitoring time started: " + isMonitoringTime);
        
    }

    private void StartMonitoringTimeWithoutReset()
    {
        Debug.Log("Monitoring time should start without reset: " + isMonitoringTime);
        isMonitoringTime = true;
        Debug.Log("Monitoring time started without reset: " + isMonitoringTime);
    }

    private void StopMonitoringTime()
    {
        isMonitoringTime = false;
    }

    private void CalculateScore()
    {
        // 1250 seconds is the maximum time allowed before the score decreases to 0
        score = initialScore - (elapsedTime * 16.6f);
    }
    private void HandleGrappleUsed()
    {
        if (cooldownSlider != null)
        {
            cooldownSlider.value = 0;
        }
    }
    private void HandleDashUsed()
    {
        if (cooldownSlider != null)
        {
            cooldownSlider.value = 0;
        }
    }
    public void EnableHUD()
    {
        gameObject.SetActive(true);
    }

    public void DisableHUD()
    {
        gameObject.SetActive(false);
    }

    private bool AllNPCsReleased()
    {
        return inventory.ReleasedNPCA == 3 && inventory.ReleasedNPCB == 3 && inventory.ReleasedNPCC == 3;
    }
    public void ResetHUD()
    {
        score = initialScore;
        elapsedTime = 0f;
        isMonitoringTime = false;
        isGameOver = false;
        UpdateHUD();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            ResetHUD();
            StartMonitoringTime();
        }
    }
    private void TransitionToGameOverMenu()
    {
        // Store the score in a static variable or a GameManager
        // GameOverManager.FinalScore = Mathf.Round(score);
        isGameOver = true;
        // inventory.ReleasedNPCA = 0;
        // Load the GameOverMenuScene
        // SceneManager.LoadScene("GameOverScene");
    }

    
}
