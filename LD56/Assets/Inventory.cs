using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // Fields to store the counts of the three unique NPCs
    public int npcACount;
    public int npcBCount;
    public int npcCCount;
    private int ReleasedNPCA;
    private int ReleasedNPCB;
    private int ReleasedNPCC;

    [Header("Console A NPCs")]
    [SerializeField] private GameObject ConsoleANPC1;
    [SerializeField] private GameObject ConsoleANPC2;
    [SerializeField] private GameObject ConsoleANPC3;

    [Header("Console B NPCs")]
    [SerializeField] private GameObject ConsoleBNPC1;
    [SerializeField] private GameObject ConsoleBNPC2;
    [SerializeField] private GameObject ConsoleBNPC3;

    [Header("Console C NPCs")]
    [SerializeField] private GameObject ConsoleCNPC1;
    [SerializeField] private GameObject ConsoleCNPC2;
    [SerializeField] private GameObject ConsoleCNPC3;

    [Header("Console A Icons")]
    [SerializeField] private GameObject ConsoleAIcon1;
    [SerializeField] private GameObject ConsoleAIcon2;
    [SerializeField] private GameObject ConsoleAIcon3;

    [Header("Console B Icons")]
    [SerializeField] private GameObject ConsoleBIcon1;
    [SerializeField] private GameObject ConsoleBIcon2;
    [SerializeField] private GameObject ConsoleBIcon3;

    [Header("Console C Icons")]
    [SerializeField] private GameObject ConsoleCIcon1;
    [SerializeField] private GameObject ConsoleCIcon2;
    [SerializeField] private GameObject ConsoleCIcon3;
    
    // Start is called before the first frame update
    void Start()
    {
        npcACount = 0;
        npcBCount = 0;
        npcCCount = 0;

        ReleasedNPCA = 0;
        ReleasedNPCB = 0;
        ReleasedNPCC = 0;

        SetActiveStateForAllNPCs(false);
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
                Debug.Log("NPC A captured");
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

    // Method to release an NPC and activate the sleeper agents
    public void ReleaseNPCA()
    {
        //  logic to release NPCA
        if (npcACount > 0)
        {
            npcACount--;
            switch (ReleasedNPCA)
            {
                case 0:
                    {
                        ConsoleANPC1.SetActive(true);
                        SetImageColor(ConsoleAIcon1, Color.green);
                        ReleasedNPCA++;
                        break;
                    }
                case 1:
                    {
                        ConsoleANPC2.SetActive(true);
                        SetImageColor(ConsoleAIcon2, Color.green);
                        ReleasedNPCA++;
                        break;
                    }
                case 2:
                    {
                        ConsoleANPC3.SetActive(true);
                        SetImageColor(ConsoleAIcon3, Color.green);
                        ReleasedNPCA++;
                        break;
                    }
                default:
                    {
                        Debug.LogWarning("Invalid NPCA Value");
                        break;
                    }
            }
        }
        else
        {
            Debug.LogWarning("No NPCA to deplete");
        }
    }

    // Method to release an NPC and activate the sleeper agents
    public void ReleaseNPCB()
    {
        //  logic to release NPCB
        if (npcBCount > 0)
        {
            npcBCount--;
            switch (ReleasedNPCB)
            {
                case 0:
                    ConsoleBNPC1.SetActive(true);
                    SetImageColor(ConsoleBIcon1, Color.green);
                    ReleasedNPCB++;
                    break;
                case 1:
                    ConsoleBNPC2.SetActive(true);
                    SetImageColor(ConsoleBIcon2, Color.green);
                    ReleasedNPCB++;
                    break;
                case 2:
                    ConsoleBNPC3.SetActive(true);
                    SetImageColor(ConsoleBIcon3, Color.green);
                    ReleasedNPCB++;
                    break;
                default:
                    Debug.LogWarning("Invalid NPCB Value");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("No NPCB to deplete");
        }
    }

    // Method to release an NPC and activate the sleeper agents
    public void ReleaseNPCC()
    {
        //  logic to release NPCC
        if (npcCCount > 0)
        {
            npcCCount--;
            switch (ReleasedNPCC)
            {
                case 0:
                    ConsoleCNPC1.SetActive(true);
                    SetImageColor(ConsoleCIcon1, Color.green);
                    ReleasedNPCC++;
                    break;
                case 1:
                    ConsoleCNPC2.SetActive(true);
                    SetImageColor(ConsoleCIcon2, Color.green);
                    ReleasedNPCC++;
                    break;
                case 2:
                    ConsoleCNPC3.SetActive(true);
                    SetImageColor(ConsoleCIcon3, Color.green);
                    ReleasedNPCC++;
                    break;
                default:
                    Debug.LogWarning("Invalid NPCC Value");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("No NPCC to deplete");
        }
    }

    // Helper method to set the color of the Image component
    private void SetImageColor(GameObject icon, Color color)
    {
        Image image = icon.GetComponent<Image>();
        if (image != null)
        {
            Debug.Log("Setting color of " + icon.name + " to " + color);
            image.color = color;
        }
        else
        {
            Debug.LogWarning("No Image component found on " + icon.name);
        }
    }

    // Helper method to set active state for all NPCs
    private void SetActiveStateForAllNPCs(bool state)
    {
        ConsoleANPC1.SetActive(state);
        ConsoleANPC2.SetActive(state);
        ConsoleANPC3.SetActive(state);
        ConsoleBNPC1.SetActive(state);
        ConsoleBNPC2.SetActive(state);
        ConsoleBNPC3.SetActive(state);
        ConsoleCNPC1.SetActive(state);
        ConsoleCNPC2.SetActive(state);
        ConsoleCNPC3.SetActive(state);
    }
}