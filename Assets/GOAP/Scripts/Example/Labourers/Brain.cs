using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Goals
{
    public static string FillHunger = "FillHunger";
    public static string FillMind = "FillMind";
    public static string FillOther = "FillOther";
}

public class Brain : MonoBehaviour,IBrain
{
    public int Hunger = 100;
    public int Mind = 100;
    public void Init()
    {
        
    }

    private float _costTime = 0;
    public void Tick()
    {
        _costTime += Time.deltaTime;
        if (_costTime >= 1)
        {
            Hunger -= 2;
            Mind -= 2;
            _costTime = 0;
        }
    }

    public void Release()
    {

    }

    public HashSet<KeyValuePair<string, object>> NextGoal()
    {
        Dictionary<string, int> goalsWeight = new Dictionary<string, int>();
        goalsWeight.Add(Goals.FillHunger, GetHungerWeight());
        goalsWeight.Add(Goals.FillMind, GetMindWeight());
        goalsWeight.Add(Goals.FillOther, GetOtherWeight());
        var items = from pair in goalsWeight
                    orderby pair.Value descending
                    select pair;

        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        foreach (KeyValuePair<string, int> pair in items)
        {
            goal.Add(new KeyValuePair<string, object>(pair.Key, true));
        }
        return goal;
    }

    private int GetOtherWeight()
    {
        return Normal;
    }

    private int GetMindWeight()
    {
        if (Mind < 30)
            return Height;
        else if (Mind < 60)
            return Normal;
        else
            return Low;
    }

    private int Low = 1;
    private int Normal = 2;
    private int Height = 3;
    private int GetHungerWeight()
    {
        if (Hunger < 30)
            return Height;
        else if (Hunger < 60)
            return Normal;
        else
            return Low;
    }
}
