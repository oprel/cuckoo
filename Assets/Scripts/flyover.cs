using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyover : MonoBehaviour {

	public float radius;
	public float speed;
	public Vector3 viewTarget;

	void Update () {
		Vector3 pos = transform.position;
		pos.x = Mathf.Sin(Time.time*speed)*radius;
		pos.z = Mathf.Cos(Time.time*speed + Mathf.PI)*radius;
		transform.position = pos;
		transform.LookAt(viewTarget);

	}

	void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position,radius);
	}
}
