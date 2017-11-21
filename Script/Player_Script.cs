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
			Destroy(coll.transform.parent.gameObject);
		} else if (coll.gameObject.name.StartsWith ("WeakWallRight")) {
			game.wallBounce (-1); //bounce left
			Destroy(coll.transform.parent.gameObject);
		}  else if (coll.gameObject.tag == "Enemy"){
			game.gameover ();
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.name.StartsWith ("Coin")) {
			Destroy (coll.gameObject);
			game.getCoin ();
		} else if (coll.gameObject.name.StartsWith ("Platform")) {
			game.groundCheck ();
		} else if (coll.gameObject.name.StartsWith ("WeakPlatform")) {
			game.weakGroundCheck (coll.gameObject);
		} else if (coll.gameObject.name.StartsWith ("BouncePlatform")) {
			game.groundCheck ();
			game.platformBounce ();
		}
	}
}
