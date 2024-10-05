using UnityEngine;

public class PatrolState : NPCState
{
    public PatrolState(NPCBase npcBase) : base(npcBase) { }

    public override void EnterState()
    {
        npc.agent.speed = npc.patrolSpeed;
        npc.isAlert = false;
        MoveToNextPatrolPoint(); // Start patrol by moving to the first point
    }

    public override void Execute()
    {
        // If NPC is not alert, continue patrolling
        if (!npc.isAlert)
        {
            // Check if the NPC has reached the destination
            if (!npc.agent.pathPending && npc.agent.remainingDistance <= npc.agent.stoppingDistance)
            {
                // NPC reached the patrol point, so move to the next one
                MoveToNextPatrolPoint();
            }
        }
        else
        {
            npc.TransitionToFlee(); // Switch to FleeState when alerted
        }
    }

    private void MoveToNextPatrolPoint()
    {
        // Ensure there are patrol points set
        if (npc.patrolPoints.Count == 0)
        {
            Debug.LogError("No patrol points assigned.");
            return;
        }

        // Set the NPC's destination to the next patrol point
        npc.agent.destination = npc.patrolPoints[npc.currentPatrolPoint].position;
        Debug.Log("Moving to patrol point: " + npc.patrolPoints[npc.currentPatrolPoint].name);

        // Move to the next patrol point in the list (loop back to the first after reaching the last)
        npc.currentPatrolPoint = (npc.currentPatrolPoint + 1) % npc.patrolPoints.Count;
    }
}
