using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour {
	public static gameManager self;
	public float gameTime;
	public int scoreLeft;
	public int scoreRight;
	public GameObject goalLeft;
	public GameObject goalRight;
	public GameObject ballSpawner;
	public GameObject gears;
	public Text oldPoints, youngPoints;

	public gamestateVisuals visuals;
	public Text timeDisplay;
	public trashSpawner[] planks;

	private cameraShake camShaker;
	private static float gameTimer;
	private bool gamePaused;

	private void Awake() {
		self = this;
		visuals = GetComponent<gamestateVisuals>();
		camShaker = Camera.main.GetComponent<cameraShake>();
		gameTimer = self.gameTime;

	}

	public void DisableGameSounds() {
		autoRotate[] sounds = gears.transform.GetComponentsInChildren<autoRotate>();
		foreach(var i in sounds) i.StopAudio();
	}
	
	void FixedUpdate() {
		gameTimer-=Time.deltaTime;
		timeDisplay.text = "time: " + (int)gameTimer + "/" + gameTime;
		if (Input.GetButton("Fire2")) {
			if(playerManager.self.stream != null) playerManager.self.stream.Dispose();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		if (gameTimer <= 0 && scoreLeft != scoreRight) {
			endingManager.endGame(scoreLeft >scoreRight);
			gameTimer = Mathf.Infinity;
		} 
	}

	public void SetFinalScore() {
		oldPoints.text = "Old: " + scoreLeft;
		youngPoints.text = "Young: " + scoreRight;
	}

	public static void addScoreLeft(int i) {
		self.scoreLeft += i;
		if(self.scoreLeft < 0) self.scoreLeft = 0;
		audioManager.PLAY_STATIONARY("Collect", 0.1f, Random.Range(0.4f, 0.6f));
		self.camShaker.ShakeCamera(0.15f, 0.1f);
		foreach (trashSpawner p in self.planks) if(Random.Range(0, 10) < 3) p.scored();
		gamestateVisuals.scoreFeedback(true,i);
		
	}

	public static void addScoreRight(int i) {
		self.scoreRight += i;
		if(self.scoreRight < 0) self.scoreRight = 0;
		audioManager.PLAY_STATIONARY("Collect", 0.1f, Random.Range(0.4f, 0.6f));
		self.camShaker.ShakeCamera(0.15f, 0.1f);
		foreach (trashSpawner p in self.planks) if(Random.Range(0, 10) < 3) p.scored();
		gamestateVisuals.scoreFeedback(false,i);
	}
}
