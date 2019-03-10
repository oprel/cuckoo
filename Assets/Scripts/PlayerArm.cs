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

	private GameObject birdPresenter;
	private float basePresenterScale;
	private static bool animatePresenter = false;
	private float baseZ;

	void Start () {
		self = this;
		birdPresenter = GameObject.FindGameObjectWithTag("BirdPresenter");
		basePresenterScale = birdPresenter.transform.localScale.z;
		house = transform.GetChild(0);
		hinge = house.Find("Hinge");
		drop = hinge.Find("DropPoint");
		baseZ = hinge.localPosition.z;
		hingeBase = new Vector3(hinge.localScale.x, baseScale, hinge.localScale.z);
		hinge.localScale = hingeBase;
		house.gameObject.SetActive(false);
        hinge.transform.localScale = new Vector3(hinge.localScale.x, baseScale, hinge.localScale.z);
	}

	void FixedUpdate () {
		if(resettimer <= 0) timer += Time.deltaTime;

		if(target != null) target.transform.localPosition = new Vector3(0, 0, 0);

		if(armExtended) animatePresenter = true;

		if(animatePresenter) {
			float tar = Mathf.Sin(Time.time / 1.2f) + 1;
			if(tar > 1.5f) tar = Mathf.Lerp(tar, 1.8f, Time.deltaTime * 2);
			birdPresenter.transform.localScale = new Vector3(1, 1, Mathf.Lerp(birdPresenter.transform.localScale.z, tar, Time.deltaTime * 2));
			birdPresenter.transform.localPosition = new Vector3(birdPresenter.transform.localPosition.x, 0.83f, birdPresenter.transform.localPosition.z);
			hinge.localPosition = new Vector3(hinge.localPosition.x, hinge.localPosition.y, Mathf.Cos(Time.time) / 10 + baseZ);
		}

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
        this.target.transform.position = Vector3.zero;
        this.target.GetComponent<Rigidbody>().useGravity = false;
        this.target.GetComponent<Rigidbody>().isKinematic = true;
        this.target.GetComponent<player>().cutscene = true;
        this.target.transform.localScale = new Vector3(1, 1, 1);
		this.target.transform.localPosition = new Vector3(0, 0, 0);
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
