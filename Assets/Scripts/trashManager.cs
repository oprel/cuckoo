using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trashManager : MonoBehaviour {
    public int maxTrash = 4;
    public static int trashAmount;
    public static trashManager self;
    public float energyUntilHit = 2;
    public int hitsUntilDrop = 1;

    void Awake() {
        self = this;
    }

    public static int CountTrash() {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Trash");
		int num = 0;
		foreach(GameObject ball in balls) if(ball.GetComponent<Rigidbody>() != null && !ball.GetComponent<Rigidbody>().isKinematic) num++;
		trashAmount = num;
        return num;
    }
}
