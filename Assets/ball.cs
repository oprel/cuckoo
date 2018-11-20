using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour {
	public float beakBoost = 300f;

	public static float spawnAnimSpeed = 5;
	private playerManager.Team team;
	public bool red = false;
	
	void Start() {
		if(red) team = playerManager.self.GetTeam("RED"); 
		else team = playerManager.self.GetTeam("BLUE");
		GetComponentInChildren<SpriteRenderer>().sprite = team.ballTexture;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject == gameManager.self.goalLeft) {
			 gameManager.scoreRight++;
			 Destroy(gameObject);
		}
		if (other.gameObject == gameManager.self.goalRight) {
			 gameManager.scoreLeft++;
			 Destroy(gameObject);
		}
	}

	//Boost on beak collision
	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Hitter") GetComponent<Rigidbody>().AddForceAtPosition(col.transform.forward * beakBoost, col.transform.position);
	}
}
