using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour {
	public float beakBoost = 300f;

	public static float spawnAnimSpeed = 5;
	private playerManager.Team team;
	public bool red = false;

	public float rotationSpeed = 1;
	private float rotBaseSpeed;
	
	void Start() {
		if(red) team = playerManager.self.GetTeam("RED"); 
		else {
			team = playerManager.self.GetTeam("BLUE");
			rotationSpeed *= -1;
		}
		//GetComponentInChildren<SpriteRenderer>().sprite = team.ballTexture;
		rotBaseSpeed = rotationSpeed;
		playerManager.self.balls.Add(gameObject);
	}

	void FixedUpdate() {
		transform.Rotate(0, rotationSpeed, 0);

		if(transform.position.y < -10) {
			ballSpawner.decrementBalls();
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject == gameManager.self.goalLeft) {
			gameManager.addScoreLeft((red)? 1 : -1);
			ballSpawner.decrementBalls();

			//speed UI
			if(red) gamestateVisuals.self.msgleft.speedChange(true);
			else gamestateVisuals.self.msgleft.speedChange(false);
			gamestateVisuals.hitStun();

			Destroy(gameObject);
		}
		if (other.gameObject == gameManager.self.goalRight) {
			gameManager.addScoreRight((red)? -1 : 1);
			ballSpawner.decrementBalls();

			//speed UI
			if(!red) gamestateVisuals.self.msgright.speedChange(true);
			else gamestateVisuals.self.msgright.speedChange(false);
			gamestateVisuals.hitStun();
			Destroy(gameObject);
		}
	}

	//Boost on beak collision
	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Hitter") {
			GetComponent<Rigidbody>().AddForceAtPosition(col.transform.forward * beakBoost, col.transform.position);
			gamestateVisuals.hitStun();
		}
	}

	public void resetRotation() {
		rotationSpeed = rotBaseSpeed;
	}
}
