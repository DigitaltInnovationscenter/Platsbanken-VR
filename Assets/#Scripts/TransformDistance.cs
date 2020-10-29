using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformDistance 
{
    public OrbPanelHandler trans { get; set; }
    public float distance { get; set; }

    public TransformDistance(OrbPanelHandler trs, float dist)
    {
        trans = trs;
        distance = dist;
    }

}
