using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour {

	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject == gameManager.self.goalLeft){
			 gameManager.scoreRight++;
			 Destroy(gameObject);
		}
		if (other.gameObject == gameManager.self.goalRight){
			 gameManager.scoreLeft++;
			 Destroy(gameObject);
		}
	}
	
}
