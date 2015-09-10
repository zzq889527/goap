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
    }

    public bool NeedAbort;
}
