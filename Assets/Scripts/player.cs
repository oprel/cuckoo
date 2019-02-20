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

	private GameObject text;

	[SerializeField]
	private float speed = 1;
	private float speedTarget = 1;
	[HideInInspector]
	public Transform aiTarget;

	public float energy;

	public bool disable = false;

	private void Start() {
		playerManager.addPlayer(leftTeam, this);
		rb = GetComponent<Rigidbody>();
		startPos = transform.position;
		if(disable) gameObject.SetActive(false);
	}

	public void Update() {
		if(transform.position.y < -2) Reset();
		eyeGear.speed = rotationSpeed;
		
		//Speed
		speed = Mathf.Lerp(speed, speedTarget, Time.deltaTime * 2);
	}

	private void FixedUpdate() {
		Vector3 vel = rb.velocity;
		vel.y = 0;
		rb.velocity = transform.forward * vel.magnitude + new Vector3(0, rb.velocity.y, 0);
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

	public void changeSpeed(float speedup) {
		speedTarget = speedup;
	}
}
