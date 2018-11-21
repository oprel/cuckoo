using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamestateVisuals : MonoBehaviour {
	public autoRotate gearLeft;
	public autoRotate gearRight;
	public speedChangeDisplay msgleft;
	public speedChangeDisplay msgright;

	void Update () {
		gearLeft.speed = gameManager.self.scoreLeft * -10;
		gearRight.speed = gameManager.self.scoreRight * 10;
	}
}
