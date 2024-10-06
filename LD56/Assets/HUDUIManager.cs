using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private Inventory inventory;

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

        if (AllNPCsReleased())
        {
            StopMonitoringTime();
            Debug.Log("Score: " + Mathf.Round(score));
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
}
