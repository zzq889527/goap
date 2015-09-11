using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IBrain
{
    void Init();
    void Tick();
    void Release();
    HashSet<KeyValuePair<string, object>> NextGoal();
}
