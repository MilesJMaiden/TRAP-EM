using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject hudUI;

    private bool isPaused = false;
    private FirstPersonPlayerController playerController;
    private HUDUIManager hudUIManager;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the UIManager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pauseMenu.SetActive(false);
        // Find the player controller and HUD manager in the scene
        playerController = FindObjectOfType<FirstPersonPlayerController>();
        hudUIManager = FindObjectOfType<HUDUIManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void StartGame()
    {
        hudUI.SetActive(true);
        Time.timeScale = 1f; // Resume the game
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        hudUI.SetActive(false);
        Time.timeScale = 0f; // Pause the game

        // Set the paused state in the player controller
        if (playerController != null)
        {
            playerController.SetPaused(true);
        }

        // Disable HUD updates
        if (hudUIManager != null)
        {
            hudUIManager.DisableHUD();
        }

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        hudUI.SetActive(true);
        Time.timeScale = 1f; // Resume the game

        // Set the paused state in the player controller
        if (playerController != null)
        {
            playerController.SetPaused(false);
        }

        // Enable HUD updates
        if (hudUIManager != null)
        {
            hudUIManager.EnableHUD();
        }

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}