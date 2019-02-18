using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour {
	public bool leftTeam;
	private Rigidbody rb;
	private Vector3 startPos;
	public int number;
	[HideInInspector]
	public float rotationSpeed;
	public autoRotate eyeGear;
	public KeyCode keyT;

	private GameObject text;

	private float rotationDelay = 0;
	private float leakDelay = 0;

	[SerializeField]
	private float speed = 1;
	private float speedTarget = 1;
	private GameObject oil;
	[HideInInspector]
	public Transform aiTarget;

	public float energy;

	private void Start() {
		playerManager.addPlayer(leftTeam, this);
		rb = GetComponent<Rigidbody>();
		startPos = transform.position;
	}

	public void Update() {
		if(transform.position.y < -2) Reset();
		eyeGear.speed = rotationSpeed;
		//Speed
		speed = Mathf.Lerp(speed, speedTarget, Time.deltaTime * 2);

		//Leaking
		if(leakDelay > 0  && !oil) leakDelay--;
		rotationDelay += Time.deltaTime;
		if(rotationDelay > 0.4f) rotationDelay = 0;
		rb.velocity = transform.forward * rb.velocity.magnitude;
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
			oil = Instantiate(playerManager.self.oilPrefab, transform.position, Quaternion.Euler(0, startrot+(90*(i)), 0));
			oil.GetComponent<Rigidbody>().AddForce(300 * oil.transform.forward);
		}
		leakDelay = 5;
	}

	public void changeSpeed(float speedup) {
		speedTarget = speedup;
	}
}
