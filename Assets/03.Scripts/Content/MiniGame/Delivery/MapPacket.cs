using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapPacket
{
    public float maxDist;
    public Action<float> onUpdateDistance;

    public MapPacket(float maxDist, Action<float> onUpdateDistance)
    {
        this.maxDist = maxDist;
        this.onUpdateDistance = onUpdateDistance;
    }
}