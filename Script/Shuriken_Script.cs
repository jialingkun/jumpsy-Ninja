using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken_Script : MonoBehaviour {
	public float velocitySpeed = 0.5f;
	public int direction = 1; //1 right, -1 left
	private Vector2 velocity;
	private Vector2 pointLeft;
	private Vector2 pointRight;
	// Use this for initialization
	void Start () {
		pointLeft = this.transform.Find ("PointLeft").position;
		pointRight = this.transform.Find ("PointRight").position;
		this.GetComponent<Rigidbody2D> ().velocity = new Vector2 (direction * velocitySpeed, 0);
		if (direction == -1) {
			this.GetComponent<Animator> ().SetBool ("reverse", true);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (direction == 1 && this.transform.position.x > pointRight.x) {
			this.transform.position = pointLeft;
		}else if (direction == -1 && this.transform.position.x < pointLeft.x){
			this.transform.position = pointRight;
		}
	}
	

}
