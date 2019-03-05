using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class gameManager : MonoBehaviour {
	public static gameManager self;
	public float gameTime;
	
	public int scoreLeft;
	public int scoreRight;
	public GameObject goalLeft;
	public GameObject goalRight;
	public GameObject ballSpawner;
	public GameObject gears;
	public TextMeshProUGUI oldPoints, youngPoints, timeDisplay;

	public gamestateVisuals visuals;
	public trashSpawner[] planks;

	private cameraShake camShaker;
	private static float gameTimer;
	private bool gamePaused;
	private bool ended = false;

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
		if(Input.GetKeyDown(KeyCode.P)) {
			scoreRight += 20;
			gameTimer = gameTime = 1;
			gamestateVisuals.displayScore();
		}

		gameTimer -= Time.deltaTime;
		timeDisplay.text = "time: " + (int)gameTimer + "/" + gameTime;
		if(gameTimer < 0) timeDisplay.gameObject.SetActive(false);
		if (Input.GetButton("Fire2")) ResetGame();
		if (gameTimer <= 0 && scoreLeft != scoreRight && !ended) {
			endingManager.endGame(scoreLeft > scoreRight);
			ended = true;
		} 
	}

	public static int GetCurrentGameTime() {
		return (int)gameTimer;
	}

	public void ResetGame(){
		if(playerManager.self.getStream() != null) playerManager.self.getStream().Dispose();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	public void SetFinalScore() {
		oldPoints.text = "blue: " + scoreLeft.ToString().Replace("0","O");
		youngPoints.text = "red: " + scoreRight.ToString().Replace("0","O");
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
