using UnityEngine;
using System.Collections;

public class SleepAction : GoapAction
{
    private bool sleeped = false;
    private CampComponent camp;

    private float startTime = 0;
    public float workDuration = 3; // seconds
    public SleepAction()
    {
        addPrecondition(Goals.NeedMind, true);
		addEffect (Goals.FillMind, true);
	}
    public override void reset()
    {
        sleeped = false;
        startTime = 0;
    }

    public override bool isDone()
    {
        return sleeped;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        CampComponent c = UnityEngine.GameObject.FindObjectOfType(typeof(CampComponent)) as CampComponent;
        if (c == null)
            return false;
        camp = c;
        target = camp.gameObject;
        return camp != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > workDuration)
        {
            // finished chopping
            Brain brain = (Brain)agent.GetComponent(typeof(Brain));
            brain.Mind+=50;
            sleeped = true;
        }
        return true;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a tree
    }
}
