using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamestateVisuals : MonoBehaviour {
	public static gamestateVisuals self;
	[Header("Components")]
	public autoRotate gearLeft;
	public autoRotate gearRight;
	public speedChangeDisplay msgleft;
	public speedChangeDisplay msgright;
	public cameraShake cameraShake;
	public GameObject beakboostvisual;

	private GameObject hand;
	private GameObject smallHand;

	public float tickSpeed = 0.1f;
	private float time = 0, tickTime = 0;
	private bool alternate = false;

	//Giant clock
	private Light clockLight;
	private float clockShowTime = 0;
	private float baseIntensity;
	public float clockShowDuration = 10;
	private float clockMinDelay = 0;
	private float handBaseScale;
	private float smallHandBaseRot;

	void Awake() {
		self = this;
		msgleft.gameObject.SetActive(true);
		msgright.gameObject.SetActive(true);
		hand = GameObject.FindGameObjectWithTag("Arena").transform.Find("arenaNew/clockborder/hand1").gameObject;
		smallHand = GameObject.FindGameObjectWithTag("Arena").transform.Find("arenaNew/clockborder/hand2").gameObject;
		handBaseScale = hand.transform.localScale.x;
		smallHandBaseRot = smallHand.transform.localEulerAngles.z;

		clockLight = GameObject.Find("Lighting/Clock Light").GetComponent<Light>();
		baseIntensity = clockLight.intensity;
		clockShowTime = clockShowDuration;	
		clockMinDelay = clockShowDuration;
	}
	
	void Update () {
		hand.transform.localScale = new Vector3(handBaseScale + Mathf.Sin(tickTime*4)/20, handBaseScale, hand.transform.localScale.z);

		gearLeft.speed = gameManager.self.scoreLeft * -40;
		gearRight.speed = gameManager.self.scoreRight * 40;
	
		time += Time.deltaTime;
		tickTime += Time.deltaTime;

		//Kleine wijzer laat zien wie voor staat 
		 
		if(gameManager.self.scoreRight != gameManager.self.scoreLeft && (gameManager.self.scoreRight > 0 || gameManager.self.scoreLeft > 0)) {
			if(gameManager.self.scoreRight > gameManager.self.scoreLeft) smallHand.transform.localRotation = Quaternion.Euler(smallHand.transform.localEulerAngles.x, smallHand.transform.localEulerAngles.y, Mathf.LerpAngle(smallHand.transform.localEulerAngles.z, 90, Time.deltaTime));
			else smallHand.transform.localRotation = Quaternion.Euler(smallHand.transform.localEulerAngles.x, smallHand.transform.localEulerAngles.y, Mathf.LerpAngle(smallHand.transform.localEulerAngles.z, -90, Time.deltaTime));
		}
		else smallHand.transform.localRotation = Quaternion.Euler(smallHand.transform.localEulerAngles.x, smallHand.transform.localEulerAngles.y, Mathf.LerpAngle(smallHand.transform.localEulerAngles.z, smallHandBaseRot, Time.deltaTime));
		

		//Giant Clock tick
		if(clockShowTime > 0) {
			clockShowTime -= Time.deltaTime;
			clockLight.intensity = Mathf.Lerp(clockLight.intensity, baseIntensity, Time.deltaTime);
		} else {
			if(clockMinDelay > 0) clockMinDelay -= Time.deltaTime;
			clockLight.intensity = Mathf.Lerp(clockLight.intensity, 0, Time.deltaTime * 2);
			if(Random.Range(0, 500) < 2 && clockMinDelay <= 0) {
				clockMinDelay = clockShowDuration;
				clockShowTime = clockShowDuration;
			}
		}

		//Ticking of master clock
		if(time > 1) {
			alternate = !alternate;
			hand.transform.rotation = Quaternion.Euler(0, 0, hand.transform.eulerAngles.z - tickSpeed);
			playerManager.self.tickPlayers();
			if(alternate) audioManager.PLAY_STATIONARY("Tick1", clockShowTime / 50, 0.8f);
			else audioManager.PLAY_STATIONARY("Tick2", clockShowTime / 50, 0.8f);
			audioManager.PLAY_STATIONARY("Ride", 0.1f, 0.5f);
			time = 0;
		}
	}

	public static void screenShake() {
		self.cameraShake.ShakeCamera(.2f,.2f);
	}
	public static void hitStun(float t = 5){
		//self.StartCoroutine(stun(t));
		return;
	}
	private static IEnumerator stun(float t) {
		float s = Time.timeScale;
		Time.timeScale = 0;
		for (int i = 0; i < t; i++) yield return new WaitForEndOfFrame();
		Time.timeScale = s;
	}
}
