using System.Collections.Generic;
using UnityEngine;

/**
 * Plans what actions can be completed in order to fulfill a goal state.
 */

public class GoapPlanner
{
    /**
	 * Plan what sequence of actions can fulfill the goal.
	 * Returns null if a plan could not be found, or a list of the actions
	 * that must be performed, in order, to fulfill the goal.
	 */

    public Queue<GoapAction> plan(GameObject agent,
        HashSet<GoapAction> availableActions,
        HashSet<KeyValuePair<string, object>> worldState,
        HashSet<KeyValuePair<string, object>> goal)
    {
        // reset the actions so we can start fresh with them
        foreach (var a in availableActions)
        {
            a.doReset();
        }

        // check what actions can run using their checkProceduralPrecondition
        var usableActions = new HashSet<GoapAction>();
        foreach (var a in availableActions)
        {
            if (a.checkProceduralPrecondition(agent))
                usableActions.Add(a);
        }

        // we now have all actions that can run, stored in usableActions

        // build up the tree and record the leaf nodes that provide a solution to the goal.
        var leaves = new List<Node>();

        // build graph
        var start = new Node(null, 0, 0, worldState, null);
        var success = buildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            // oh no, we didn't get a plan
            Debug.Log("NO PLAN");
            return null;
        }

        // get the cheapest leaf
        Node cheapest = null;
        foreach (var leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.BetterThen(cheapest))
                    cheapest = leaf;
            }
        }

        // get its node and work back through the parents
        var result = new List<GoapAction>();
        var n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action); // insert the action in the front
            }
            n = n.parent;
        }
        // we now have this action list in correct order

        var queue = new Queue<GoapAction>();
        foreach (var a in result)
        {
            queue.Enqueue(a);
        }

        // hooray we have a plan!
        return queue;
    }

    /**
	 * Returns true if at least one solution was found.
	 * The possible paths are stored in the leaves list. Each leaf has a
	 * 'runningCost' value where the lowest Cost will be the best action
	 * sequence.
	 */

    private bool buildGraph(Node parent, List<Node> leaves
        , HashSet<GoapAction> usableActions, HashSet<KeyValuePair<string, object>> goal)
    {
        var foundOne = false;

        // go through each action available at this node and see if we can use it here
        foreach (var action in usableActions)
        {
            // if the parent state has the conditions for this action's preconditions, we can use it here
            if (inState(action.Preconditions, parent.state))
            {
                // apply the action's effects to the parent state
                var currentState = populateState(parent.state, action.Effects);
                //Debug.Log(GoapAgent.prettyPrint(currentState));
                var node = new Node(parent, parent.runningCost + action.GetCost(), parent.weight + action.GetWeight(),
                    currentState, action);

                //force child.precondition in parent.effects or child.precondition is empty.
                if (action.Preconditions.Count == 0 && parent.action != null ||
                    parent.action != null && !CondRelation(action.Preconditions, parent.action.Effects))
                    continue;

                if (inState(goal, currentState))
                {
                    // we found a solution!
                    leaves.Add(node);
                    foundOne = true;
                }
                else
                {
                    // not at a solution yet, so test all the remaining actions and branch out the tree
                    var subset = actionSubset(usableActions, action);
                    var found = buildGraph(node, leaves, subset, goal);
                    if (found)
                        foundOne = true;
                }
            }
        }

        return foundOne;
    }

    //if there is one true relationship
    private bool CondRelation(HashSet<KeyValuePair<string, object>> preconditions
                            , HashSet<KeyValuePair<string, object>> effects)
    {
        var allMatch = false;
        foreach (var t in preconditions)
        {
            var match = false;
            foreach (var s in effects)
            {
                if (s.Equals(t))
                {
                    match = true;
                    break;
                }
            }
            if (match)
                return true;
        }
        return allMatch;
    }

    /**
	 * Create a subset of the actions excluding the removeMe one. Creates a new set.
	 */

    private HashSet<GoapAction> actionSubset(HashSet<GoapAction> actions, GoapAction removeMe)
    {
        var subset = new HashSet<GoapAction>();
        foreach (var a in actions)
        {
            if (!a.Equals(removeMe))
                subset.Add(a);
        }
        return subset;
    }

    /**
	 * Check that all items in 'test' are in 'state'. If just one does not match or is not there
	 * then this returns false.
	 */

    private bool inState(HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>> state)
    {
        var allMatch = true;
        foreach (var t in test)
        {
            var match = false;
            foreach (var s in state)
            {
                if (s.Equals(t))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
                allMatch = false;
        }
        return allMatch;
    }

    /**
	 * Apply the stateChange to the currentState
	 */

    private HashSet<KeyValuePair<string, object>> populateState(HashSet<KeyValuePair<string, object>> currentState,
        HashSet<KeyValuePair<string, object>> stateChange)
    {
        var state = new HashSet<KeyValuePair<string, object>>();
        // copy the KVPs over as new objects
        foreach (var s in currentState)
        {
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        foreach (var change in stateChange)
        {
            // if the key exists in the current state, update the Value
            var exists = false;

            foreach (var s in state)
            {
                if (s.Equals(change))
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
                var updated = new KeyValuePair<string, object>(change.Key, change.Value);
                state.Add(updated);
            }
            // if it does not exist in the current state, add it
            else
            {
                state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }
        }
        return state;
    }

    /**
	 * Used for building up the graph and holding the running costs of actions.
	 */

    private class Node
    {
        public readonly GoapAction action;
        public readonly Node parent;
        public readonly float runningCost;
        public readonly HashSet<KeyValuePair<string, object>> state;
        public readonly float weight;

        public Node(Node parent, float runningCost, float weight, HashSet<KeyValuePair<string, object>> state,
            GoapAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.weight = weight;
            this.state = state;
            this.action = action;
        }

        /// <summary>
        ///     compare node
        /// </summary>
        /// <param name="cheapest"></param>
        /// <returns></returns>
        public bool BetterThen(Node rh)
        {
//            return runningCost < rh.runningCost;
            if (weight > rh.weight && runningCost < rh.runningCost)
                return true;
            if (weight < rh.weight && runningCost > rh.runningCost)
                return false;
            //make weight > cost
            var better = (weight/rh.weight - 1) >= (runningCost/rh.runningCost - 1);
            return better;
        }
    }
}