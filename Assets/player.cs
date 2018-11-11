using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour {

	public bool leftTeam;
	private Rigidbody rb;
	private Vector3 startPos;
	
	private void Awake(){
		playerManager.addPlayer(leftTeam,this);
		rb = GetComponent<Rigidbody>();
		startPos = transform.position;
	}

	public void Update(){
		if (transform.position.y<-2) Reset();
	}
	public void impulse(float force){
		rb.AddForce(transform.forward * force);
	}

public void Reset(){
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.position = startPos;
	}
}
