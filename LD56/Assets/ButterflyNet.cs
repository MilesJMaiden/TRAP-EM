using UnityEngine;

public class ButterflyNet : MonoBehaviour
{
    [SerializeField]
    private float swingDuration = 0.5f; // Duration of the swing
    [SerializeField]
    private Collider weaponCollider; // Collider of the weapon
    [SerializeField]
    private Animator weaponAnimator; // Animator for the weapon

    private bool isSwinging = false; // Boolean to check if the weapon is swinging
    private float swingTime = 0.0f; // Time elapsed during the swing

    void Start()
    {
        if (weaponCollider == null)
        {
            Debug.LogError("WeaponCollider is not assigned in the Inspector.");
        }

        if (weaponAnimator == null)
        {
            Debug.LogError("WeaponAnimator is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        if (isSwinging)
        {
            swingTime += Time.deltaTime;
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
            weaponAnimator.SetTrigger("Swing"); // Trigger the swing animation
        }
    }

    private void EndSwing()
    {
        isSwinging = false;
        weaponCollider.enabled = false; // Disable the collider after the swing
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSwinging && other.CompareTag("Enemy"))
        {
            // Destroy the enemy on hit
            Destroy(other.gameObject);
        }
    }
}