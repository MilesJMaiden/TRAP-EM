using System.Collections;
using UnityEngine;

public class ButterflyNet : MonoBehaviour
{
    [SerializeField]
    private float swingDuration = 0.5f; // Duration of the swing
    [SerializeField]
    private Collider weaponCollider; // Collider of the weapon
    [SerializeField]
    private FirstPersonPlayerController playerController;

    private bool isSwinging = false; // Boolean to check if the weapon is swinging
    private float swingTime = 0.0f; // Time elapsed during the swing

    #region Rotation Variables
    private Quaternion startRotation;
    private Quaternion midRotation;
    private Quaternion endRotation;

    #endregion

    private Inventory inventory;

    void Start()
    {
        if (weaponCollider == null)
        {
            Debug.LogError("WeaponCollider is not assigned in the Inspector.");
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerController is not assigned in the Inspector.");
        }

        // Initialize the inventory
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory component not found in the scene.");
        }

        // Initialize the rotations
        startRotation = Quaternion.identity;
        midRotation = Quaternion.identity;
        endRotation = Quaternion.identity;
    }

    void Update()
    {
        if (isSwinging)
        {
            swingTime += Time.deltaTime;
            float t = swingTime / swingDuration;

            if (t <= 0.2f) // First 20% of the duration
            {
                transform.rotation = Quaternion.Lerp(startRotation, midRotation, t / 0.2f);
            }
            else // Remaining 80% of the duration
            {
                transform.rotation = Quaternion.Lerp(midRotation, endRotation, (t - 0.2f) / 0.8f);
            }

            if (swingTime >= swingDuration)
            {
                EndSwing();
            }
        }
    }

    public void Swing()
    {
        if (!isSwinging)
        {
            isSwinging = true;
            swingTime = 0.0f;
            weaponCollider.enabled = true; // Enable the collider during the swing

            // Set the initial rotation based on the player's forward direction
            Vector3 playerForward = playerController.transform.forward;
            Vector3 playerRight = playerController.transform.right;

            startRotation = Quaternion.LookRotation(playerForward) * Quaternion.Euler(0, 0, 0);
            midRotation = Quaternion.LookRotation(playerForward) * Quaternion.Euler(80, 0, 0);
            endRotation = Quaternion.LookRotation(playerForward) * Quaternion.Euler(220, 0, 0);
        }
    }

    private void EndSwing()
    {
        isSwinging = false;
        weaponCollider.enabled = false; // Disable the collider after the swing
        transform.rotation = startRotation; // Reset the rotation to the start rotation
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with: " + other.name);
        if (isSwinging)
        {
            int npcType = DetermineNPCType(other.tag);
            if (npcType != 0)
            {
                Debug.Log("Enemy hit with tag: " + other.tag);
                Destroy(other.gameObject); // Destroy the enemy on hit
                inventory.CaptureNPC(npcType);
                Debug.Log("Caught Types | NPCA: " + inventory.npcACount + " | NPCB: " + inventory.npcBCount + " | NPCC: " + inventory.npcCCount);  
            }
            else
            {
                Debug.Log("Collided with non-NPC object");
            }
        }
        else
        {
            Debug.Log("Swinging is false");
        }
    }

    // Method to determine the NPC type based on the collided object
    private int DetermineNPCType(string tag)
    {
        switch(tag)
        {
            case "NPCA":
                return 1;
            case "NPCB":
                return 2;
            case "NPCC":
                return 3;
            default:
                return 0;
        }
    }
}