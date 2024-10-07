using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeANPC : NPCBase
{
    protected override void Start()
    {
        base.Start();
        // Customize behavior for Type A NPC here if needed
        // E.g., change movement patterns
        useSerpentineMovement = true;
        useZigzagMovement = false;
        weaveFrequency = 2.5f;
        weaveAmplitude = 1.5f;
    }

    protected override void Update()
    {
        base.Update();
        // Additional update logic for Type A NPC, if needed
    }
}
