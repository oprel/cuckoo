using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour {
	public static gameManager self;
	public int scoreLeft;
	public int scoreRight;
	public GameObject goalLeft;
	public GameObject goalRight;
	public GameObject ballSpawner;

	public gamestateVisuals visuals;
	public Text scoreDisplay;
	public trashSpawner[] planks;

	private cameraShake camShaker;

	private void Awake() {
		self = this;
		visuals = GetComponent<gamestateVisuals>();
		camShaker = Camera.main.GetComponent<cameraShake>();
	}
	
	void FixedUpdate() {
		scoreDisplay.text = scoreLeft.ToString() + " - " + scoreRight.ToString();
		if (Input.GetButton("Fire2")) {
			if(playerManager.self.stream != null) playerManager.self.stream.Dispose();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

	public static void addScoreLeft(int i) {
		self.scoreLeft += i;
		if(self.scoreLeft < 0) self.scoreLeft = 0;
		audioManager.PLAY_STATIONARY("Collect", 0.1f, 0.5f);
		self.camShaker.ShakeCamera(0.15f, 0.1f);
		foreach (trashSpawner p in self.planks) p.scored();
	}

	public static void addScoreRight(int i) {
		self.scoreRight += i;
		if(self.scoreRight < 0) self.scoreRight = 0;
		audioManager.PLAY_STATIONARY("Collect", 0.1f, 0.5f);
		self.camShaker.ShakeCamera(0.15f, 0.1f);
		foreach (trashSpawner p in self.planks) p.scored();
	}
}
