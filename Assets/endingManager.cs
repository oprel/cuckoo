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


    // Start is called before the first frame update
    private void Awake() {
        self = this;
    }


	public static void endGame(bool leftWins){
		//self.StartCoroutine(self.slowDown());
        self.winnerDisplay.gameObject.SetActive(true);
        if (leftWins){
            self.winnerDisplay.text = "OLD WINS";
        }else{
            self.winnerDisplay.text = "YOUNG WINS";
        }
	}
    

	private IEnumerator slowDown(){
		while (Time.timeScale>.1f){
			Time.timeScale = Mathf.Lerp(Time.timeScale,0,.1f);
		}
		Time.timeScale=1;
		yield return new WaitForSeconds(1f);
		StartCoroutine(EndCutscene());
	}

	private IEnumerator EndCutscene(){
		Camera.main.gameObject.SetActive(false);
		endingCutscene.gameObject.SetActive(true);
		yield return null;
	}

    private IEnumerator shameDoors(){
        for (int i = 0; i < 1; i+= 1/80)
        {
            endDoorLeft.transform.rotation = Quaternion.Euler(0,Mathf.SmoothStep(0,180,0),0);
            endDoorRight.transform.rotation = Quaternion.Euler(0,Mathf.SmoothStep(0,-180,0),0);
            yield return null;
        }
        hand.SetActive(true);
        yield return null;
    }
}
