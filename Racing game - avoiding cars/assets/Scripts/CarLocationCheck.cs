using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLocationCheck : MonoBehaviour {

    BoxCollider bx;
	// Use this for initialization
	void Start () {
        bx = this.GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        // We only care if the car hits obstacles on the road -- colliding with anything else is fine
        if (other.gameObject.tag == "ObstacleCar")
        {
            gameObject.SetActive(false);
        }
       
    }
}
