using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Used for building up the graph and holding the running costs of actions.
 */

public class GoapNode
{
    private static int MaxID;
    public int ID;
    public GoapAction action;
    public GoapNode parent;
    public float runningCost;
    public Dictionary<string, bool> state;
    public float weight;

    public GoapNode(GoapNode parent, float runningCost, float weight, Dictionary<string, bool> state,
        GoapAction action)
    {
        ID = MaxID++;
        ReInit(parent, runningCost, weight, state, action);
    }

    public void ReInit(GoapNode parent, float runningCost, float weight, Dictionary<string, bool> state,
        GoapAction action)
    {
        Clear();
        this.parent = parent;
        this.runningCost = runningCost;
        this.weight = weight;
        this.state = state;
        this.action = action;
    }

    private void Clear()
    {
        this.parent = null;
        this.runningCost = 0;
        this.weight = 0;
        this.state = null;
        this.action = null;
    }

    /// <summary>
    ///     compare node
    /// </summary>
    /// <param name="cheapest"></param>
    /// <returns></returns>
    public bool BetterThen(GoapNode rh)
    {
        //            return runningCost < rh.runningCost;
        if (weight > rh.weight && runningCost < rh.runningCost)
            return true;
        if (weight < rh.weight && runningCost > rh.runningCost)
            return false;
        //make weight > cost
        var better = (weight / rh.weight - 1) >= (runningCost / rh.runningCost - 1);
        return better;
    }
}

public class NodeManager
{
    static List<int> _freeIds = new List<int>();
    static Stack<GoapNode> _freeNodes = new Stack<GoapNode>();
    public static GoapNode GetFreeNode(GoapNode parent, float runningCost, float weight, Dictionary<string, bool> state,
        GoapAction action)
    {
        if(_freeNodes.Count <= 0)
            return new GoapNode(parent,runningCost,weight,state,action);
        else
        {
            GoapNode free = _freeNodes.Pop();
            _freeIds.Remove(free.ID);
            free.ReInit(parent, runningCost, weight, state, action);
            return free;
        }
    }

    public static void ReleaseNode(List<GoapNode> leaves)
    {
        foreach (var leaf in leaves)
        {
            ReleaseNode(leaf);
        }
    }
    public static void ReleaseNode(GoapNode node)
    {
        if (!_freeIds.Contains(node.ID))
        {
            _freeIds.Add(node.ID);
            _freeNodes.Push(node);
        }
        if (node.parent != null)
            ReleaseNode(node.parent);
    }
}
