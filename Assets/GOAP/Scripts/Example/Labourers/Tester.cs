using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tester : Labourer
{
    public bool EnableCollectTools;

    /**
     * Our only goal will ever be to make tools.
     * The ForgeTooldAction will be able to fulfill this goal.
     */
    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        goal.Add(new KeyValuePair<string, object>("walkaround", true));
        if (EnableCollectTools)
            goal.Add(new KeyValuePair<string, object>("collectTools", true));
        return goal;
    }

    public override void Tick()
    {
        if (NeedAbort)
        {
            Agent.AbortFsm();
            NeedAbort = false;
        }

        if (AddWalk)
        {
            WalkaroundAction walk = gameObject.AddComponent<WalkaroundAction>();
            Agent.AddAction(walk);
            AddWalk = false;
        }
        if (RemoveWalk)
        {
            WalkaroundAction walk = gameObject.GetComponent<WalkaroundAction>();
            Agent.RemoveAction(walk);
            Destroy(walk);
            RemoveWalk = false;
        }
    }

    public bool NeedAbort;

    public bool AddWalk;
    public bool RemoveWalk;
}
