using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveFog : MonoBehaviour
{
    public Material sky;
    public Color co;
    public bool NoFog;
    public float fogStartDistance = 65;
    public float fogEndDistance = 300;
    // Start is called before the first frame update
    void Start()
    {
        //active and set white color
        if(!NoFog)
        RenderSettings.fog = true;
        //set the fog color to white
        RenderSettings.fogColor = co;
        //set fog start and end
        RenderSettings.fogStartDistance = fogStartDistance;
        RenderSettings.fogEndDistance = fogEndDistance;
        RenderSettings.skybox = sky;
     }

    // Update is called once per frame
    void Update()
    {
        
    }
}
