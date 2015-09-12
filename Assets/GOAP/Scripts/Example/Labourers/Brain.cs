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
        goalsWeight.Add(TagHunger, 0);
        goalsWeight.Add(TagMind, 0);
        goalsWeight.Add(TagOther, 0);
    }

    private float _costTime = 0;
    public void Tick(IGoap goap)
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

    Dictionary<GoapTag, int> goalsWeight = new Dictionary<GoapTag, int>();
    GoapTag TagHunger = new GoapTag(Goals.FillHunger, true);
    GoapTag TagMind = new GoapTag(Goals.FillMind, true);
    GoapTag TagOther = new GoapTag(Goals.FillOther, true);

    List<GoapTag> _sortedTags = new List<GoapTag>();
    public List<GoapTag> NextGoal()
    {
        goalsWeight[TagHunger] = GetHungerWeight();
        goalsWeight[TagMind] = GetMindWeight();
        goalsWeight[TagOther] = GetOtherWeight();

        var items = from pair in goalsWeight
                    orderby pair.Value descending
                    select pair;

        _sortedTags.Clear();
        foreach (KeyValuePair<GoapTag, int> pair in items)
        {
            _sortedTags.Add(pair.Key); 
        }
        return _sortedTags;
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
