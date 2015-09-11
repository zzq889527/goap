using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Goals
{
    public static string NeedHunger = "NeedHunger";
    public static string FillHunger = "FillHunger";
    public static string NeedMind = "NeedMind";
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

    public string NextGoal()
    {
        if (NeedMind())
            return Goals.FillMind;
        else if (NeedHunger())
            return Goals.FillHunger;
        else
            return Goals.FillOther;
    }

    public void UpdateWorldData(HashSet<KeyValuePair<string, object>> worldData)
    {
        if (NeedMind())
            worldData.Add(new KeyValuePair<string, object>(Goals.NeedMind, NeedMind()));
        if (NeedHunger())
            worldData.Add(new KeyValuePair<string, object>(Goals.NeedHunger, NeedHunger()));
    }

    private bool NeedMind()
    {
        return Mind < 50;
    }

    public bool NeedHunger()
    {
        return Hunger < 50;
    }
}
