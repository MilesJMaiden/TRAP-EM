using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleBase : MonoBehaviour
{
    [SerializeField] private GameObject consoleAText;

    public Inventory inventory;
    bool EKeyIsActive;
    private bool playerInside;

    

    // Start is called before the first frame update
    void Start()
    {
        // inventory = FindObjectOfType<Inventory>();
        playerInside = false;
        EKeyIsActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInside)
        {
            EKeyIsActive = true;
        }
    }

    public virtual void releaseNPC()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        consoleAText.SetActive(true);
        playerInside = true;
        Debug.Log("Collision detected with: " + other.name);

    }

    private void OnTriggerExit(Collider other)
    {
        consoleAText.SetActive(false);
        playerInside = false;
        Debug.Log("Collision ended with: " + other.name);
    }

    public void OnTriggerStay(Collider other)
    {
        if (EKeyIsActive)
        {
            Debug.Log("ConsoleA and E pressed");
            // inventory.ReleaseNPCA();
            releaseNPC();
        }
        EKeyIsActive = false;
    }
}
