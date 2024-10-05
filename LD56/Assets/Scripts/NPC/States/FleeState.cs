using UnityEngine;

public class FleeState : NPCState
{
    public FleeState(NPCBase npcBase) : base(npcBase) { }

    public override void EnterState()
    {
        npc.agent.speed = npc.alertSpeed;
        npc.isAlert = true;

        // Move away from the current patrol point (simple flee logic)
        FleeFromPlayer();
    }

    public override void Execute()
    {
        // Continue fleeing behavior if still alert
        FleeFromPlayer();
    }

    private void FleeFromPlayer()
    {
        // For simplicity, we can just move to the previous patrol point (reverse direction)
        int fleePoint = (npc.currentPatrolPoint - 1 + npc.patrolPoints.Count) % npc.patrolPoints.Count;
        npc.agent.destination = npc.patrolPoints[fleePoint].position;

        Debug.Log("Fleeing to patrol point: " + npc.patrolPoints[fleePoint].name);
    }
}
