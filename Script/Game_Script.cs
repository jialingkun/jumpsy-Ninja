using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class Game_Script : MonoBehaviour {
	//jump
	private GameObject player;
	private Transform playerTransform;
	private Transform targetRange;
	private Transform BounceRange;
	private Rigidbody2D rigid;
	private float gravity;
	[HideInInspector]
	public bool isGrounded;

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
	public float bounceAngle;
	private float tempBounceAngle;

	//Stopping Point
	private float[] point;
	private int currentPoint;
	private bool isStopped;

	//Moving camera
	private GameObject cameraObject;
	private Vector3 cameraPosition;
	private GameObject movingLine;
	public float CameraMoveSpeed;



	// Use this for initialization
	void Start () {
		isGrounded = true;
		//jump
		player = GameObject.Find("Player");
		playerTransform = player.transform;
		targetRange = GameObject.Find ("TargetRange").transform;
		BounceRange = GameObject.Find ("BounceRange").transform;
		rigid = player.GetComponent<Rigidbody2D>();
		gravity = Physics.gravity.magnitude * rigid.gravityScale;
		// Selected angle in radians
		angle = initialAngle * Mathf.Deg2Rad;
		tempBounceAngle = bounceAngle * Mathf.Deg2Rad;
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

		//moving camera
		cameraObject = GameObject.Find("Camera");
		cameraPosition = cameraObject.transform.position;
		movingLine = GameObject.Find("CameraMovingLine");

	}
	
	// Update is called once per frame
	void Update () {
		//stopping point
		if (currentPoint>=0 && currentPoint<=4) {
			if (Mathf.Abs(playerTransform.position.x-point [currentPoint]) < 0.05f && !isStopped) {
				velocity.x = 0;
				velocity.y = rigid.velocity.y;
				rigid.velocity = velocity;
				isStopped = true;
			}
		}

		//Moving camera
		if (movingLine.transform.position.y > cameraPosition.y) {
			cameraPosition.y = Mathf.Lerp (cameraPosition.y, movingLine.transform.position.y, Time.deltaTime * CameraMoveSpeed);
			cameraObject.transform.position = cameraPosition;
		}

		
	}

	public void clickJump(int direction){
		//direction -1 left, 1 right

		if (isGrounded) {
			//parabola jump algorithm
			targetPosition = targetRange.position;

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
			isStopped = false;


			//execute jump
			rigid.velocity = velocity;
			isGrounded = false;
		}
	}

	public void wallBounce(int direction){
		//direction -1 left, 1 right

		//parabola jump algorithm
		targetPosition = BounceRange.position;

		// Positions of this object and the target on the same plane
		planarTarget.x = targetPosition.x; //half distance
		planarPosition.x = player.transform.position.x;

		// Planar distance between objects
		distance = Vector2.Distance(planarTarget, planarPosition);

		// Distance along the y axis between objects
		yOffset = player.transform.position.y - targetPosition.y; //half height

		initialVelocity = (1 / Mathf.Cos(tempBounceAngle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(tempBounceAngle) + yOffset));

		velocity.x = initialVelocity * Mathf.Cos (tempBounceAngle) * direction; //direction -1 left, 1 right
		velocity.y = initialVelocity * Mathf.Sin (tempBounceAngle) * 1; // 1 up



		//point change (From 5 to 4 or from -1 to 0)
		currentPoint = currentPoint + direction;
		isStopped = false;


		//execute jump
		rigid.velocity = velocity;
		isGrounded = false;
	}


}
