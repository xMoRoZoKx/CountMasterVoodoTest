using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_DevObject : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
    }
}
