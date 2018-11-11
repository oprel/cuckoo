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

	public Text scoreDisplay;

	private void Awake(){
		self = this;
	}
	
	// Update is called once per frame
	void Update () {
		scoreDisplay.text= scoreLeft.ToString() + " - " + scoreRight.ToString();
	}
}
