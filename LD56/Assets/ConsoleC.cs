using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConsoleC : MonoBehaviour
{
    [SerializeField] private GameObject consoleAText;

    private Inventory inventory;
    private bool isPlayerNearby;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();

        isPlayerNearby = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        consoleAText.SetActive(true);
        Debug.Log("Collision detected with: " + other.name);
        
    }

    private void OnTriggerExit(Collider other)
    {
        consoleAText.SetActive(false);
        Debug.Log("Collision ended with: " + other.name);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayerNearby)
        {
            Debug.Log("ConsoleA and E pressed");
            inventory.ReleaseNPCC();
        }
        isPlayerNearby = false;
    }
}
