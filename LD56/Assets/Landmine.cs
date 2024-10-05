using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    [SerializeField] private float explosionForce = 10.0f;
    [SerializeField] private float explosionRadius = 5.0f;
    [SerializeField] private string playerTag = "Player"; // Tag to identify the player
    [SerializeField] private float maxExplosionDistance = 10.0f; // Maximum distance the force should be applied

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            CharacterController playerController = other.GetComponent<CharacterController>();
            if (playerController != null)
            {
                Vector3 explosionDirection = (other.transform.position - transform.position).normalized;
                Vector3 force = explosionDirection * explosionForce;

                // Apply the force to the player's velocity
                FirstPersonPlayerController playerScript = other.GetComponent<FirstPersonPlayerController>();
                if (playerScript != null)
                {
                    playerScript.ApplyExplosionForce(force, maxExplosionDistance);
                }
            }
        }
    }
}
