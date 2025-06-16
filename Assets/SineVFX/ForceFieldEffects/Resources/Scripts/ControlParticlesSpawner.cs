using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlParticlesSpawner : MonoBehaviour {

    public ParticleSystem cps;
    public string bulletTag = "SineBullet";

    //private void OnCollisionEnter(Collision collision) {
    //    if (collision.gameObject.CompareTag(bulletTag) == true) {
    //        Destroy(collision.gameObject);
    //        cps.transform.position = collision.transform.position;
    //        cps.Emit(1);
    //    }
    //}

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag(bulletTag) == true) {
            print("Wow !! ");
            //cps.transform.position = other.transform.position;
            cps.Emit(1);
        }
    }
}
