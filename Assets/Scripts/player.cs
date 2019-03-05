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

	public float activityTimer = 0;

	private GameObject text;

	[SerializeField]
	private float speed = 1;
	private float speedTarget = 1;
	[HideInInspector]
	public Transform aiTarget;

	public float energy;
	public bool disable = false;

	private Vector3 lookPos;
	private bool fallingOut;

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

	private Transform eyeBall;

	//Stun
	private float stunTime, stunRot;
	public ParticleSystem stunParticles;

	private bool moveForward = true;

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
		eyeBall = transform.Find("sprites/eyeball");
	}

	public void Update() {
		if(transform.position.y < -5 && !fallingOut) StartCoroutine(fallout());
		eyeGear.speed = rotationSpeed;
		
		speed = Mathf.Lerp(speed, speedTarget, Time.deltaTime * 2);
		if(!isStunned() && !playerManager.self.isCutscenePlaying()) {
			eyeBall.rotation = Quaternion.Euler(eyeBall.eulerAngles.x, Vector3.RotateTowards(eyeBall.position, lookPos, 180, 0).y, eyeBall.eulerAngles.z);
		}
	}

	public void setEyeRotation(float rot) {
		eyeBall.rotation = Quaternion.Euler(eyeBall.eulerAngles.x, rot, eyeBall.eulerAngles.z);
	}
	private void lookAt(Vector3 pos) {
		lookPos = pos;
	}

	private void FixedUpdate() {
		activityTimer += Time.deltaTime;

		//Dazed
		if(isStunned()) {
			stunTime -= Time.deltaTime;
			eyeBall.rotation = Quaternion.Euler(eyeBall.eulerAngles.x, stunTime * 720, eyeBall.eulerAngles.z);
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x, stunRot + Mathf.Sin(Time.time*12)*7, transform.eulerAngles.z);
		} else eyeBall.rotation = Quaternion.Euler(eyeBall.eulerAngles.x, 0, eyeBall.eulerAngles.z);

		Vector3 vel = rb.velocity;
		vel.y = 0;
		if( moveForward && !isStunned()) 
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

	public void ResetActivityDelay() {
		activityTimer = 0;
	}
	
	public void impulse(float force) {
		if(isStunned()) return;
		rb.AddForce(transform.forward * force * speed);
		ResetActivityDelay();
	}

	public void Reset() {
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.position = startPos;
		ResetActivityDelay();
	}

	public void changeSpeed(float speedup) {
		speedTarget = speedup;
	}

	void OnCollisionEnter(Collision col) { //Stun
		if(col.gameObject.tag == "Hitter" && !isStunned()) {
			GameObject t = KooKoo.FindParentWithTag(col.transform.parent.gameObject, "Player");
			player other = t.GetComponent<player>();
			if(other.energy < 4|| other.leftTeam == leftTeam) return;
			GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * playerManager.self.beakBoostOnPlayers, t.transform.position);
			Instantiate(gamestateVisuals.self.beakboostvisual, transform.position, Quaternion.identity);
			gamestateVisuals.hitStun();
			stunTime = Mathf.Clamp(activityTimer * playerManager.self.stunTimeMultiplier, 3, 10);
			audioManager.PLAY_SOUND("Hit", transform.position, 10f, 1f);
			audioManager.PLAY_SOUND("BeakStun", transform.position, 30f, Random.Range(0.9f, 1.2f));
			stunRot = transform.eulerAngles.y;
			StartCoroutine(playStunParticles());
		}
		lookAt(col.transform.position);
	}
	private IEnumerator playStunParticles() {
		gamestateVisuals.Stunned(transform.position);
		stunParticles.Play();
		yield return new WaitForSeconds(playerManager.self.stunTimeMultiplier); //
		stunParticles.Stop();
	}

	public bool isStunned() {
		return stunTime > 0;
	}

	public IEnumerator suspendForwardMovement(float length){
		moveForward = false;
		yield return new WaitForSeconds(length);
		moveForward = true;
	}

	private IEnumerator fallout(){
		fallingOut = true;
		gamestateVisuals.fallOut(transform.position);
		yield return new WaitForSeconds(2f);
		Reset();
		fallingOut = false;
	}
}
