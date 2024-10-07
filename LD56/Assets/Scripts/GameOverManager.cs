using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static float FinalScore { get; set; }

    [SerializeField]
    private TMP_Text finalScoreText;

    void Start()
    {
        // Display the final score
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + FinalScore.ToString();
        }

        // Unlock the cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        // Reset HUD values
        HUDUIManager.Instance.ResetHUD();
        // Reload the MainScene and reset all variables
        SceneManager.LoadScene("MainScene");
        
    }

    public void ExitToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenuScene");
    }
}
