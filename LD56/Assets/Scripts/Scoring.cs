using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Scoring : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float time = 0f;
    [SerializeField]
    private float Score = 10000f;
    public TimeMonitor timer;
    private Inventory inventory;

    void Start()
    {
        timer = gameObject.AddComponent<TimeMonitor>();
        inventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AllNPCsReleased())
        {
            timer.StopMonitoring();
            time = timer.ElapsedTime;
            // 1250 seconds is the maximum time allowed before the score decreases to 0
            Score = Score - (time * 8);
            Debug.Log("Score: " + System.MathF.Round(Score));
        }
        
    }

    private bool AllNPCsReleased()
    {
        return inventory.ReleasedNPCA == 3 && inventory.ReleasedNPCB == 3 && inventory.ReleasedNPCC == 3;
    }
}
