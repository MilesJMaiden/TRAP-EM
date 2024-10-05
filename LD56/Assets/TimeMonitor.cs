using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMonitor : MonoBehaviour
{

    private bool value = false;
    private float elapsedTime = 0f;

    public float ElapsedTime => elapsedTime;

    // Update is called once per frame
    void Update()
    {
    }

    // Method to start monitoring time
    public void StartMonitoring()
    {
        StartCoroutine(MonitorTime());
    }

    // Coroutine to monitor time while value is false
    private IEnumerator MonitorTime()
    {
        elapsedTime = 0f;
        while (!value)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Elapsed Time: " + elapsedTime + " seconds");
        yield return elapsedTime;
    }

    // Example method to stop monitoring
    public void StopMonitoring()
    {
        value = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        value = false;
        StartMonitoring();
        // Simulate stopping the monitoring after 5 seconds
        //Invoke("StopMonitoring", 5f);
    }
}

