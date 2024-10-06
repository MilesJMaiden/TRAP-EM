using UnityEngine;

public class ButterflyNet : MonoBehaviour
{
    [SerializeField]
    private float swingDuration = 0.5f; // Duration of the swing
    [SerializeField]
    private Collider weaponCollider; // Collider of the weapon
    

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

        startRotation = Quaternion.Euler(0, 0, 0);
        midRotation = Quaternion.Euler(0, 0, 20);
        endRotation = Quaternion.Euler(0, -20, -120);
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
        if (isSwinging && (other.CompareTag("NPCA") || other.CompareTag("NPCB") || other.CompareTag("NPCC")))
        {
            // Destroy the enemy on hit
            Destroy(other.gameObject);

            // Use the Inventory component to capture the NPC
            if (inventory != null)
            {
                // Assuming the NPC type is determined by the tag or another property
                int npcType = DetermineNPCType(other);
                inventory.CaptureNPC(npcType);
            }
        }
    }

    // Method to determine the NPC type based on the collided object
    private int DetermineNPCType(Collider other)
    {
        // Example logic to determine NPC type based on tag
        if (other.CompareTag("NPCA"))
        {
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
}