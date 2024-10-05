using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    private Transform target;  // The target object to look at

    [SerializeField]
    private bool allowXRotation = false;  // Boolean to toggle X axis rotation

    void Start()
    {
        // Find the object with the "Player" tag and set it as the target
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("No object with 'Player' tag found in the scene.");
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Get the direction to the target
            Vector3 direction = target.position - transform.position;

            // If X axis rotation is not allowed, zero out the X component of the direction
            if (!allowXRotation)
            {
                direction.y = 0f;  // Only rotate on Y axis
            }

            // If there is some direction to look at (not standing on the same Y)
            if (direction != Vector3.zero)
            {
                // Create a new rotation based on the direction and apply it
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f); // Smooth rotation
            }
        }
    }
}
