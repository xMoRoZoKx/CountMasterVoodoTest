using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsTools
{
    public static bool IsInLayerMask(this Component component, LayerMask mask)
    {
        return ((1 << component.gameObject.layer) & mask) != 0;
    }
    public static bool IsInLayerMask(this int layer, LayerMask mask)
    {
        return ((1 << layer) & mask) != 0;
    }
    public static bool IsInLayerMask(this LayerMask mask, int layer)
    {
        return ((1 << layer) & mask) != 0;
    }
}
