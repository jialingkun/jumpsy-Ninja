using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using GoogleMobileAds.Api;

public class Ads : MonoBehaviour {

	private BannerView bannerViewTop;
	private BannerView bannerViewBottom;
	private InterstitialAd interstitial;
	//private RewardBasedVideoAd rewardVideo;


	private string bannerID;
	private string interstitialID;
	//private string rewardVideoID;
	private AdRequest bannerAdRequest;


	//unity ads android id
	private string gameId = "1623155";

	public int interstitialShowAfter;
	private int interstitialCounter; 


	//specific script for video ads handler
	private Game_Script gameScript;
	private bool freeCoinReward;

	void Start () {
		//bannerID = "ca-app-pub-3940256099942544/6300978111";
		//interstitialID = "ca-app-pub-3940256099942544/1033173712";
		//rewardVideoID = "ca-app-pub-3940256099942544/5224354917";

		interstitialID = "ca-app-pub-8796483174280591/3338516036";
		//rewardVideoID = "ca-app-pub-8796483174280591/7181540275";

		//bannerAdRequest = new AdRequest.Builder().Build();

		interstitialCounter = 0;
		RequestInterstitial ();

		//rewardVideo = RewardBasedVideoAd.Instance;
		//rewardVideo.OnAdClosed += HandleOnAdClosed;
		//rewardVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
		//RequestRewardVideo ();


		//Initialize unity ads
		if (Advertisement.isSupported) {
			Advertisement.Initialize (gameId, true);
		}


		//specific script for video ads handler
		gameScript = this.GetComponent<Game_Script>();
		freeCoinReward = false;

	}

	/*
	public void showBannerBottom(){
		string adUnitId = bannerID;

		if (bannerViewTop!=null) {
			bannerViewTop.Hide ();
		}

		if (bannerViewBottom != null) {
			bannerViewBottom.Show ();
		} else {
			// Create a 320x50 banner at the top of the screen.
			bannerViewBottom = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
			// Load the banner with the request.
			bannerViewBottom.LoadAd(bannerAdRequest);
		}



	}

	public void showBannerTop(){
		string adUnitId = bannerID;

		if (bannerViewBottom != null) {
			bannerViewBottom.Hide ();
		
		}

		if (bannerViewTop != null) {
			bannerViewTop.Show ();
		} else {
			// Create a 320x50 banner at the top of the screen.
			bannerViewTop = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);
			// Load the banner with the request.
			bannerViewTop.LoadAd(bannerAdRequest);
		}

	}
	*/

	private void RequestInterstitial(){
		if (interstitial != null) {
			interstitial.Destroy ();
		}
		string adUnitId = interstitialID;
		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);
	}

	public void showInterstitial(){
		if (interstitial.IsLoaded ()) {
			interstitial.Show ();
			RequestInterstitial ();
		} else {
			RequestInterstitial ();
		}
	}

	public void addInterstitialCounter(){
		interstitialCounter++;
		if (interstitialShowAfter<=interstitialCounter) {
			showInterstitial ();
			interstitialCounter = 0;
		}

	}

	/*
	private void RequestRewardVideo(){
		string adUnitId = rewardVideoID;

		AdRequest request = new AdRequest.Builder().Build();
		rewardVideo.LoadAd(request, adUnitId);
	}

	public void ShowRewardVideo(){
		if (rewardVideo.IsLoaded()) {
			rewardVideo.Show ();
		} else {
			RequestRewardVideo ();
		}
	}

	public void HandleOnAdClosed(object sender, System.EventArgs args){
		RequestRewardVideo ();
		freeCoinReward = false;
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		//specific script
		if (freeCoinReward) {
			freeCoinReward = false;
			gameScript.adsFoodReward (30);
		} else {
			gameScript.refreshRevive();
		}

	}
	*/


	//unity reward video
	public void ShowRewardVideo ()
	{
		ShowOptions options = new ShowOptions();
		options.resultCallback = HandleShowResult;

		Advertisement.Show("rewardedVideo", options);

	}

	void HandleShowResult (ShowResult result)
	{
		if(result == ShowResult.Finished) {
			//Debug.Log("Video completed - Offer a reward to the player");
			if (freeCoinReward) {
				freeCoinReward = false;
				gameScript.adsCoinReward (300);
			} else {
				gameScript.refreshRevive();
			}

		}else if(result == ShowResult.Skipped) {
			//Debug.LogWarning("Video was skipped - Do NOT reward the player");

		}else if(result == ShowResult.Failed) {
			Debug.LogError("Video failed to show");
		}
		freeCoinReward = false;
	}

	public void startFreeCoinReward(){
		freeCoinReward = true;
		ShowRewardVideo ();

	}

}
