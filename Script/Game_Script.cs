using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class Game_Script : MonoBehaviour {
	//jump
	private GameObject player;
	private Transform targetRange;
	private Transform BounceRange;
	private Rigidbody2D rigid;
	private float gravity;
	private bool isGrounded;
	private int lastDirection;
	//reusable variable
	private Vector2 targetPosition;
	private Vector2 planarTarget;
	private Vector2 planarPosition;
	private float planarDistance;
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
	//chasing camera
	private bool firstJump;
	public float initialSpeed;
	public float currentSpeed;
	public float speedIncrement;
	public float maxSpeed;


	//Stage field
	private GameObject stageField;

	//stages
	public Stage_Templates[] stageTemplates;
	public int stagesInitialCount;
	private GameObject recentStageObject;
	private float recentStageHeight;
	//random prefabs to spawn
	private List<GameObject> prefabsToRand;
	private GameObject selectedPrefab;
	private float selectedPrefabHeight;
	private Vector2 spawnPosition;

	//restart initial parameter
	private Vector3 cameraInitialPosition;
	private Vector2 playerInitialPosition;
	public GameObject stageInitialPrefab;

	//score and coin
	private int score;
	private int bestscore;
	private int currentCoin;
	private int collectedCoin;
	private Text gameplayScoreText;
	private Text gameplayBestScoreText;
	private Text gameplayCoinText;
	//get coin
	private bool isGettingCoin;

	//weak platform
	public float weakPlatformTime;
	private bool isOnWeak;
	private GameObject weakPlatform;





	// Use this for initialization
	void Start () {
		isGrounded = true;
		//jump
		player = GameObject.Find("Player");
		targetRange = GameObject.Find ("TargetRange").transform;
		BounceRange = GameObject.Find ("BounceRange").transform;
		rigid = player.GetComponent<Rigidbody2D>();
		gravity = Physics.gravity.magnitude * rigid.gravityScale;
		lastDirection = 1; //initialize only
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
		//chasing camera
		firstJump = false;
		currentSpeed = initialSpeed;

		//Stage field
		stageField = GameObject.Find("StageField");;


		//stages spawn initialize
		prefabsToRand = new List<GameObject>();
		recentStageObject = GameObject.Find("Stage0");

		//stages inital spawn
		for (int i = 0; i < stagesInitialCount; i++) {
			spawnStages ();
		}

		//restart initial parameter
		cameraInitialPosition = cameraObject.transform.position;
		playerInitialPosition = player.transform.position;

		//score text
		gameplayScoreText = GameObject.Find("CurrentScore").GetComponent<Text>();
		gameplayBestScoreText = GameObject.Find("CurrentBestScore").GetComponent<Text>();
		gameplayCoinText = GameObject.Find("CurrentCoin").GetComponent<Text>();
		//score and coin value
		score = 0;
		bestscore = PlayerPrefs.GetInt("bestscore",0);
		currentCoin = 0;
		collectedCoin = PlayerPrefs.GetInt("coin",0);
		//update score text
		gameplayScoreText.text = "SCORE : "+score;
		gameplayBestScoreText.text = "BEST : "+bestscore;
		gameplayCoinText.text = "" + currentCoin;
		//get coin
		isGettingCoin = false;

		//weak platform
		isOnWeak = false;

	}





	
	// Update is called once per frame
	void FixedUpdate () {
		//temporary keyboard control
		if (Input.GetKey(KeyCode.RightArrow)) {
			clickJump (1);
		}else if (Input.GetKey(KeyCode.LeftArrow)) {
			clickJump (-1);
		}


		//stopping point
		if (currentPoint>=0 && currentPoint<=4) {
			if (Mathf.Abs(player.transform.position.x-point [currentPoint]) < 0.05f && !isStopped) {
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

		//chasing Camera
		/*if (firstJump) {
			cameraPosition.y = cameraPosition.y + Time.deltaTime * currentSpeed;
			cameraObject.transform.position = cameraPosition;
			if (currentSpeed<maxSpeed) {
				currentSpeed = initialSpeed + cameraPosition.y * speedIncrement;
			}

		}*/

		//update score label
		score = Mathf.RoundToInt(cameraPosition.y);
		gameplayScoreText.text = "SCORE : "+score;

		//get coin
		if (isGettingCoin) {
			currentCoin = currentCoin + 1;
			gameplayCoinText.text = "" + currentCoin;
			isGettingCoin = false;
		}
		
	}

	public void gameover(){
		//best score
		if (score>bestscore) {
			PlayerPrefs.SetInt("bestscore", score);
			bestscore = score;
			//newBestScoreText.SetActive(true);
		}
		refresh ();
	}

	private void refresh(){
		//destroy all stage and tail
		GameObject[] clones = GameObject.FindGameObjectsWithTag("Clone");
		foreach (GameObject clone in clones) {
			GameObject.Destroy(clone);
		}

		//return to initial position
		cameraObject.transform.position = cameraInitialPosition;
		rigid.velocity = Vector2.zero;
		player.transform.position = playerInitialPosition;
		//don't draw trail when going back
		player.GetComponent<TrailRenderer>().Clear();

		//instantiate stage 0
		recentStageObject = (GameObject)Instantiate (stageInitialPrefab);
		recentStageObject.transform.parent = stageField.transform;

		isGrounded = true;
		currentPoint = 1;
		isStopped = true;
		//moving camera
		cameraPosition = cameraObject.transform.position;
		//chasing camera
		firstJump = false;
		currentSpeed = initialSpeed;

		//stages inital spawn
		for (int i = 0; i < stagesInitialCount; i++) {
			spawnStages ();
		}

		//restart initial parameter
		cameraInitialPosition = cameraObject.transform.position;
		playerInitialPosition = player.transform.position;

		//score and coin value
		score = 0;
		bestscore = PlayerPrefs.GetInt("bestscore",0);
		currentCoin = 0;
		//update score text
		gameplayScoreText.text = "SCORE : "+score;
		gameplayBestScoreText.text = "BEST : "+bestscore;
		gameplayCoinText.text = "" + currentCoin;
		//get coin
		isGettingCoin = false;
	}

	public void spawnStages(){
		prefabsToRand.Clear ();

		for (int i = 0; i < stageTemplates.Length; i++) {
			//check distance condition (-1 for endless distance max)
			if (cameraPosition.y>=stageTemplates[i].distanceMin && 
				(cameraPosition.y<=stageTemplates[i].distanceMax || stageTemplates[i].distanceMax<0)) {
				//put all prefabs to list
				for (int j = 0; j < stageTemplates[i].stage_prefabs.Length; j++) {
					prefabsToRand.Add (stageTemplates [i].stage_prefabs [j]);
				}

			}
		}

		selectedPrefab = prefabsToRand [Random.Range (0, prefabsToRand.Count)];

		selectedPrefabHeight = selectedPrefab.GetComponent<SpriteRenderer> ().sprite.bounds.extents.y * selectedPrefab.transform.localScale.y;
		recentStageHeight = recentStageObject.GetComponent<SpriteRenderer> ().sprite.bounds.extents.y * recentStageObject.transform.localScale.y;
		spawnPosition = recentStageObject.transform.position;
		spawnPosition.y = spawnPosition.y + (recentStageHeight + selectedPrefabHeight);
		recentStageObject = (GameObject)Instantiate (selectedPrefab, spawnPosition, Quaternion.identity);
		recentStageObject.transform.parent = stageField.transform;
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
			planarDistance = Vector2.Distance(planarTarget, planarPosition);

			// Distance along the y axis between objects
			yOffset = player.transform.position.y - targetPosition.y;

			initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(planarDistance, 2)) / (planarDistance * Mathf.Tan(angle) + yOffset));

			velocity.x = initialVelocity * Mathf.Cos (angle) * direction; //direction -1 left, 1 right
			velocity.y = initialVelocity * Mathf.Sin (angle) * 1; // 1 up



			//point change
			currentPoint = currentPoint + direction;
			isStopped = false;


			//execute jump
			rigid.velocity = velocity;
			isGrounded = false;

			//move the spike
			if (!firstJump) {
				firstJump = true;
			}

			//update last direction
			lastDirection = direction;

			//destroy weak platform
			if (isOnWeak) {
				Destroy (weakPlatform);
			}
		}
	}

	public void groundCheck(){
		if (rigid.velocity.y <= 0) {
			isGrounded = true;
		}
	}

	public void getCoin(){
		isGettingCoin = true;
	}





	//Platform variation
	public void wallBounce(int direction){
		//direction -1 left, 1 right

		//parabola jump algorithm
		targetPosition = BounceRange.position;

		// Positions of this object and the target on the same plane
		planarTarget.x = targetPosition.x; //half distance
		planarPosition.x = player.transform.position.x;

		// Planar distance between objects
		planarDistance = Vector2.Distance(planarTarget, planarPosition);

		// Distance along the y axis between objects
		yOffset = player.transform.position.y - targetPosition.y; //half height

		initialVelocity = (1 / Mathf.Cos(tempBounceAngle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(planarDistance, 2)) / (planarDistance * Mathf.Tan(tempBounceAngle) + yOffset));

		velocity.x = initialVelocity * Mathf.Cos (tempBounceAngle) * direction; //direction -1 left, 1 right
		velocity.y = initialVelocity * Mathf.Sin (tempBounceAngle) * 1; // 1 up



		//point change (From 5 to 4 or from -1 to 0)
		currentPoint = currentPoint + direction;
		isStopped = false;


		//execute jump
		rigid.velocity = velocity;
		isGrounded = false;

		//update last direction
		lastDirection = direction;
	}

	public void platformBounce(){
		clickJump (lastDirection);
	}

	public void weakGroundCheck(GameObject platform){
		groundCheck ();
		if (isGrounded) {
			isOnWeak = true;
			weakPlatform = platform;
		}
	}


}
