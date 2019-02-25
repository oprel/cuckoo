using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class endingManager : MonoBehaviour
{

    public static endingManager self;
    public Cutscene endingCutscene;
    public GameObject endDoorLeft, endDoorRight, hand;
    public Text winnerDisplay;
    private static GameObject playerWinner;
    private static GameObject playerLoser;
    private static bool leftWins;

    private static List<Cutscene.Shot> focusShots = new List<Cutscene.Shot>();


    // Start is called before the first frame update
    private void Awake() {
        self = this;
        endingCutscene.gameObject.SetActive(false);
    }
    public void displayWinner(){
        winnerDisplay.gameObject.SetActive(true);
        if (leftWins){
            winnerDisplay.text = "OLD WINS";
        }else{
            winnerDisplay.text = "YOUNG WINS";
        }
    }

	public static void endGame(bool didLeftWin){
        leftWins = didLeftWin;
		if (leftWins){
            playerWinner = playerManager.leftPlayers[1].gameObject;
            playerLoser = playerManager.rightPlayers[1].gameObject;
        }else{
            playerLoser = playerManager.leftPlayers[1].gameObject;
            playerWinner = playerManager.rightPlayers[1].gameObject;
        }
        
        self.StartCoroutine(self.slowDown());

      
	}
    

	private IEnumerator slowDown(){
        /*
		while (Time.timeScale>.1f){
			Time.timeScale = Mathf.Lerp(Time.timeScale,0,.01f);
            yield return null;
		}
		Time.timeScale=1;*/
		yield return new WaitForSeconds(2f);

        
		self.StartCoroutine(EndCutscene());
	}

	private IEnumerator EndCutscene(){
        focusShots.Clear();
        /*foreach (Cutscene.Shot shot in endingCutscene.shots){
            if (shot.focusOnTarget) focusShots.Add(shot);
        }*/
        for (int i = 0; i < endingCutscene.shots.Length; i++)
        {
            endingCutscene.shots[i].target = playerWinner;
        }
        playerManager.self.enabled=false;
		Camera.main.gameObject.SetActive(false);
		endingCutscene.gameObject.SetActive(true);
        endingCutscene.GetComponent<Camera>().enabled = true;
        Debug.Log("ending START");
		yield return null;
        self.StartCoroutine(shameDoors());
        
	}

    private IEnumerator shameDoors(){
        for (float i = 0f; i < 1f; i+= 1f/80f)
        {
            endDoorLeft.transform.rotation = Quaternion.Euler(0,Mathf.SmoothStep(0,180,i),0);
            endDoorRight.transform.rotation = Quaternion.Euler(0,Mathf.SmoothStep(0,-180,i),0);
            yield return null;
        }

        StartCoroutine(celebratePlayers());
        while (true){
            StartCoroutine(shamePlayers());
            yield return new WaitForSeconds(3);
        }
       

    }
    public float radius;
    private IEnumerator shamePlayers(){
         for (int i = 0; i < playerManager.leftPlayers.Count; i++)
        {
            Transform t = playerManager.leftPlayers[i].transform;
             t.Find("trail").gameObject.SetActive(false);
            t.rotation = Quaternion.Euler(new Vector3(Random.value*360,90,-90));
            Vector3 pos = Random.insideUnitSphere * radius;
            pos.z=0;
            for (float j = 0f; j < 1f; j+= 1f/20f){
                t.position = Vector3.Lerp(t.position,transform.position+pos,j);
                yield return null;
            }            

        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,radius);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(winnerLocation,radius);
    }

    public Vector3 winnerLocation;
    private IEnumerator celebratePlayers(){
        for (int i = 0; i < playerManager.rightPlayers.Count; i++)
        {
            Transform t = playerManager.rightPlayers[i].transform;
             t.Find("trail").gameObject.SetActive(false);
            t.rotation = Quaternion.Euler(new Vector3(Random.value*360,90,-90));
            Vector3 pos = new Vector3(Random.value*radius,0,0);
            for (float j = 0f; j < 1f; j+= 1f/80f){
                t.position = Vector3.Lerp(t.position,winnerLocation+pos,j);
                yield return null;
            } 
        }   
    }
}
