using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.name.StartsWith ("Shuriken")) {
			Destroy (coll.gameObject);
		} else if(coll.name.StartsWith ("spike")) {
			Destroy (coll.gameObject);
		} 
	}
}
