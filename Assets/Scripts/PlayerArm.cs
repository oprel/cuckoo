using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour {
	public static PlayerArm self;

    public GameObject target;
	public float baseScale = 0.2f, moveDelay = 0.2f;
	private float rand;
	private float timer, resettimer = 0;

	private Transform house, hinge, drop;
	private Vector3 hingeBase;

	private bool armExtended = false;
	private Vector3 pos;

	void Start () {
		self = this;
		house = transform.GetChild(0);
		hinge = house.Find("Hinge");
		drop = hinge.Find("DropPoint");
		hingeBase = new Vector3(hinge.localScale.x, baseScale, hinge.localScale.z);
		hinge.localScale = hingeBase;
		house.gameObject.SetActive(false);
        hinge.transform.localScale = new Vector3(hinge.localScale.x, baseScale, hinge.localScale.z);
	}

	void FixedUpdate () {
		if(resettimer <= 0) timer += Time.deltaTime;

		if(resettimer > 0) {
			resettimer -= Time.deltaTime;
			hinge.localScale = new Vector3(hingeBase.x, Mathf.Lerp(hinge.localScale.y, hingeBase.y, Time.deltaTime * 3), hingeBase.z);
		}
	}

	private void reset() {
		timer = 0;
		rand = Random.Range(0f, 1.5f);
		armExtended = false;
		house.gameObject.SetActive(false);
	}

	public void Activate(GameObject target) {
        this.target = target;
        this.target.transform.SetParent(drop);
        this.target.transform.localPosition = Vector3.zero;
		house.gameObject.SetActive(true);
		StartCoroutine("extendArm");
	}

	IEnumerator extendArm() {
		while(!armExtended) {
			if(!reachedPoint()) hinge.localScale = new Vector3(hingeBase.x, hinge.localScale.y + 0.02f, hingeBase.z);
			yield return new WaitForSeconds(moveDelay / 100);
			if(reachedPoint()) armExtended = true;
		}
		StopCoroutine("extendArm");
	}

	private bool reachedPoint() {
		return hinge.localScale.y >= 2f;
	}
}
