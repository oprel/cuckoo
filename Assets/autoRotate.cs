using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoRotate : MonoBehaviour {

	public float speed;
	// Use this for initialization
	void Start () {
		if (speed<0) speed = Random.value * 10;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.forward, speed * Time.deltaTime);
	}
}
