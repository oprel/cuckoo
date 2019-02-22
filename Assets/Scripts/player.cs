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
	private Bodypart beakBottom, beakTop, hairTop, hairBottom;
	private float animTime = 0;
	private float kwispelSpeed = 10;

	private void Start() {
		playerManager.addPlayer(leftTeam, this);
		rb = GetComponent<Rigidbody>();
		startPos = transform.position;
		if(disable) gameObject.SetActive(false);

		beakBottom = new Bodypart(transform.Find("sprites/beakbottom"));
		beakTop = new Bodypart(transform.Find("sprites/beaktop"));
		if(leftTeam) hairTop = new Bodypart(transform.Find("sprites/hair"));
		else {
			hairTop = new Bodypart(transform.Find("sprites/hairtop"));
			hairBottom = new Bodypart(transform.Find("sprites/hairbottom"));
		}
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
	
		if(hairTop != null) {
			if(hairBottom != null) {
				hairTop.part.localEulerAngles = new Vector3(hairTop.part.localEulerAngles.x, Mathf.Sin(Time.time * kwispelSpeed + (number*20)) * 3, hairTop.part.localEulerAngles.z);
				hairBottom.part.localEulerAngles = new Vector3(hairBottom.part.localEulerAngles.x, Mathf.Cos(Time.time * (kwispelSpeed + 2) + (number*20)) * 2.6f, hairBottom.part.localEulerAngles.z);
			} else hairTop.part.localEulerAngles = new Vector3(hairTop.part.localEulerAngles.x, Mathf.Sin(Time.time * kwispelSpeed + (number*20)) * 3, hairTop.part.localEulerAngles.z);
		}
	}

	public void Deanimate() {
		kwispelSpeed = 10;
		if(!leftTeam) {
			beakTop.part.localEulerAngles = new Vector3(beakTop.part.localEulerAngles.x, 0, beakTop.part.localEulerAngles.z);
			beakBottom.part.localEulerAngles = new Vector3(beakBottom.part.localEulerAngles.x, 0, beakBottom.part.localEulerAngles.z);
		}
	}

	public void Animate() {
		animTime += Time.deltaTime;
		if(Random.Range(0, 100) < 10) {
			if(!leftTeam) audioManager.PLAY_SOUND("Chirp_Pos", transform.position, 100, Random.Range(0.9f, 1.2f));
			else audioManager.PLAY_SOUND("Chirp_Neg", transform.position, 100, Random.Range(0.9f, 1.2f));
			audioManager.PLAY_SOUND("Hit", transform.position, 0.2f, Random.Range(2.4f, 2.7f));
		}
		if(!leftTeam) {
			kwispelSpeed = 50;
			beakTop.part.rotation = Quaternion.Euler(beakTop.baseRot.x, Mathf.LerpAngle(beakTop.part.eulerAngles.y, Mathf.Sin(animTime*120 + (number*20))*5-2.5f, animTime * 20), beakTop.part.eulerAngles.z);
			beakBottom.part.rotation = Quaternion.Euler(beakBottom.part.eulerAngles.x, Mathf.LerpAngle(beakBottom.part.eulerAngles.y, Mathf.Sin(animTime*110 + (number*20))*2.5f-10, animTime * 20), beakBottom.part.eulerAngles.z);
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Mathf.LerpAngle(transform.eulerAngles.y, Mathf.Sin(animTime*110 + (number*20))*4f-90, animTime * 20), transform.eulerAngles.z);
		} else {
			kwispelSpeed = 5;
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Mathf.LerpAngle(transform.eulerAngles.y, Mathf.Sin(animTime*20 + (number*20))*2f+90, animTime * 20), transform.eulerAngles.z);
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

	public void changeSpeed(float speedup) {
		speedTarget = speedup;
	}
}
