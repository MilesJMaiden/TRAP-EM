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

        StartCoroutine(FindInventory());
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
            Debug.Log("Swinging is true");
            if (other.CompareTag("NPCA") || other.CompareTag("NPCB") || other.CompareTag("NPCC"))
            {
                Debug.Log("Enemy hit with tag: " + other.tag);
                // Destroy the enemy on hit
                Destroy(other.gameObject);

                // Use the Inventory component to capture the NPC
                if (inventory != null)
                {
                    // Assuming the NPC type is determined by the tag or another property
                    int npcType = DetermineNPCType(other);
                    inventory.CaptureNPC(npcType);

                    Debug.Log("Caught Types | NPCA: " + inventory.npcACount + " | NPCB: " + inventory.npcBCount + " | NPCC: " + inventory.npcCCount);
                }
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
    private int DetermineNPCType(Collider other)
    {
        // Example logic to determine NPC type based on tag
        if (other.CompareTag("NPCA"))
        {
            Debug.Log("NPCA Caught");
            return 1;
        }
        else if (other.CompareTag("NPCB"))
        {
            return 2;
        }
        else if (other.CompareTag("NPCC"))
        {
            return 3;
        }
        else
        {
            Debug.LogWarning("Unknown NPC type");
            return 0; // Invalid type
        }
    }

    private IEnumerator FindInventory()
    {
        while (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
            if (inventory == null)
            {
                Debug.LogWarning("Inventory component not found. Retrying...");
                yield return new WaitForSeconds(0.5f); // Wait for half a second before retrying
            }
        }
        Debug.Log("Inventory component found.");
    }
}