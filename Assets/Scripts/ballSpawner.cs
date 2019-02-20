﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballSpawner : MonoBehaviour {
	public static ballSpawner self;

	public GameObject ballPrefab, clockPrefab;
	public float frequency;
	public float radius;
	private float timer, resettimer = 0;

	private Transform house, hinge, fakeBall, drop, clock;
	public Transform doorL,doorR;
	private Vector3 doorLBase, doorRBase, hingeBase, fakeBallBase;

	private bool doorsOpen = false, armExtended = false;
	private Vector3 pos;
	private bool flipped = false, red = false;

	private static int ballAmount;
	public int maxBalls = 9;

	public bool dropClock = true;

	void Start () {
		self = this;
		ballAmount = 2;
		house = transform.GetChild(0);
		hinge = house.Find("Hinge");
		fakeBall = house.Find("FakeBall");
		clock = house.Find("FakeClock");
		drop = hinge.Find("DropPoint");
		doorRBase = doorR.localPosition;
		doorLBase = doorL.localPosition;
		hingeBase = new Vector3(hinge.localScale.x, 0.35f, hinge.localScale.z);
		hinge.localScale = hingeBase;
		fakeBallBase = fakeBall.localScale;
		fakeBall.localScale = new Vector3(0, 0, 0);
		house.gameObject.SetActive(false);
		for (int i = 0; i < 4; i++) spawn(true);
		dropClock = Camera.main.GetComponent<Cutscene>().playCutscene;
		clock.gameObject.SetActive(dropClock);
		fakeBall.gameObject.SetActive(!dropClock);
	}
	
	void FixedUpdate () {
		if(resettimer <= 0) timer += Time.deltaTime;
		
		//Ball spawning
		if(timer > frequency && resettimer <= 0 && ballAmount < maxBalls && !dropClock) {
			timer = 0;
			spawn(Random.value > .8f);
		}
		fakeBall.position = clock.position = drop.position;

		if(resettimer > 0) {
			resettimer -= Time.deltaTime;
			hinge.localScale = new Vector3(hingeBase.x, Mathf.Lerp(hinge.localScale.y, hingeBase.y, Time.deltaTime * 3), hingeBase.z);
			if(resettimer < 1) {
				doorL.localPosition = new Vector3(Mathf.Lerp(doorL.localPosition.x, doorLBase.x, Time.deltaTime * 4), doorL.localPosition.y, doorL.localPosition.z);
				doorR.localPosition = new Vector3(Mathf.Lerp(doorR.localPosition.x, doorRBase.x, Time.deltaTime * 4), doorR.localPosition.y, doorR.localPosition.z);
			}
		}
		else if(armExtended && doorsOpen && !dropClock) reset();
	}

	public void ResetClock() {
		timer = 0;
		armExtended = doorsOpen = false;
		if(house != null) house.gameObject.SetActive(false);
		if(fakeBall != null) {
			fakeBall.localScale = new Vector3(0, 0, 0);
			fakeBall.gameObject.SetActive(true);
		}
		dropClock = false;
		if(clock != null) clock.gameObject.SetActive(false);
	}

	public static void decrementBalls() {
		ballAmount--;
		if(ballAmount < 0) ballAmount = 0;
	}

	public void DropClock() {
		activate();
		StartCoroutine("openDoors");
	}

	private void reset() {
		timer = 0;
		armExtended = doorsOpen = false;
		house.gameObject.SetActive(false);
		fakeBall.localScale = new Vector3(0, 0, 0);
		fakeBall.gameObject.SetActive(true);
	}

	private void spawn(bool trash = false) {
		ballAmount++;
		if (trash) return;
		
		red = Random.Range(0, 2) == 0 ? true : false;
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
			if(!reachedPoint()) {
				hinge.localScale = new Vector3(hingeBase.x, hinge.localScale.y + 0.04f, hingeBase.z);
				fakeBall.localScale = new Vector3(Mathf.Lerp(fakeBall.localScale.x, fakeBallBase.x, Time.deltaTime * 8), Mathf.Lerp(fakeBall.localScale.y, fakeBallBase.y, Time.deltaTime * 8), Mathf.Lerp(fakeBall.localScale.z, fakeBallBase.z, Time.deltaTime * 8));
			}
			yield return new WaitForSeconds(.02f);
			if(reachedPoint()) {
				GameObject ball = Instantiate((dropClock)?clockPrefab : ballPrefab, transform.position + pos, transform.rotation);
				ball.GetComponent<ball>().red = red;
				if(dropClock) {
					ball.transform.rotation = Quaternion.Euler(90, 0, 0);
					ball.GetComponent<ball>().clock = true;
				}
				fakeBall.gameObject.SetActive(false);
				clock.gameObject.SetActive(false);
				audioManager.PLAY_SOUND("Plop", transform.position, 1, 1f);
				armExtended = true;
			}
		}
		StopCoroutine("extendArm");
		StopCoroutine("openDoors");
		resettimer = 2;
	}

	IEnumerator openDoors() {
		float t=0;
		doorL.localRotation = Quaternion.identity;
		doorR.localRotation = Quaternion.identity;
		while(!doorsOpen) {
			t += Time.deltaTime * 4;
			float open = Mathf.Lerp(doorL.localRotation.y, 180, t);
			doorL.localRotation= Quaternion.Euler(0, -open, 0);
			doorR.localRotation= Quaternion.Euler(0, open, 0);
			yield return null;
		if(open > 160) {
				StartCoroutine("extendArm");
				doorsOpen = true;
			}
		}
	}

	private bool reachedPoint() {
		return drop.position.z * ((flipped)? -1 : 1) <= pos.z * ((flipped)? -1 : 1);
	}

	private void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
