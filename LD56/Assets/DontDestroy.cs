using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("PauseMenu instance created");
            DontDestroyOnLoad(gameObject); // Optional: Keeps the UIManager across scenes
        }
        else if (Instance != this)
        {
            Debug.Log("Destroying duplicate PauseMenu instance");
            Destroy(gameObject);
        }
    }
}
