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

	public gamestateVisuals visuals;
	public Text scoreDisplay;

	private void Awake() {
		self = this;
		visuals = GetComponent<gamestateVisuals>();
	}
	
	void FixedUpdate() {
		scoreDisplay.text = scoreLeft.ToString() + " - " + scoreRight.ToString();
		if (Input.GetButton("Fire2")) {
			playerManager.self.stream.Dispose();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

	public static void addScoreLeft(int i) {
		self.scoreLeft += i;
		if(self.scoreLeft < 0) self.scoreLeft = 0;
		audioManager.PLAY_STATIONARY("Collect", 0.1f, 0.5f);
	}

	public static void addScoreRight(int i) {
		self.scoreRight += i;
		if(self.scoreRight < 0) self.scoreRight = 0;
		audioManager.PLAY_STATIONARY("Collect", 0.1f, 0.5f);
	}
}
