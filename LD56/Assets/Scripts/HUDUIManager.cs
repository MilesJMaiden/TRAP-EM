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
    }

    void OnEnable()
    {
        FirstPersonPlayerController.OnGrappleUsed += HandleGrappleUsed;
    }

    void OnDisable()
    {
        FirstPersonPlayerController.OnGrappleUsed -= HandleGrappleUsed;
    }
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
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

        if (AllNPCsReleased() && !isGameOver)
        {
            StopMonitoringTime();
            Debug.Log("Score: " + Mathf.Round(score));
            isGameOver = true;
            TransitionToGameOverMenu();
        }
    }

    private void UpdateHUD()
    {
        timeText.text = "Time: " + elapsedTime.ToString("F2");
        scoreText.text = "Score: " + Mathf.Round(score).ToString();
    }

    private void StartMonitoringTime()
    {
        isMonitoringTime = true;
        elapsedTime = 0f;
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

    private void TransitionToGameOverMenu()
    {
        // Store the score in a static variable or a GameManager
        GameOverManager.FinalScore = Mathf.Round(score);
        // Load the GameOverMenuScene
        SceneManager.LoadScene("GameOverScene");
    }
}
