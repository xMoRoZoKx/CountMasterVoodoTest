using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectableMonoBevaviour : MonoBehaviour
{
    [HideInInspector] public Connections connections = new Connections();

    protected virtual void OnDestroy()
    {
        connections.DisconnectAll();
    }
}
