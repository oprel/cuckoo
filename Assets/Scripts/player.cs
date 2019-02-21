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

	private class Bodypart {
		public Transform part;
		public Vector3 basePos, baseRot;
		
		public Bodypart(Transform trans) {
			part = trans;
			basePos = trans.position;
			baseRot = trans.eulerAngles;
		}
	}
	private Bodypart beakBottom, beakTop;
	private float animTime = 0;

	private void Start() {
		playerManager.addPlayer(leftTeam, this);
		rb = GetComponent<Rigidbody>();
		startPos = transform.position;
		if(disable) gameObject.SetActive(false);

		beakBottom = new Bodypart(transform.Find("sprites/beakbottom"));
		beakTop = new Bodypart(transform.Find("sprites/beaktop"));
	}

	public void Update() {
		if(transform.position.y < -2) Reset();
		eyeGear.speed = rotationSpeed;
		
		speed = Mathf.Lerp(speed, speedTarget, Time.deltaTime * 2);
	}

	private void FixedUpdate() {
		Vector3 vel = rb.velocity;
		vel.y = 0;
		rb.velocity = transform.forward * vel.magnitude + new Vector3(0, rb.velocity.y, 0);
	}

	public void Animate() {
		animTime += Time.deltaTime;
		audioManager.PLAY_SOUND("Chirp_Pos", transform.position, 100, Random.Range(0.9f, 1.2f));
		beakTop.part.rotation = Quaternion.Euler(beakTop.baseRot.x, Mathf.Sin(animTime*100)*10, beakTop.baseRot.z);
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
