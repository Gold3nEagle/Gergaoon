using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour {

    public GameObject explosion;
    public ParticleSystem[] effects;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Hat")
        {
            Instantiate(explosion, transform.position, transform.rotation);
            foreach(var effect in effects)
            {
                effect.transform.parent = null;
                effect.Stop();
                Destroy(effect.gameObject, 1.0f);
            }
            Destroy(gameObject);
        }
    }



}
