using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformIn_Script : MonoBehaviour {
	private Game_Script game;
	private Vector2 destination;
	private float speed;
	// Use this for initialization
	void Start () {
		game = GameObject.Find ("Canvas").GetComponent<Game_Script> ();
		destination = this.transform.Find ("Destination").position;

		speed = 10f; //editable
	}
	
	// Update is called once per frame
	void Update () {
		if (game.isPlaying) {
			if (Vector2.Distance(this.transform.position, destination) > 0.05f) {
				this.transform.position = Vector2.MoveTowards (this.transform.position, destination, speed * Time.deltaTime);
			}

		}
	}
}
