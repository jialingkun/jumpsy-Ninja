using UnityEngine;
using System.Collections;

public class DestroyLine_Script : MonoBehaviour {
	private Game_Script game;

	void Start(){
		game = GameObject.Find ("Canvas").GetComponent<Game_Script> ();
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.name.StartsWith ("Stage")) {
			//eating
			game.spawnStages();
			Destroy (coll.gameObject);
		}
	}
}
