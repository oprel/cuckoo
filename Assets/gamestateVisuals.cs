using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamestateVisuals : MonoBehaviour {

	public autoRotate gearLeft;
	public autoRotate gearRight;
	public speedChangeDisplay msgleft;
	public speedChangeDisplay msgright;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gearLeft.speed = gameManager.scoreLeft * -10;
		gearRight.speed = gameManager.scoreRight * 10;
	}
}
