using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Fields to store the counts of the three unique NPCs
    private int npcACount;
    private int npcBCount;
    private int npcCCount;

    // Reference to the hidden GameObject
    public GameObject hiddenObject;

    // Start is called before the first frame update
    void Start()
    {
        npcACount = 0;
        npcBCount = 0;
        npcCCount = 0;

        if (hiddenObject != null)
        {
            hiddenObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to capture an NPC
    public void CaptureNPC(int npcType)
    {
        switch (npcType)
        {
            case 1:
                npcACount++;
                break;
            case 2:
                npcBCount++;
                break;
            case 3:
                npcCCount++;
                break;
            default:
                Debug.LogWarning("Invalid NPC type");
                break;
        }
    }

    // Method to release an NPC and activate the hidden object
    private void ReleaseNPCA()
    {
        // Example logic to deplete NPCA
        if (npcACount > 0)
        {
            npcACount--;
            if (hiddenObject != null)
            {
                hiddenObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("No NPCA to deplete");
        }
    }
    // Method to release an NPC and activate the hidden object
    private void ReleaseNPCB()
    {
        // Example logic to deplete NPCA
        if (npcBCount > 0)
        {
            npcBCount--;
            if (hiddenObject != null)
            {
                hiddenObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("No NPCB to deplete");
        }
    }
    // Method to release an NPC and activate the hidden object
    private void ReleaseNPCC()
    {
        // Example logic to deplete NPCA
        if (npcCCount > 0)
        {
            npcCCount--;
            if (hiddenObject != null)
            {
                hiddenObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("No NPCC to deplete");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) && other.CompareTag("ConsoleA"))
        {
            ReleaseNPCA();
        }
        if (Input.GetKeyDown(KeyCode.E) && other.CompareTag("ConsoleB"))
        {
            ReleaseNPCA();
        }
        if (Input.GetKeyDown(KeyCode.E) && other.CompareTag("ConsoleC"))
        {
            ReleaseNPCA();
        }
    }
}