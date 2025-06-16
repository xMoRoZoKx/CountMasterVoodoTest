using System;
using UnityEngine;

[Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3() { }

    public SerializableVector3(Vector3 unityVector)
    {
        x = unityVector.x;
        y = unityVector.y;
        z = unityVector.z;
    }

    public Vector3 ToUnityVector3()
    {
        return new Vector3(x, y, z);
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
}
