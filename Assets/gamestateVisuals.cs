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

	void Awake(){
		self = this;
	}
	
	// Update is called once per frame
	void Update () {
		gearLeft.speed = gameManager.scoreLeft * -10;
		gearRight.speed = gameManager.scoreRight * 10;
	}

	public static void hitStun(float t = 5){
		self.StartCoroutine(stun(t));
		self.cameraShake.ShakeCamera(10,.1f);
	}
	private static IEnumerator stun(float t){
		float s = Time.timeScale;
		Time.timeScale = 0;
		for (int i = 0; i < t; i++)
		{
			yield return new WaitForEndOfFrame();
		}
		Time.timeScale=s;
	}

}
