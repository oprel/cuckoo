using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour {
	public bool leftTeam;
	private Rigidbody rb;
	private Vector3 startPos;
	public int number;

	public KeyCode keyT;

	private GameObject text;

	private float rotationDelay = 0;
	private float lastRot = 0;
	private float leakDelay = 0;

	[SerializeField]
	private float speed = 1;
	private float speedTarget = 1;

	private void Start() {
		playerManager.addPlayer(leftTeam, this);
		rb = GetComponent<Rigidbody>();
		startPos = transform.position;
		text = Instantiate(playerManager.self.textPrefab, transform.position, Quaternion.Euler(90, 0, 0));
		text.GetComponent<TextMesh>().text = keyT.ToString();
		lastRot = transform.rotation.y;
	}

	public void Update() {
		if(transform.position.y < -2) Reset();

		//Speed
		speed = Mathf.Lerp(speed, speedTarget, Time.deltaTime * 2);

		//Key text
		text.transform.position = transform.position;

		//Leaking
		if(leakDelay > 0) leakDelay--;
		rotationDelay += Time.deltaTime;
		if(rotationDelay > 0.4f && keyT == KeyCode.A) {
			if(Mathf.Abs(lastRot - transform.rotation.y) > 1 && leakDelay <= 0) leak();
			rotationDelay = 0;
			lastRot = transform.rotation.y;
		}
	}
	
	public void impulse(float force) {
		rb.AddForce(transform.forward * force * speed);
	}

	public void Reset() {
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.position = startPos;
	}

	public void leak() {
		float startrot = Random.Range(0, 360);
		for(int i = 1; i <= 3; i++) {
			GameObject oil = Instantiate(playerManager.self.oilPrefab, transform.position, Quaternion.Euler(0, startrot+(90*(i)), 0));
			oil.GetComponent<Rigidbody>().AddForce(300 * oil.transform.forward);
		}
		leakDelay = 5;
	}

	public void changeSpeed(float speedup) {
		speedTarget = speedup;
	}
}
