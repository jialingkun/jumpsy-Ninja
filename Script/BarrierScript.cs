using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour {

	private Game_Script game;

	void Start(){
		game = GameObject.Find ("Canvas").GetComponent<Game_Script> ();
	}


	void OnTriggerEnter2D(Collider2D coll){
		if (coll.name.StartsWith ("Shuriken")) {
			GameObject.Instantiate (game.shurikenDestroyPrefab, coll.transform.position, Quaternion.identity);
			Destroy (coll.gameObject);

		} else if(coll.name.StartsWith ("spike")) {
			GameObject.Instantiate (game.spikeDestroyPrefab, coll.transform.position, Quaternion.identity);
			Destroy (coll.gameObject);
		} 
	}
}
