using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour {

	private Game_Script game;

	void Start(){
		game = GameObject.Find ("Canvas").GetComponent<Game_Script> ();
	}

	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.name.StartsWith ("WallLeft")) {
			game.wallBounce (1); //bounce right
		} else if (coll.gameObject.name.StartsWith ("WallRight")) {
			game.wallBounce (-1); //bounce left
		} else if (coll.gameObject.name.StartsWith ("WeakWallLeft")) {
			game.wallBounce (1); //bounce right
			print("Bounce Right");
			Destroy(coll.transform.parent.gameObject);
		} else if (coll.gameObject.name.StartsWith ("WeakWallRight")) {
			game.wallBounce (-1); //bounce left
			print("Bounce Left");
			Destroy(coll.transform.parent.gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.name.StartsWith ("Coin")) {
			Destroy (coll.gameObject);
			game.getCoin ();
		} else if (coll.name.StartsWith ("Platform")) {
			game.groundCheck ();
		} else if (coll.name.StartsWith ("WeakPlatform")) {
			game.weakGroundCheck (coll.gameObject);
		} else if (coll.name.StartsWith ("BouncePlatform")) {
			game.groundCheck ();
			game.platformBounce ();
		} else if (coll.tag == "Enemy"){
			game.gameover ();
		}
	}
}
