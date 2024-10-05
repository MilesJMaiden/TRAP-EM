public abstract class NPCState
{
    protected NPCBase npc;

    public NPCState(NPCBase npcBase)
    {
        npc = npcBase;
    }

    public abstract void EnterState();
    public abstract void Execute();
}
