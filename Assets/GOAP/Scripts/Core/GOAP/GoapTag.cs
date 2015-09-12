using UnityEngine;
using System.Collections;

public struct GoapTag {
    public GoapTag(string name, bool enable) : this()
    {
        this.Name = name;
        this.Enable = enable;
    }

    public string Name { get; set; }
    public bool Enable { get; set; }
    public static GoapTag Default = new GoapTag("None",true);
}
