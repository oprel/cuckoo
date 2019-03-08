using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamestateVisuals : MonoBehaviour {
	public static gamestateVisuals self;

	[Header("Components")]
	public autoRotate gearLeft;
	public autoRotate gearRight;
	public speedChangeDisplay msgleft;
	public speedChangeDisplay msgright;
	public cameraShake cameraShake;
	public GameObject beakboostvisual;
	public TextMeshProUGUI scoreDisplay, timeDisplay, suddenDeath;
	public float particleOffset;
	public GameObject scoreParticles, falloutParticles, stunnedParticles;
	public Light areaLight;

	private GameObject hand;
	private GameObject smallHand;
	private GameObject doorRed, doorBlue;
	private GameObject[] redDoors, blueDoors;
	private steamController steamLeft, steamRight;

	public float tickSpeed = 0.1f, steamAmount = 2;
	private float time = 0, tickTime = 0;
	private bool alternate = false;
	private int scoreLeft, scoreRight;

	//Giant clock
	private Light clockLight;
	private float clockShowTime = 0;
	public float clockShowDuration = 10;
	private float clockMinDelay = 0;
	private float handBaseScale;
	private float smallHandBaseRot;

	public float darknessOnClockShow = 0.5f;
	private float baseDarkness;

	void Awake() {
		self = this;
		msgleft.gameObject.SetActive(true);
		msgright.gameObject.SetActive(true);
		hand = GameObject.FindGameObjectWithTag("Arena").transform.Find("hand1").gameObject;
		smallHand = GameObject.FindGameObjectWithTag("Arena").transform.Find("hand2").gameObject;
		handBaseScale = hand.transform.localScale.x;
		smallHandBaseRot = smallHand.transform.localEulerAngles.z;

		baseDarkness = areaLight.intensity;

		clockLight = GameObject.Find("Lighting/Clock Light").GetComponent<Light>();
		
		doorRed = GameObject.FindGameObjectWithTag("RedDoor");
		doorBlue = GameObject.FindGameObjectWithTag("BlueDoor");

		steamLeft = gearLeft.GetComponentInChildren<steamController>();
		steamRight = gearRight.GetComponentInChildren<steamController>();
		displayScore();
	}

	public void CloseDoors() {
		for(int i = 0; i < doorRed.transform.childCount; i++) doorRed.transform.GetChild(i).gameObject.SetActive(false);
		for(int i = 0; i < doorBlue.transform.childCount; i++) doorBlue.transform.GetChild(i).gameObject.SetActive(false);
	}

	void Update () {
		hand.transform.localScale = new Vector3(handBaseScale + Mathf.Sin(tickTime * 7) / 40, handBaseScale, hand.transform.localScale.z);
		if (gameManager.ended){
			areaLight.intensity = Mathf.Lerp(areaLight.intensity, baseDarkness, Time.deltaTime * 1);
			clockLight.intensity = Mathf.Lerp(clockLight.intensity, 0, Time.deltaTime * 1);

		} 
		if (playerManager.self.isCutscenePlaying()) return;
		scoreLeft = gameManager.self.scoreLeft;
		scoreRight = gameManager.self.scoreRight;
		
		gearLeft.speed = scoreLeft * -40;
		steamLeft.setRate(scoreLeft *steamAmount);
		gearRight.speed = scoreRight * 40;
		steamRight.setRate(scoreRight *steamAmount);
	
		time += Time.deltaTime;
		tickTime += Time.deltaTime;

		//Kleine wijzer
		smallHand.transform.localRotation = Quaternion.Euler(0, 0, smallHand.transform.localEulerAngles.z - (tickSpeed * 360 / (gameManager.self.gameTime)) / (gameManager.self.gameTime / 2));
		
		//Giant Clock tick
		hand.transform.rotation = Quaternion.Euler(0, 0, (hand.transform.eulerAngles.z + Mathf.Sin((1f - time) * 30) * (1f - time) / 3));
		
		//Ticking of master clock
		if(time > 1) {
			alternate = !alternate;
			hand.transform.rotation = Quaternion.Euler(0, 0, hand.transform.eulerAngles.z - tickSpeed * ((720 + 10) / (gameManager.self.gameTime)));
			playerManager.self.tickPlayers();
			if(alternate) audioManager.PLAY_STATIONARY("Tick1", clockShowTime / 50, 0.8f);
			else audioManager.PLAY_STATIONARY("Tick2", clockShowTime / 50, 0.8f);
			audioManager.PLAY_STATIONARY("Ride", 0.1f, 0.5f);
			time = 0;
		}
		if(gameManager.GetCurrentGameTime() < 0 && gameManager.self.scoreLeft == gameManager.self.scoreRight) suddenDeath.gameObject.SetActive(true);

		//Lighting
		clockLight.transform.position = new Vector3(Mathf.Lerp(10, -10, (tickTime / gameManager.self.gameTime)), clockLight.transform.position.y, clockLight.transform.position.z);
		if (gameManager.GetCurrentGameTime() < 15 - clockShowTime / 2) return;

		if(clockShowTime > 0) {
			clockShowTime -= Time.deltaTime;
			areaLight.intensity = Mathf.Lerp(areaLight.intensity, darknessOnClockShow, Time.deltaTime);
		} else {
			if(clockMinDelay > 0) clockMinDelay -= Time.deltaTime;
			areaLight.intensity = Mathf.Lerp(areaLight.intensity, baseDarkness, Time.deltaTime);
			int t = gameManager.GetCurrentGameTime();
			if((t % 30 == 0 || t <= 15) && 
				clockMinDelay <= 0 && t > 0 && t!= (int)gameManager.self.gameTime) {
				clockMinDelay = clockShowDuration;
				clockShowTime = clockShowDuration;
				StartCoroutine(showRemainingTime());
			}
		}
	}

	private IEnumerator showRemainingTime() {
		timeDisplay.gameObject.SetActive(true);
		string time = gameManager.GetCurrentGameTime().ToString().Replace("0","O");
		timeDisplay.text = time + " SECONDS LEFT";
		timeDisplay.canvasRenderer.SetAlpha(0.01f);
		timeDisplay.CrossFadeAlpha(1f, clockShowDuration / 4, false);
		float i = 1;
		while(i > .25f) {
			i = clockShowTime / (clockShowDuration);
			timeDisplay.characterSpacing = Mathf.SmoothStep(30f, 20f, i);
			yield return null;
			if (gameManager.GetCurrentGameTime() < 10) break;
		}
		timeDisplay.canvasRenderer.SetAlpha(1f);
		timeDisplay.CrossFadeAlpha(0f, clockShowDuration / 4, false);
		displayScore();
	}

	public static void screenShake() {
		self.cameraShake.ShakeCamera(.2f,.2f);
	}
	public static void hitStun(float t = 5) {
		//self.StartCoroutine(stun(t));
		return;
	}
	private static IEnumerator stun(float t) {
		float s = Time.timeScale;
		Time.timeScale = 0;
		for (int i = 0; i < t; i++) yield return new WaitForEndOfFrame();
		Time.timeScale = s;
	}

	public static void displayScore() {
		string txt = gameManager.self.scoreLeft.ToString() + "−" + gameManager.self.scoreRight.ToString();
		self.scoreDisplay.text = txt.Replace("0","O");
	}

	public static void scoreFeedback(bool leftGoal, int change) {
		hitStun();
		displayScore();
		if (leftGoal) {
			self.msgleft.speedChange(change > 0);
			Instantiate(self.scoreParticles, particlePos(self.gearLeft.transform.position), self.scoreParticles.transform.rotation);
		} else {
			self.msgright.speedChange(change > 0);
			Instantiate(self.scoreParticles, particlePos(self.gearRight.transform.position), self.scoreParticles.transform.rotation);
		}
	}

	public static Vector3 particlePos(Vector3 pos) {
		pos.y = 1;
		return pos - pos.normalized * self.particleOffset;
	}

	public static void fallOut(Vector3 pos) {
		if (gameManager.ended) return;
		Instantiate(self.falloutParticles, particlePos(pos * .8f), self.falloutParticles.transform.rotation);
	}

	public static void Stunned(Vector3 pos) {
		Instantiate(self.stunnedParticles, pos, self.stunnedParticles.transform.rotation);
	}
}
