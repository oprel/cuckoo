using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballSpawner : MonoBehaviour {
	public GameObject ballPrefab;
	public float frequency;
	public float radius;
	private float timer, resettimer = 0;

	private Transform house, doorL, doorR, hinge, fakeBall, drop;
	private Vector3 doorLBase, doorRBase, hingeBase;

	private bool doorsOpen = false, armExtended = false;

	private Vector3 pos;

	private bool flipped = false;

	void Start () {
		house = transform.GetChild(0);
		doorL = house.Find("Door_L");
		doorR = house.Find("Door_R");
		hinge = house.Find("Hinge");
		fakeBall = house.Find("FakeBall");
		drop = hinge.Find("DropPoint");
		doorRBase = doorR.localPosition;
		doorLBase = doorL.localPosition;
		hingeBase = new Vector3(hinge.localScale.x, 0.35f, hinge.localScale.z);
		hinge.localScale = hingeBase;
		house.gameObject.SetActive(false);
		
		//Debug
		timer = frequency / 2;
	}
	
	void Update () {
		if(resettimer <= 0) timer += Time.deltaTime;
		
		//Ball spawning
		if(timer > frequency && resettimer <= 0) {
			timer = 0;
			spawn();
		}
		fakeBall.position = drop.position;

		if(resettimer > 0) {
			resettimer -= Time.deltaTime;
			hinge.localScale = new Vector3(hingeBase.x, Mathf.Lerp(hinge.localScale.y, hingeBase.y, Time.deltaTime * 3), hingeBase.z);
			if(resettimer < 1) {
				doorL.localPosition = new Vector3(Mathf.Lerp(doorL.localPosition.x, doorLBase.x, Time.deltaTime * 4), doorL.localPosition.y, doorL.localPosition.z);
				doorR.localPosition = new Vector3(Mathf.Lerp(doorR.localPosition.x, doorRBase.x, Time.deltaTime * 4), doorR.localPosition.y, doorR.localPosition.z);
			}
		}
		else if(armExtended && doorsOpen) reset();
	}

	private void reset() {
		timer = 0;
		armExtended = doorsOpen = false;
		house.gameObject.SetActive(false);
		fakeBall.gameObject.SetActive(true);
	}

	private void spawn() {
		if(Random.Range(0, 2) == 0) flipped = true;
		else flipped = false;

		if(flipped) transform.localRotation = Quaternion.Euler(0, 180, 0);
		else transform.localRotation = Quaternion.Euler(0, 0, 0);

		pos = Random.insideUnitSphere * radius;
		pos.y = 0;
		activate();
		StartCoroutine("openDoors");
	}

	private void activate() {
		house.localPosition = new Vector3(pos.x, house.localPosition.y, house.localPosition.z);
		house.gameObject.SetActive(true);
	}

	IEnumerator extendArm() {
		while(!armExtended) {
			if(!reachedPoint()) hinge.localScale = new Vector3(hingeBase.x, hinge.localScale.y + 0.04f, hingeBase.z);
			yield return new WaitForSeconds(.02f);
			if(reachedPoint()) {
				Instantiate(ballPrefab, transform.position + pos, transform.rotation);
				fakeBall.gameObject.SetActive(false);
				armExtended = true;
			}
		}
		StopCoroutine("extendArm");
		StopCoroutine("openDoors");
		resettimer = 2;
	}

	IEnumerator openDoors() {
		while(!doorsOpen) {
			doorL.localPosition = new Vector3(Mathf.Lerp(doorL.localPosition.x, -0.1f, Time.deltaTime * 2), doorL.localPosition.y, doorL.localPosition.z);
			doorR.localPosition = new Vector3(Mathf.Lerp(doorR.localPosition.x, 0.1f, Time.deltaTime * 2), doorR.localPosition.y, doorR.localPosition.z);
			yield return new WaitForSeconds(.01f);
			if(doorL.localPosition.x <= -0.075f) {
				StartCoroutine("extendArm");
				doorsOpen = true;
			}
		}
	}

	private bool reachedPoint() {
		return drop.position.z * ((flipped)?-1:1) <= pos.z * ((flipped)?-1:1);
	}

	private void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(transform.position,radius);
	}
}
