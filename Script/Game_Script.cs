﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class Game_Script : MonoBehaviour {
	//splash screen
	public float splash_DelayTime;
	private GameObject splashUI;
	private Image panelImage;

	//ads component
	private Ads ads;

	//leaderboard
	private string leaderboardID = "CgkI2-3qqN8XEAIQAQ";

	//Audio
	public AudioClip selectSound;
	public AudioClip eatSound;
	public AudioClip jumpSound;
	public AudioClip deathSound;
	public AudioClip powerupSound;
	private AudioSource BGMaudioSource;
	private AudioSource SEaudioSource;
	//mute
	private GameObject muteONButton;
	private GameObject muteOFFButton;
	private int muteState;

	//revive
	private GameObject reviveButton;
	private bool reviveState;
	public GameObject stageRevivePrefab;



	//UI object
	private GameObject menuUI;
	private GameObject howtoUI;
	private GameObject playUI;
	private GameObject tutorialUI;
	private GameObject gameoverUI;
	private GameObject reviveUI;
	private GameObject pauseUI;
	private GameObject quitUI;
	//shop UI
	private GameObject shopUI;
	private GameObject notEnoughUI;
	//private GameObject wantToBuyUI;
	private GameObject afterAdsUI;

	//tutorial
	private GameObject tutorialControl;
	private GameObject tutorialLeft;
	private GameObject tutorialRight;
	public GameObject stageTutorialPrefab;


	//start parameter
	[HideInInspector]
	public bool isPlaying;

	//jump
	private GameObject player;
	private TrailRenderer trail;
	private Transform targetRange;
	private Transform BounceRange;
	private Rigidbody2D rigid;
	private float gravity;
	private bool isGrounded;
	//player direction
	private Vector3 headRotate;
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
	//shaking camera
	public float shakeMagnitude;
	public float shakeDuration;


	//Stage field
	private GameObject stageField;
	//stages
	public GameObject stageInitialPrefab;
	public Stage_Templates[] stageTemplates;
	public int stagesInitialCount;
	private GameObject recentStageObject;
	private float recentStageHeight;
	//random prefabs to spawn
	private List<GameObject> prefabsToRand;
	private GameObject selectedPrefab;
	private float selectedPrefabHeight;
	private Vector2 spawnPosition;

	//Background field
	private GameObject backgroundField;
	private Vector2 backgroundFieldPosition;
	public float backgroundSlowDown;
	//background
	public GameObject backgroundInitialPrefab;
	//public Background_Templates[] backgroundTemplates;
	public Background_Templates[] backgroundTransition;
	public int backgroundInitialCount;
	private GameObject recentBackgroundObject;
	private float recentBackgroundHeight;
	//transition parameter
	//private int currentBackgroundTemplateNumber;
	//random prefabs to spawn
	private GameObject selectedBackgroundPrefab;
	private float selectedBackgroundPrefabHeight;
	private Vector3 backgroundSpawnPosition;

	//Characters
	public Character_Templates[] characters;
	private int selectedCharacter;
	private int buyIndexCharacter;
	private GameObject tempCharacterObject;

	//restart initial parameter
	private Vector3 cameraInitialPosition;
	private Vector2 playerInitialPosition;

	//score and coin
	private int score;
	private int bestscore;
	private int collectedCoin;
	//get coin
	private bool isGettingCoin;

	//Scoring UI
	private Text gameplayScoreText;
	private Text gameplayBestScoreText;
	private Text gameplayCoinText;
	private Text shopCoinText;
	//gameover and Scoring UI
	private Text gameoverScoreText;
	private Text gameoverBestscoreText;
	private GameObject newBestScoreText;


	//weak platform
	private bool isOnWeak;
	private GameObject weakPlatform;

	//Barrier
	private GameObject barrier;
	private Animator barrierAnimator;

	public int barrierLifespan;
	[HideInInspector]
	public int currentBarrierLifespan;

	//animation prefab
	public GameObject playerExplodePrefab;
	public GameObject wallBreakPrefab;
	public GameObject platformBreakPrefab;
	public GameObject shurikenDestroyPrefab;
	public GameObject spikeDestroyPrefab;





	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll ();

		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate ();

		//splash
		splashUI = GameObject.Find ("SplashScreen");
		panelImage = GameObject.Find("SplashPanel").GetComponent<Image> ();
		FadeInSplash();

		ads = this.GetComponent<Ads> ();

		//audio
		BGMaudioSource = this.GetComponent<AudioSource>();
		SEaudioSource = this.gameObject.AddComponent<AudioSource> ();
		//mute
		muteONButton = GameObject.Find ("MuteON");
		muteOFFButton = GameObject.Find ("MuteOFF");
		muteState = PlayerPrefs.GetInt("mutestate",0);

		//UI
		menuUI = GameObject.Find ("MainMenu");
		playUI = GameObject.Find ("GamePlay");
		gameoverUI = GameObject.Find ("GameOver");
		howtoUI = GameObject.Find ("HowTo");
		pauseUI = GameObject.Find ("Pause");
		quitUI = GameObject.Find ("Quit");
		//shopUI
		shopUI = GameObject.Find ("Shop");
		notEnoughUI = GameObject.Find ("NotEnough");
		//wantToBuyUI = GameObject.Find ("WantToBuy");
		afterAdsUI = GameObject.Find ("AfterAds");

		//Tutorial
		tutorialUI = GameObject.Find ("Tutorial");
		tutorialControl = GameObject.Find ("TutorialControl");
		tutorialLeft = GameObject.Find ("TutorialLeft");
		tutorialRight = GameObject.Find ("TutorialRight");

		//revive
		reviveButton = GameObject.Find ("Revive");
		reviveState = false;

		//playing parameter
		isPlaying = false;


		isGrounded = true;
		//jump
		player = GameObject.Find("Player");
		trail = GameObject.Find("Trail").GetComponent<TrailRenderer>();
		targetRange = GameObject.Find ("TargetRange").transform;
		BounceRange = GameObject.Find ("BounceRange").transform;
		rigid = player.GetComponent<Rigidbody2D>();
		gravity = Physics.gravity.magnitude * rigid.gravityScale;
		//direction
		headRotate = new Vector3(0,180,0); //initialize only
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
		currentPoint = 2;
		isStopped = true;

		//moving camera
		cameraObject = GameObject.Find("Camera");
		cameraPosition = cameraObject.transform.position;
		movingLine = GameObject.Find("CameraMovingLine");
		//chasing camera
		firstJump = false;
		currentSpeed = initialSpeed;

		//Stage field
		stageField = GameObject.Find("StageField");


		//stages spawn initialize
		prefabsToRand = new List<GameObject>();
		recentStageObject = GameObject.Find("Stage0");


		//tutorial stage spawn
		if (PlayerPrefs.GetInt ("firstTime", 0) <= 1) {
			selectedPrefab = stageTutorialPrefab;
			spawnPosition = recentStageObject.transform.position;
			Destroy (recentStageObject); //destroy stage 0
			recentStageObject = (GameObject)Instantiate (selectedPrefab, spawnPosition, Quaternion.identity);
			recentStageObject.transform.parent = stageField.transform;
		}

		//stages inital spawn
		for (int i = 0; i < stagesInitialCount; i++) {
			spawnStages ();
		}


		//Background field
		backgroundField = GameObject.Find("BackgroundField");
		backgroundFieldPosition = backgroundField.transform.position;
		recentBackgroundObject = GameObject.Find("Background0");
		//transition
		//currentBackgroundTemplateNumber = 0;
		//Background inital spawn
		for (int i = 0; i < backgroundInitialCount; i++) {
			spawnBackground (i);
		}


		//restart initial parameter
		cameraInitialPosition = cameraObject.transform.position;
		playerInitialPosition = player.transform.position;

		//score text
		gameplayScoreText = GameObject.Find("CurrentScore").GetComponent<Text>();
		gameplayBestScoreText = GameObject.Find("CurrentBestScore").GetComponent<Text>();
		gameplayCoinText = GameObject.Find("TotalCoin").GetComponent<Text>();
		shopCoinText = GameObject.Find("ShopTotalCoin").GetComponent<Text>();

		gameoverScoreText = GameObject.Find ("Score").GetComponent<Text>();
		gameoverBestscoreText = GameObject.Find ("BestScore").GetComponent<Text>();
		newBestScoreText = GameObject.Find ("NewBestScoreLabel");

		//score and coin value
		score = 0;
		bestscore = PlayerPrefs.GetInt("bestscore",0);
		collectedCoin = PlayerPrefs.GetInt("coin",0);
		//update score text
		gameplayScoreText.text = "SCORE : "+score;
		gameplayBestScoreText.text = "BEST : "+bestscore;
		gameplayCoinText.text = "" + collectedCoin;
		shopCoinText.text = "" + collectedCoin;
		//get coin
		isGettingCoin = false;

		//weak platform
		isOnWeak = false;

		//Barrier
		barrier = GameObject.Find ("Barrier");
		barrierAnimator = barrier.GetComponent<Animator> ();
		barrier.SetActive (false);
		currentBarrierLifespan = 0;


		selectedCharacter = PlayerPrefs.GetInt("selectedCharacter",0);
		buyIndexCharacter = 0;
		for (int i = 0; i < characters.Length; i++) {

			//get purchase Status database
			if (i>0) {
				if (PlayerPrefs.GetInt("purchaseCharacter"+i,0)>0) {
					characters [i].purchased = true;
				} 
			}

			//each Character in shop UI
			tempCharacterObject = transform.Find ("Shop/Panel/ScrollView/Viewport/Content/Character" + i).gameObject;
			tempCharacterObject.transform.Find ("Image").GetComponent<Image> ().sprite = characters [i].image;
			if (selectedCharacter == i) {
				tempCharacterObject.transform.Find ("UseButton").gameObject.SetActive (false);
				tempCharacterObject.transform.Find ("Purchase").gameObject.SetActive (false);
			} else if (characters [i].purchased) {
				tempCharacterObject.transform.Find ("UsedLabel").gameObject.SetActive (false);
				tempCharacterObject.transform.Find ("Purchase").gameObject.SetActive (false);
			} else {
				tempCharacterObject.transform.Find ("UsedLabel").gameObject.SetActive (false);
				tempCharacterObject.transform.Find ("UseButton").gameObject.SetActive (false);
				tempCharacterObject.transform.Find ("Purchase/Price").GetComponent<Text> ().text = ""+characters [i].price;
			}

			tempCharacterObject.transform.Find ("ConfirmBuy").gameObject.SetActive (false);
		}
		// change player sprite
		player.GetComponent<SpriteRenderer> ().sprite = characters [selectedCharacter].image;

		



		//Hide UI
		howtoUI.SetActive (false);
		tutorialUI.SetActive (false);
		playUI.SetActive (false);
		gameoverUI.SetActive (false);
		pauseUI.SetActive (false);
		quitUI.SetActive (false);
		//shop UI
		shopUI.SetActive (false);
		notEnoughUI.SetActive (false);
		//wantToBuyUI.SetActive (false);
		afterAdsUI.SetActive (false);

	}


	private void FadeInSplash(){
		panelImage.CrossFadeAlpha(0.0f, 1.0f, false); //(alpha value, fade speed, not important)

		GooglePlayLogIn (); //start login when splash screen show

	}

	//google play LeaderBoard
	private void GooglePlayLogIn ()
	{
		Social.localUser.Authenticate ((bool success) =>
			{
				if (success) {
					//Debug.Log ("Login Sucess");
					//GameObject.Find("Leaderboard").SetActive(false);
					StartCoroutine(FadeOutSplash());
				} else {
					//Debug.Log ("Login failed");
					//GameObject.Find("Help").SetActive(false);
					StartCoroutine(FadeOutSplash());
				}
			});
	}

	IEnumerator FadeOutSplash(){
		yield return new WaitForSeconds(splash_DelayTime);
		panelImage.CrossFadeAlpha(1.0f, 1.0f, false);
		yield return new WaitForSeconds(1.5f);

		finishLoadingSplash ();
	}

	private void finishLoadingSplash(){
		splashUI.SetActive (false);
		//Finish Splash, start the sound
		if (muteState > 0) {
			muteONButton.SetActive (false);
			//BGMaudioSource.Stop (); //No need because play on awake is false
		} else {
			muteOFFButton.SetActive (false);
			BGMaudioSource.Play ();
		}
	}




	
	// Update is called once per frame
	void Update () {


		if (isPlaying) {

			//if back button pressed, pause game
			if (Input.GetKeyDown (KeyCode.Escape)) {
				clickPause ();
			}

			//temporary keyboard control
			/*if (Input.GetKey (KeyCode.RightArrow)) {
				clickJump (1);
			} else if (Input.GetKey (KeyCode.LeftArrow)) {
				clickJump (-1);
			}*/
			//stopping point
			if (currentPoint >= 0 && currentPoint <= 4) {
				if (Mathf.Abs (player.transform.position.x - point [currentPoint]) < 0.05f && !isStopped) {
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
			if (firstJump) {
				cameraPosition.y = cameraPosition.y + Time.deltaTime * currentSpeed;
				cameraObject.transform.position = cameraPosition;
				if (currentSpeed < maxSpeed) {
					currentSpeed = initialSpeed + cameraPosition.y * speedIncrement;
				}
			}

			//moving background
			backgroundFieldPosition.y = -(cameraPosition.y/backgroundSlowDown);
			backgroundField.transform.localPosition = backgroundFieldPosition;

			//update score label
			score = Mathf.RoundToInt (cameraPosition.y);
			gameplayScoreText.text = "SCORE : " + score;

			//get coin
			if (isGettingCoin) {
				SEaudioSource.PlayOneShot (eatSound, 0.6f);
				collectedCoin = collectedCoin + 1;
				gameplayCoinText.text = "" + collectedCoin;
				shopCoinText.text = "" + collectedCoin;
				isGettingCoin = false;
			}
			
		} else {
			//quiting
			if (Input.GetKeyDown (KeyCode.Escape)) {
				if (pauseUI.activeSelf) {
					clickResume ();
				} else if (gameoverUI.activeSelf) {
					clickExit ();
				} else if (howtoUI.activeSelf) {
					clickCloseHowTo();
				} else if (notEnoughUI.activeSelf) {
					clickExitNotEnough ();
				}else if (shopUI.activeSelf) {
					clickExitShop ();
				} else if (quitUI.activeSelf) {
					quitUI.SetActive (false);
				} else {
					quitUI.SetActive (true);
				}

				/*else if (wantToBuyUI.activeSelf) {
					clickExitWantToBuy ();
				}*/ 

			} 
		}
		
	}


	//leaderboard
	public void ClickShowLeaderBoard ()
	{
		//        Social.ShowLeaderboardUI (); // Show all leaderboard
		((PlayGamesPlatform)Social.Active).ShowLeaderboardUI (leaderboardID); // Show current (Active) leaderboard
	}

	private void addScoreToLeaderBoard (int score)
	{
		if (Social.localUser.authenticated) {
			Social.ReportScore (score, leaderboardID, (bool success) =>
				{
					if (success) {
						Debug.Log ("Update Score Success");

					} else {
						Debug.Log ("Update Score Fail");
					}
				});
		}
	}



	IEnumerator Shake() {
		Vector3 originalCamPos = cameraObject.transform.position;
		Vector3 temporaryCamPos = new Vector3(0, 0, originalCamPos.z);
		float elapsed = 0.0f;

		float percentComplete;
		float damper;

		while (elapsed < shakeDuration) {
			
			elapsed += Time.deltaTime;          

			percentComplete = elapsed / shakeDuration;         
			damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			// map random value to [-1, 1]
			temporaryCamPos.x = originalCamPos.x + (Random.value * 2.0f - 1.0f) * shakeMagnitude * damper;
			temporaryCamPos.y = originalCamPos.y + (Random.value * 2.0f - 1.0f) * shakeMagnitude * damper;

			cameraObject.transform.position = temporaryCamPos;

			yield return null;
		}

		cameraObject.transform.position = originalCamPos;
	}

	public void gameover(){
		
		if (isPlaying) {
			SEaudioSource.PlayOneShot (deathSound, 0.6f);
			isPlaying = false;
			//hide player
			player.SetActive (false);
			GameObject.Instantiate (playerExplodePrefab, player.transform.position, Quaternion.identity);
			//shake
			StartCoroutine (Shake ());



			firstJump = false;
			playUI.SetActive (false);
			gameoverUI.SetActive (true);

			//resume physic if paused
			pauseUI.SetActive (false);
			Time.timeScale = 1;

			if (reviveState) {
				reviveButton.SetActive (false);
			} else {
				reviveButton.SetActive (true);
			}


			newBestScoreText.SetActive (false);

			//collected coin
			PlayerPrefs.SetInt ("coin", collectedCoin);
			//collectedFoodTextMenu.text = "" + collectedFood;

			//best score
			if (score>bestscore) {
				PlayerPrefs.SetInt("bestscore", score);
				bestscore = score;
				newBestScoreText.SetActive(true);
			}
			gameoverScoreText.text = "" + score;
			gameoverBestscoreText.text = "" + bestscore;


			ads.addInterstitialCounter ();
			addScoreToLeaderBoard (score);
		}
	}

	public void clickStart(){

		menuUI.SetActive (false);
		if (PlayerPrefs.GetInt ("firstTime", 0) <= 0) {
			PlayerPrefs.SetInt ("firstTime", 1);
			clickHowTo ();
		} else if (PlayerPrefs.GetInt ("firstTime", 0) == 1) {
			playSelectSound ();
			tutorialUI.SetActive (true);
			TutorialTurnLeftInstruction ();
			isPlaying = true;
		} else {
			playSelectSound ();
			playUI.SetActive (true);
			isPlaying = true;
		}


	}

	public void tutorialClickJump(int direction){
		//direction -1 left, 1 right

		if (isGrounded) {
			//sound
			SEaudioSource.PlayOneShot (jumpSound, 0.6f);

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

			//update direction
			if (lastDirection != direction) {
				player.transform.Rotate (headRotate);
				//update last direction
				lastDirection = direction;
			}
				
		}
	}

	public void TutorialTurnLeftInstruction(){
		tutorialControl.SetActive (true);
		tutorialLeft.SetActive (true);
		tutorialRight.SetActive (false);
	}

	public void TutorialTurnRightInstruction(){
		tutorialControl.SetActive (true);
		tutorialLeft.SetActive (false);
		tutorialRight.SetActive (true);
	}

	public void TutorialEnd(){
		playSelectSound ();
		PlayerPrefs.SetInt ("firstTime", 2);
		tutorialUI.SetActive (false);
		playUI.SetActive (true);
	}

	public void clickRestart(){

		refresh ();

		clickStart ();
	}

	public void clickRevive(){
		ads.ShowRewardVideo ();
	}

	private void refresh(){
		//show player
		player.SetActive (true);

		//destroy all stage
		GameObject[] clones = GameObject.FindGameObjectsWithTag("Clone");
		foreach (GameObject clone in clones) {
			GameObject.Destroy(clone);
		}

		//destroy all background
		clones = GameObject.FindGameObjectsWithTag("CloneBackground");
		foreach (GameObject clone in clones) {
			GameObject.Destroy(clone);
		}

		//return to initial position
		backgroundField.transform.localPosition = Vector2.zero;
		cameraObject.transform.position = cameraInitialPosition;
		rigid.velocity = Vector2.zero;
		player.transform.position = playerInitialPosition;
		//don't draw trail when going back
		trail.Clear();

		//instantiate stage 0
		recentStageObject = (GameObject)Instantiate (stageInitialPrefab);
		recentStageObject.transform.parent = stageField.transform;

		//instantiate background 0
		recentBackgroundObject = (GameObject)Instantiate (backgroundInitialPrefab,backgroundField.transform);

		//reset UI
		playUI.SetActive (false);
		gameoverUI.SetActive (false);

		//initial parameter
		isGrounded = true;
		currentPoint = 2;
		isStopped = true;
		reviveState = false;

		//rotate to initial position
		if (lastDirection != 1) {
			player.transform.Rotate (headRotate);
			lastDirection = 1;
		}




		//moving camera
		cameraPosition = cameraObject.transform.position;
		//chasing camera
		firstJump = false;
		currentSpeed = initialSpeed;

		//tutorial stage spawn
		if (PlayerPrefs.GetInt ("firstTime", 0) <= 1) {
			selectedPrefab = stageTutorialPrefab;
			spawnPosition = recentStageObject.transform.position;
			Destroy (recentStageObject); //destroy stage 0
			recentStageObject = (GameObject)Instantiate (selectedPrefab, spawnPosition, Quaternion.identity);
			recentStageObject.transform.parent = stageField.transform;
		}

		//stages inital spawn
		for (int i = 0; i < stagesInitialCount; i++) {
			spawnStages ();
		}



		//transition
		//currentBackgroundTemplateNumber = 0;
		//Background inital spawn
		for (int i = 0; i < backgroundInitialCount; i++) {
			spawnBackground (i);
		}

		//score and coin value
		score = 0;
		bestscore = PlayerPrefs.GetInt("bestscore",0);
		//update score text
		gameplayScoreText.text = "SCORE : "+score;
		gameplayBestScoreText.text = "BEST : "+bestscore;
		//get coin
		isGettingCoin = false;

		//weak platform
		isOnWeak = false;

		//barrier
		if (barrier.activeSelf) {
			barrierAnimator.SetBool ("weak", false);
		}
		barrier.SetActive (false);
	}

	public void refreshRevive(){

		//destroy all stage
		GameObject[] clones = GameObject.FindGameObjectsWithTag("Clone");
		foreach (GameObject clone in clones) {
			GameObject.Destroy(clone);
		}


		//instantiate stage revive
		recentStageObject = (GameObject)Instantiate (stageRevivePrefab, GameObject.Find("StageRevivePosition").transform.position,Quaternion.identity);
		recentStageObject.transform.parent = stageField.transform;

		//Start the game !Important
		playUI.SetActive (true);
		gameoverUI.SetActive (false);
		isPlaying = true;

		//initial parameter
		isGrounded = true;
		currentPoint = 2;
		isStopped = true;
		//important!!!!
		reviveState = true;

		//rotate to initial position
		if (lastDirection != 1) {
			player.transform.Rotate (headRotate);
			lastDirection = 1;
		}

		//moving camera
		cameraPosition = cameraObject.transform.position;
		//chasing camera
		firstJump = false;

		//stages inital spawn
		for (int i = 0; i < stagesInitialCount; i++) {
			spawnStages ();
		}

		//get coin
		isGettingCoin = false;

		//weak platform
		isOnWeak = false;

		//show player
		player.SetActive (true);

		//return to initial position
		//cameraObject.transform.position = cameraInitialPosition; //not needed, camera stay on revive
		rigid.velocity = Vector2.zero;
		player.transform.position = GameObject.Find("PlayerRevivePosition").transform.position;
		//don't draw trail when going back
		trail.Clear();
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

	public void spawnBackground(int index){
		
		selectedBackgroundPrefab = backgroundTransition[index].background_prefabs; //select prefabs


		selectedBackgroundPrefabHeight = selectedBackgroundPrefab.GetComponent<SpriteRenderer> ().sprite.bounds.extents.y * selectedBackgroundPrefab.transform.localScale.y;
		recentBackgroundHeight = recentBackgroundObject.GetComponent<SpriteRenderer> ().sprite.bounds.extents.y * recentBackgroundObject.transform.localScale.y;
		backgroundSpawnPosition = recentBackgroundObject.transform.position;
		backgroundSpawnPosition.y = backgroundSpawnPosition.y + (recentBackgroundHeight + selectedBackgroundPrefabHeight);
		recentBackgroundObject = (GameObject)Instantiate (selectedBackgroundPrefab, backgroundSpawnPosition, Quaternion.identity);
		recentBackgroundObject.transform.parent = backgroundField.transform;
	}


	/*public void spawnBackground(){

		//else if reach last background
		for (int i = 1; i <= backgroundTemplates.Length; i++) {
			
			if (i == backgroundTemplates.Length) { //if reach last distance

				if (cameraPosition.y>=backgroundTemplates[i-1].distanceStart) { //check distance condition
					if (currentBackgroundTemplateNumber != i-1) {
						selectedBackgroundPrefab = backgroundTemplates[i-1].transition_prefabs; //select transition
						currentBackgroundTemplateNumber = i-1;
						print (currentBackgroundTemplateNumber);
					}else{
						selectedBackgroundPrefab = backgroundTemplates[i-1].background_prefabs; //select prefabs
						break;
					}
				}


			}else if (cameraPosition.y<backgroundTemplates[i].distanceStart) { //check distance condition
				
				if (currentBackgroundTemplateNumber != i-1) {
					selectedBackgroundPrefab = backgroundTemplates[i-1].transition_prefabs; //select transition
					currentBackgroundTemplateNumber = i-1;
					print (currentBackgroundTemplateNumber);
				}else{
					selectedBackgroundPrefab = backgroundTemplates[i-1].background_prefabs; //select prefabs
					break;
				}
					
			}


		}

		selectedBackgroundPrefabHeight = selectedBackgroundPrefab.GetComponent<SpriteRenderer> ().sprite.bounds.extents.y * selectedBackgroundPrefab.transform.localScale.y;
		recentBackgroundHeight = recentBackgroundObject.GetComponent<SpriteRenderer> ().sprite.bounds.extents.y * recentBackgroundObject.transform.localScale.y;
		backgroundSpawnPosition = recentBackgroundObject.transform.position;
		backgroundSpawnPosition.y = backgroundSpawnPosition.y + (recentBackgroundHeight + selectedBackgroundPrefabHeight);
		recentBackgroundObject = (GameObject)Instantiate (selectedBackgroundPrefab, backgroundSpawnPosition, Quaternion.identity);
		recentBackgroundObject.transform.parent = backgroundField.transform;
	}*/

	public void clickJump(int direction){
		//direction -1 left, 1 right

		if (isGrounded) {
			//sound
			SEaudioSource.PlayOneShot (jumpSound, 0.6f);

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

			//update direction
			if (lastDirection != direction) {
				player.transform.Rotate (headRotate);
				//update last direction
				lastDirection = direction;
			}


			//destroy weak platform
			if (isOnWeak) {
				GameObject.Instantiate (platformBreakPrefab, weakPlatform.transform.position, Quaternion.identity);
				Destroy (weakPlatform);
				isOnWeak = false;
			}
		}
	}

	public bool groundCheck(){
		if (rigid.velocity.y <= 0 && !isGrounded) {
			isGrounded = true;

			//barrier
			if (currentBarrierLifespan>0) {
				currentBarrierLifespan--;
				if (currentBarrierLifespan<=0) {
					barrierAnimator.SetBool ("weak", false);
					barrier.SetActive (false);
				}else if (currentBarrierLifespan==3) {
					barrierAnimator.SetBool ("weak", true);
				}
			}

		}
		return isGrounded;
	}

	public void getCoin(){
		isGettingCoin = true;
	}

	public void getPowerUpBarrier(){
		SEaudioSource.PlayOneShot (powerupSound, 0.6f);
		barrier.SetActive (true);
		barrierAnimator.SetBool ("weak", false);
		currentBarrierLifespan = barrierLifespan;
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

		//update direction
		if (lastDirection != direction) {
			player.transform.Rotate (headRotate);
			//update last direction
			lastDirection = direction;
		}
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


	public void clickPause(){
		if (isPlaying) {
			playSelectSound ();

			isPlaying = false;

			//pause physic
			pauseUI.SetActive (true);
			Time.timeScale = 0;
		}
	}

	public void clickResume(){
		if (!isPlaying) {
			playSelectSound ();

			isPlaying = true;

			//resume physic
			pauseUI.SetActive (false);
			Time.timeScale = 1;
		}
	}

	public IEnumerator screenshotAndShare(){

		yield return new WaitForEndOfFrame();

		//take screen shot
		Texture2D screenTexture = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);
		screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
		screenTexture.Apply();

		//save screen shot
		byte[] dataToSave = screenTexture.EncodeToPNG();
		string destination = Application.persistentDataPath+"/myscreenshot.png";
		File.WriteAllBytes(destination, dataToSave);


		//share
		if(!Application.isEditor)
		{
			//if UNITY_ANDROID
			string body = "Can you beat my best score?\n" +
				"https://play.google.com/store/apps/details?id=com.bekko.highleapninja";
			string subject = "High Leap Ninja score";

			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://" + destination);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body );
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
			intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

			// run intent from the current Activity
			currentActivity.Call("startActivity", intentObject);
		}


	}

	public void clickShare(){
		playSelectSound ();
		StartCoroutine (screenshotAndShare ());
	}

	public void clickExit(){
		playSelectSound ();
		menuUI.SetActive (true);
		tutorialUI.SetActive (false);

		//resume physic
		pauseUI.SetActive (false);
		Time.timeScale = 1;

		refresh ();


	}

	public void clickQuit(){
		playSelectSound ();
		Application.Quit();
	}

	public void clickCancelQuit(){
		playSelectSound ();
		quitUI.SetActive (false);
	}

	public void clickHowTo(){
		playSelectSound ();
		menuUI.SetActive (false);
		howtoUI.SetActive (true);
	}

	public void clickCloseHowTo(){
		if (PlayerPrefs.GetInt ("firstTime", 0) == 1) {
			clickStart ();
		} else {
			menuUI.SetActive (true);
			playSelectSound ();
		}
		howtoUI.SetActive (false);

	}

	public void clickMute(){
		if (muteState > 0) {
			muteOFFButton.SetActive (false);
			muteONButton.SetActive (true);
			BGMaudioSource.Play ();
			muteState = 0;
		} else {
			muteONButton.SetActive (false);
			muteOFFButton.SetActive (true);
			BGMaudioSource.Stop ();
			muteState = 1;
		}
		PlayerPrefs.SetInt("mutestate", muteState);
	}

	public void clickShop(){
		playSelectSound ();
		shopUI.SetActive (true);
	}

	public void clickExitShop(){
		playSelectSound ();
		shopUI.SetActive (false);

		//remove Confirm dialog
		transform.Find ("Shop/Panel/ScrollView/Viewport/Content/Character" + buyIndexCharacter + "/ConfirmBuy").gameObject.SetActive (false);
	}

	public void clickBuy(int indexCharacter){
		playSelectSound ();

		//remove other Confirm dialog
		transform.Find ("Shop/Panel/ScrollView/Viewport/Content/Character" + buyIndexCharacter + "/ConfirmBuy").gameObject.SetActive (false);

		if (collectedCoin >= characters [indexCharacter].price) {
			//wantToBuyUI.SetActive (true); //old way
			//display confirm dialog
			transform.Find ("Shop/Panel/ScrollView/Viewport/Content/Character" + indexCharacter + "/ConfirmBuy").gameObject.SetActive (true);
			buyIndexCharacter = indexCharacter;
		} else {
			notEnoughUI.SetActive (true);
		}
	}

	public void ClickConfirmBuy(){
		//clickExitWantToBuy ();

		//remove Confirm dialog
		transform.Find ("Shop/Panel/ScrollView/Viewport/Content/Character" + buyIndexCharacter + "/ConfirmBuy").gameObject.SetActive (false);

		//Use recent bought character
		clickUseCharacter (buyIndexCharacter);

		//parameter change
		characters [buyIndexCharacter].purchased = true;
		PlayerPrefs.SetInt ("purchaseCharacter" + buyIndexCharacter, 1);
		//pay food
		collectedCoin = collectedCoin - characters [buyIndexCharacter].price;
		PlayerPrefs.SetInt ("coin", collectedCoin);
		gameplayCoinText.text = "" + collectedCoin;
		shopCoinText.text = "" + collectedCoin;
	}

	public void clickUseCharacter(int indexCharacter){
		playSelectSound ();
		//UI change to used
		tempCharacterObject = transform.Find ("Shop/Panel/ScrollView/Viewport/Content/Character" + indexCharacter).gameObject;
		tempCharacterObject.transform.Find ("UsedLabel").gameObject.SetActive (true);
		tempCharacterObject.transform.Find ("Purchase").gameObject.SetActive (false);
		tempCharacterObject.transform.Find ("UseButton").gameObject.SetActive (false);
		//Change old Used label
		tempCharacterObject = transform.Find ("Shop/Panel/ScrollView/Viewport/Content/Character" + selectedCharacter).gameObject;
		tempCharacterObject.transform.Find ("UsedLabel").gameObject.SetActive (false);
		tempCharacterObject.transform.Find ("UseButton").gameObject.SetActive (true);

		//remove Confirm dialog
		transform.Find ("Shop/Panel/ScrollView/Viewport/Content/Character" + buyIndexCharacter + "/ConfirmBuy").gameObject.SetActive (false);

		//change parameter
		selectedCharacter = indexCharacter;
		PlayerPrefs.SetInt("selectedCharacter",selectedCharacter);

		//snake sprite change
		player.GetComponent<SpriteRenderer> ().sprite = characters [selectedCharacter].image;
		//refresh();
	}

	public void clickExitNotEnough (){
		playSelectSound ();
		notEnoughUI.SetActive (false);
		afterAdsUI.SetActive (false);
	}

	/*public void clickExitWantToBuy(){
		playSelectSound ();
		wantToBuyUI.SetActive (false);
	}*/

	public void adsCoinReward(int rewardAmount){
		//collected coin
		collectedCoin = collectedCoin + rewardAmount;
		PlayerPrefs.SetInt ("coin", collectedCoin);
		gameplayCoinText.text = "" + collectedCoin;
		shopCoinText.text = "" + collectedCoin;

		afterAdsUI.SetActive (true);
	}

	private void playSelectSound(){
		SEaudioSource.PlayOneShot (selectSound, 0.6f);
	}


}
