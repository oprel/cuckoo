using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour {
	public static gameManager self;
	public static int scoreLeft;
	public static int scoreRight;
	public GameObject goalLeft;
	public GameObject goalRight;

	private gamestateVisuals visuals;
	public Text scoreDisplay;

	private void Awake() {
		self = this;
		visuals = GetComponent<gamestateVisuals>();
	}
	
	void FixedUpdate() {
		scoreDisplay.text = scoreLeft.ToString() + " - " + scoreRight.ToString();
	}

	public static void addScoreLeft(int i) {
		scoreLeft += i;
		if(scoreLeft < 0) scoreLeft = 0;
	}

	public static void addScoreRight(int i) {
		scoreRight += i;
		if(scoreRight < 0) scoreRight = 0;
	}
}
