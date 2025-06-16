using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVFXReactor : MonoBehaviour {

    [Header("Puddle")]
    public GameObject puddlePrefab;
    public float puddleSize = 1;
    public Transform puddlePos;

    [Header("Splash")]
    public GameObject splashPrefab;
    public float splashSize = 1;
    public Transform splashPos;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public float explosionSize = 1;
    public Transform explosionPos;

    public void SpawnVFX(GameObject prefab,float size, Vector3 pos ,Quaternion rot) {
        GameObject g = Instantiate(prefab, pos, rot);
        g.transform.localScale = Vector3.one * size;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            this.SpawnVFX(this.puddlePrefab, this.puddleSize, this.puddlePos.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), Quaternion.Euler(-90,0,0));
            this.SpawnVFX(this.splashPrefab, this.splashSize, this.splashPos.position, Quaternion.identity);
        }
    }

    private void OnDisable() {
        print("Sonaaaaaaaaaaaaaaaaaaaaa");
        this.SpawnVFX(this.explosionPrefab, this.explosionSize, this.explosionPos.position,Quaternion.identity);
    }

}
