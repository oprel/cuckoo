using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour {
	public float beakBoost = 300f;

	public static float spawnAnimSpeed = 5;
	private playerManager.Team team;
	public bool red = false;
	public bool trash = false;

	public float rotationSpeed = 1;
	private float rotBaseSpeed;
	
	void Start() {
		if(red) team = playerManager.self.GetTeam("RED"); 
		else {
			team = playerManager.self.GetTeam("BLUE");
			rotationSpeed *= -1;
		}
		rotBaseSpeed = rotationSpeed;
		playerManager.self.balls.Add(gameObject);
	}

	void FixedUpdate() {
		if (!trash)transform.Rotate(0, rotationSpeed, 0);

		if(transform.position.y < -10) {
			ballSpawner.decrementBalls();
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject == gameManager.self.goalLeft) {
			gameManager.addScoreLeft((trash)? -1 : 1);
			ballSpawner.decrementBalls();

			//speed UI
			gamestateVisuals.self.msgleft.speedChange(!trash);
			gamestateVisuals.hitStun();

			Destroy(gameObject);
		}
		if (other.gameObject == gameManager.self.goalRight) {
			gameManager.addScoreRight((trash)? -1 : 1);
			ballSpawner.decrementBalls();

			//speed UI
			gamestateVisuals.self.msgright.speedChange(!trash);
			gamestateVisuals.hitStun();
			Destroy(gameObject);
		}
	}

	//Boost on beak collision
	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Hitter") {
			GetComponent<Rigidbody>().AddForceAtPosition(col.transform.forward * beakBoost, col.transform.position);
			Instantiate(gamestateVisuals.self.beakboostvisual,transform.position, Quaternion.identity);
			gamestateVisuals.hitStun();
		}
	}

	public void resetRotation() {
		rotationSpeed = rotBaseSpeed;
	}

	public playerManager.Team GetTeam() {
		return team;
	}
}
