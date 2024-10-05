using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public abstract class NPCBase : MonoBehaviour
{
    // Common Properties for all NPCs
    public float patrolSpeed = 3.5f;
    public float alertSpeed = 6.0f;
    public List<Transform> patrolPoints;
    protected internal int currentPatrolPoint = 0;
    public bool isAlert = false;

    // Reference to the NavMeshAgent and detection collider
    protected internal NavMeshAgent agent;
    public Collider detectionCollider;

    protected NPCState currentState;

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
        currentState.Execute();
    }

    public void SetState(NPCState newState)
    {
        currentState = newState;
        currentState.EnterState();
    }

    public void TransitionToFlee()
    {
        if (!isAlert)
        {
            isAlert = true;
            SetState(new FleeState(this));
        }
    }

    // Detect when player enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Assuming the player has a tag called "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected! Switching to FleeState.");
            TransitionToFlee();
        }
    }
}
