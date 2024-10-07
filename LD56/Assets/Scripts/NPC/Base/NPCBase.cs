using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NPCBase : MonoBehaviour
{
    // Existing properties
    public float patrolSpeed = 3.5f;
    public float alertSpeed = 6.0f;
    public List<Transform> patrolPoints;
    protected internal int currentPatrolPoint = 0;
    public bool isAlert = false;

    // Reference to the NavMeshAgent and detection collider
    protected internal NavMeshAgent agent;

    // Reference to the player's Transform
    public Transform playerTransform;

    // NEW: Declare the currentState as a protected variable
    protected NPCState currentState;

    // Reference to the GameObject that can be instantiated in the FleeState
    public GameObject fleeObjectPrefab;

    // Movement variations and flee behavior
    public float fleeDistanceThreshold = 20f;
    public float fleeCheckDuration = 5f;

    // Patrol speed variance
    public bool useSerpentineMovement = true;
    public bool useZigzagMovement = false;
    public bool useRandomJitterMovement = false;
    public bool useCircularMovement = false;

    public float weaveFrequency = 2f;
    public float weaveAmplitude = 2f;
    public float jitterIntensity = 1f;
    public float circularMovementRadius = 3f;

    // Common Methods
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Count == 0)
        {
            Debug.LogError("No patrol points assigned for " + gameObject.name);
        }

        // Set initial state to patrol
        SetState(new PatrolState(this));
    }

    protected virtual void Update()
    {
        // Execute the current state's behavior
        currentState.Execute();  // Ensure currentState is called during the Update loop
    }

    public void SetState(NPCState newState)
    {
        currentState = newState;
        currentState.EnterState();
    }

    public void TransitionToFlee()
    {
        if (!isAlert && playerTransform != null) // Make sure playerTransform is assigned
        {
            isAlert = true;
            SetState(new FleeState(this, playerTransform)); // Pass playerTransform to FleeState
        }
        else
        {
            Debug.LogWarning("Player transform is not assigned.");
        }
    }

    // Detect when player enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Assuming the player has a tag called "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected! Switching to FleeState.");
            playerTransform = other.transform; // Assign the player's transform
            TransitionToFlee();
        }
    }
}
