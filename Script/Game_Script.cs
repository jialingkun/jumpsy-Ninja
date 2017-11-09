using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class Game_Script : MonoBehaviour {
	//jump
	private GameObject player;
	private Transform playerTransform;
	private Transform targetLeft;
	private Rigidbody2D rigid;
	private float gravity;
	//reusable variable
	private Vector2 targetPosition;
	private Vector2 planarTarget;
	private Vector2 planarPosition;
	private float distance;
	private float yOffset;
	private float initialVelocity;
	private Vector2 velocity;
	//jump angle
	public float initialAngle;
	private float angle;

	//Stopping Point
	private float[] point;
	private int currentPoint;
	private bool isStopped;



	// Use this for initialization
	void Start () {
		//jump
		player = GameObject.Find("Player");
		playerTransform = player.transform;
		targetLeft = GameObject.Find ("TargetRange").transform;
		rigid = player.GetComponent<Rigidbody2D>();
		gravity = Physics.gravity.magnitude * rigid.gravityScale;
		// Selected angle in radians
		angle = initialAngle * Mathf.Deg2Rad;
		// intialize only
		planarTarget = new Vector2(0, 0);
		planarPosition = new Vector2(0, 0);
		velocity = new Vector2(0, 0);

		//stopping point
		point = new float[5];
		point [0] = GameObject.Find ("Point1").transform.position.x;
		point [1] = GameObject.Find ("Point2").transform.position.x;
		point [2] = GameObject.Find ("Point3").transform.position.x;
		point [3] = GameObject.Find ("Point4").transform.position.x;
		point [4] = GameObject.Find ("Point5").transform.position.x;
		currentPoint = 1;
		isStopped = true;

	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs(playerTransform.position.x-point [currentPoint]) < 0.05f && !isStopped) {
			velocity.x = 0;
			velocity.y = rigid.velocity.y;
			rigid.velocity = velocity;
			isStopped = true;

			print ("STOP");
		}
		
	}

	public void clickJump(int direction){
		//direction -1 left, 1 right


		//parabola jump algorithm
		targetPosition = targetLeft.position;

		// Positions of this object and the target on the same plane
		planarTarget.x = targetPosition.x;
		planarPosition.x = player.transform.position.x;

		// Planar distance between objects
		distance = Vector2.Distance(planarTarget, planarPosition);

		// Distance along the y axis between objects
		yOffset = player.transform.position.y - targetPosition.y;

		initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

		velocity.x = initialVelocity * Mathf.Cos (angle) * direction; //direction -1 left, 1 right
		velocity.y = initialVelocity * Mathf.Sin (angle) * 1; // 1 up



		//point change
		currentPoint = currentPoint + direction;
		if (currentPoint > 4) {
			currentPoint = 4;
			isStopped = true; //So it can still jump to wall
		} else if (currentPoint < 0) {
			currentPoint = 0;
			isStopped = true; //So it can still jump to wall
		} else {
			isStopped = false;
		}


		//execute jump
		rigid.velocity = velocity;
	}


}
