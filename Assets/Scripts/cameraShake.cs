using UnityEngine;
using System.Collections;
 
public class cameraShake : MonoBehaviour {
	public bool debugMode = false;
 
	public float shakeAmount;
	public float shakeDuration; 
 
	float shakePercentage;
	float startAmount;
	float startDuration;
    Quaternion origRot;
 
	bool isRunning = false;
 
	//Rotation
	public bool smooth;
	public float smoothAmount = 5f;

	private float basePos;

	void Awake () {
        origRot = transform.rotation;
		if(debugMode) ShakeCamera ();
		basePos = transform.position.y;
		transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
	}
 
	//Intro zoom animation
	public void ZoomCamera() {
		transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, basePos, Time.deltaTime * 1), transform.position.z);
	}
 
	void ShakeCamera() {
		startAmount = shakeAmount;
		startDuration = shakeDuration;
 
		if (!isRunning) StartCoroutine (Shake());
	}
 
	public void ShakeCamera(float amount, float duration) {
		shakeAmount += amount;
		startAmount = shakeAmount;
		shakeDuration += duration; 
		startDuration = shakeDuration;
 
		if(!isRunning) StartCoroutine (Shake());
	}
 
 
	IEnumerator Shake() {
		isRunning = true;
		while (shakeDuration > 0.1f) {
			Vector3 rotationAmount = Random.insideUnitSphere * shakeAmount;
			rotationAmount.z = 0;
			shakePercentage = shakeDuration / startDuration; 
			shakeAmount = startAmount * shakePercentage;
			shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime); 
 
			if(smooth) transform.rotation = origRot * Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
			else transform.rotation = origRot * Quaternion.Euler (rotationAmount); 
			yield return null;
		}
		transform.rotation = origRot;
		isRunning = false;
	}
}