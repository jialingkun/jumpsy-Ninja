using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour {

	private Game_Script game;

	void Start(){
		game = GameObject.Find ("Canvas").GetComponent<Game_Script> ();
	}

	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.name.StartsWith ("Platform")) {
			print ("Yes");
			game.isGrounded = true;
		} else if (coll.gameObject.name.StartsWith ("WallLeft")) {
			game.wallBounce (1);
		} else if (coll.gameObject.name.StartsWith ("WallRight")) {
			game.wallBounce (-1);
		}
	}

	/*void OnTriggerEnter2D(Collider2D coll){
		if (coll.name.StartsWith ("Food")) {
		}
	}*/
}
