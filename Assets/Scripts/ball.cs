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

	[Header("Break states / Cracks")]
	public Texture2D[] breakTextures;
	public int breakState = 0;
	private string[] breakSounds = {"CrankShort", "CrankMid", "CrankLong"};
	private float dmgDelay = 0;

	private MeshRenderer mesh;
	private Rigidbody rb;
	
	void Start() {
		if(red) team = playerManager.self.GetTeam("RED"); 
		else {
			team = playerManager.self.GetTeam("BLUE");
			rotationSpeed *= -1;
		}
		rotBaseSpeed = rotationSpeed;
		playerManager.self.balls.Add(gameObject);
		mesh = GetComponent<MeshRenderer>();
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate() {
		if (!trash) transform.Rotate(0, rotationSpeed, 0);

		if(transform.position.y < -10) {
			ballSpawner.decrementBalls();
			Destroy(gameObject);
		}

		//Break states
		if(trash) {
			if(dmgDelay > 0) dmgDelay -= Time.deltaTime;
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
			Instantiate(gamestateVisuals.self.beakboostvisual, transform.position, Quaternion.identity);
			gamestateVisuals.hitStun();
			if(trash && dmgDelay <= 0) damage(col);
		}
		else if((col.gameObject.tag == "Player" || col.gameObject.tag == "Hitter") && !trash) audioManager.PLAY_SOUND("Nudge", transform.position, 140f, Random.Range(0.8f, 1.2f));
	}

	protected void damage(Collision col) {
		dmgDelay = 2;
		breakState++;
		if(breakState > breakTextures.Length - 1) breakState = breakTextures.Length - 1;
		audioManager.PLAY_SOUND(breakSounds[Mathf.Clamp(breakState, 0, breakTextures.Length)], transform.position, 40);
		if(breakState >= breakTextures.Length - 1) {
			destroy(col);
			return;
		}
		for(int i = 0; i < mesh.materials.Length; i++) mesh.materials[i].mainTexture = breakTextures[breakState];
	}

	protected void destroy(Collision col) {
		if(col.gameObject.tag == "") Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), col.collider);
	}

	public void resetRotation() {
		rotationSpeed = rotBaseSpeed;
	}

	public playerManager.Team GetTeam() {
		return team;
	}
}
