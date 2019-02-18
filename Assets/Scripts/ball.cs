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
	public int breakState = -1;
	private int oldBreakState = -1;
	private string[] breakSounds = {"CrankShort", "CrankMid", "CrankLong"};
	private float dmgDelay = 0;

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
		if(breakState > breakTextures.Length) breakState = breakTextures.Length;
		if(breakState != oldBreakState && trash) updateBreakState();

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
		audioManager.PLAY_SOUND(breakSounds[Mathf.Clamp(breakState, 0, breakTextures.Length - 1)], transform.position, 40);
		if(breakState >= breakTextures.Length) {
			destroy(col);
			return;
		}

		updateBreakState();
	}

	private void updateBreakState() {
		oldBreakState = breakState;
		MeshRenderer[] m = GetComponentsInChildren<MeshRenderer>();
		if(m.Length > 0) {
			if(breakState >= 0 && breakTextures[breakState] != null)  foreach(MeshRenderer mesh in m) for(int i = 0; i < mesh.materials.Length; i++) mesh.materials[i].mainTexture = breakTextures[breakState];
			else foreach(MeshRenderer mesh in m) for(int i = 0; i < mesh.materials.Length; i++) mesh.materials[i].mainTexture = null;
		}
	}

	protected void destroy(Collision col) {
		MeshCollider[] me = GetComponentsInChildren<MeshCollider>();
		foreach(MeshCollider m in me) {
			Physics.IgnoreCollision(m, col.collider);
			m.transform.SetParent(null);
			m.gameObject.AddComponent<Rigidbody>();
			MeshRenderer mR = m.GetComponent<MeshRenderer>();
			foreach(Material mat in mR.materials) {
				mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				mat.SetInt("_ZWrite", 0);
				mat.DisableKeyword("_ALPHATEST_ON");
				mat.EnableKeyword("_ALPHABLEND_ON");
				mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				mat.renderQueue = 3000;
				mat.color = new Color(mat.color.r * 0.8f - 0.25f, mat.color.g * 0.8f - 0.25f, mat.color.b * 0.8f - 0.25f);
			}
			m.gameObject.AddComponent<Fader>();
		}
	}

	public void resetRotation() {
		rotationSpeed = rotBaseSpeed;
	}

	public playerManager.Team GetTeam() {
		return team;
	}
}
