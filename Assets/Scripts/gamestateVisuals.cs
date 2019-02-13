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

	public float tickSpeed = 0.1f;
	private float time = 0;

	void Awake() {
		self = this;
		msgleft.gameObject.SetActive(true);
		msgright.gameObject.SetActive(true);
		hand = GameObject.FindGameObjectWithTag("Arena").transform.Find("arenaNew/clockborder/hand1").gameObject;
	}
	
	void Update () {
		gearLeft.speed = gameManager.self.scoreLeft * -40;
		gearRight.speed = gameManager.self.scoreRight * 40;
	
		time += Time.deltaTime;

		if(time > 1) {
			hand.transform.rotation = Quaternion.Euler(0, 0, hand.transform.eulerAngles.z - tickSpeed);
			time = 0;
		}
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
}
