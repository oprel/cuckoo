using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trashManager : MonoBehaviour {
    void FixedUpdate() {
        trashSpawner.trashAmount = CountTrash();
    }

    private int CountTrash() {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Trash");
		int num = 0;
		foreach(GameObject ball in balls) if(ball.GetComponent<Rigidbody>() != null && !ball.GetComponent<Rigidbody>().isKinematic) num++;
		return num;
    }
}
