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

	private void Awake(){
		self = this;
		visuals = GetComponent<gamestateVisuals>();
	}
	
	// Update is called once per frame
	void Update () {
		if (scoreLeft==0){
			visuals.msgleft.speedChange(true);
			scoreLeft++;
		}
		scoreDisplay.text= scoreLeft.ToString() + " - " + scoreRight.ToString();
	}
}
