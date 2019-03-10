using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class endingManager : MonoBehaviour {
    public static endingManager self;
    public Cutscene endingCutscene;
    public GameObject endDoorLeft, endDoorRight;
    public TextMeshProUGUI winnerDisplay;
    private List<Transform> winners = new List<Transform>();
    private List<Transform> losers = new List<Transform>();
    private static bool leftWins;
    public PlayerArm[] arms;

    private void Awake() {
        self = this;
        endingCutscene.gameObject.SetActive(false);
    }

    public void presentWinner() {
        playerManager.self.SetIgnoreControls(true);
		for(int i = 0; i < arms.Length; i++) arms[i].Activate(winners[i].gameObject);
    }

    public void displayWinner() {
        winnerDisplay.gameObject.SetActive(true);
        if (leftWins) winnerDisplay.text = "BLUE WINS";
        else winnerDisplay.text = "RED WINS";
    }

	public static void endGame(bool didLeftWin) {
        leftWins = didLeftWin;
        for (int i = 0; i < playerManager.leftPlayers.Count; i++) {
            Transform t = playerManager.leftPlayers[i].transform;
            if (leftWins) self.winners.Add(t);
            else self.losers.Add(t);
        }
        for (int i = 0; i < playerManager.rightPlayers.Count; i++) {
            Transform t = playerManager.rightPlayers[i].transform;
            if (!leftWins) self.winners.Add(t);
            else self.losers.Add(t);
        }
        self.StartCoroutine(self.slowDown());
    }
    
	private IEnumerator slowDown() {
        for (float i = 0f; i < 1f; i+= 1f / 80f) {
            Time.timeScale = Mathf.SmoothStep(1, 0, i);
            yield return null;
        }
		Time.timeScale = 1;
		EndCutscene();
	}

	private void EndCutscene() {
        for (int i = 0; i < endingCutscene.shots.Length; i++) endingCutscene.shots[i].target = winners[1].gameObject;
        playerManager.self.enabled = false;
		Camera.main.gameObject.SetActive(false);
		endingCutscene.gameObject.SetActive(true);
        endingCutscene.GetComponent<Camera>().enabled = true;
	}

    public void outside() {
        StartCoroutine(shameDoors());
    }

    private IEnumerator shameDoors() {
        for (float i = 0f; i < 1f; i+= 1f / 80f) {
            endDoorLeft.transform.rotation = Quaternion.Euler(0, Mathf.SmoothStep(-180, 0, i), 0);
            endDoorRight.transform.rotation = Quaternion.Euler(0, Mathf.SmoothStep(0, -180, i), 0);
            yield return null;
        }

        StartCoroutine(celebratePlayers());
        while (true) {
            StartCoroutine(shamePlayers());
            yield return new WaitForSeconds(600);
            gameManager.self.ResetGame();
        }
    }

    public float radius;
    private IEnumerator celebratePlayers() {
         foreach (Transform t in winners) {
             t.Find("trail").gameObject.SetActive(false);
            t.rotation = Quaternion.Euler(new Vector3(Random.value * 360, 90, -90));
            Vector3 pos = Random.insideUnitSphere * radius;
            pos.z = 0;
            for (float j = 0f; j < 1f; j += 1f / 20f) {
                t.position = Vector3.Lerp(t.position, transform.position + pos, j);
                yield return null;
            }            
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,radius);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(loserLocation,radius);
    }

    public Vector3 loserLocation;
    private IEnumerator shamePlayers() {
        foreach (Transform t in losers) {
            t.Find("trail").gameObject.SetActive(false);
            t.rotation = Quaternion.Euler(new Vector3(Random.value * 360, 90, -90));
            Vector3 pos = new Vector3(Random.value * radius, 0, 0);
            for (float j = 0f; j < 1f; j+= 1f / 80f) {
                t.position = Vector3.Lerp(t.position, loserLocation + pos, j);
                yield return null;
            } 
        }   
    }
}