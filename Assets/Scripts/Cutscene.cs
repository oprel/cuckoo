using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour {
    public bool playCutscene = true;

    public UnityEngine.Events.UnityEvent startEvent;

    public GameObject[] disableOnStartObjects;
    public GameObject countdown;
    private Text[] countdownText;
    private float countdownPos;

    [System.Serializable]
    public class Shot {
        public string name;
        [Range(0, 10)]
        public float duration;
        public Vector3 pos, rotation;
        public GameObject target;
        public bool focusOnTarget = false;
        public UnityEngine.Events.UnityEvent postEvent;
    }

    [Space(10)]
    [SerializeField]
    public Shot gameShot;
    public Shot[] shots;
    [Space(10)]
    [Range(0, 15)]
    public int currentShot = 0;
    private Shot current;

    private Camera mainCam;
    private float time = 0;

    public void Start() {
        mainCam = Camera.main;
        countdownText =  countdown.GetComponentsInChildren<Text>();
        countdownPos = countdown.transform.position.y;

        if(!playCutscene || currentShot < 0) forceApplyShot(gameShot);
        else forceApplyShot(shots[currentShot]);

        if(playCutscene) {
            foreach(GameObject g in disableOnStartObjects) g.SetActive(false);
        } else {
            currentShot = -1;
            EndCutscene();
        }
        OnStart();
    }

    protected void OnStart() {
        startEvent.Invoke();
    }

    void FixedUpdate() {
        time += Time.deltaTime;

        if(playCutscene) {
            if(currentShot < 0) current = gameShot;
            else {
                current = shots[currentShot];
                smoothApplyShot(current);
            }
        } else smoothApplyShot(gameShot);

        if(currentShot != -1 && current != null) {
            if(time > shots[currentShot].duration) {
                current.postEvent.Invoke();
                if(currentShot < shots.Length - 1) currentShot++;
                else {
                    current = gameShot;
                    playCutscene = false;
                }
                time = 0;
            }
        }
    }

    public void smoothApplyShot(Shot shot) {
        float speed = 2;

        if(shot.focusOnTarget) {
            Vector3 tar = shot.target.transform.position;
             float xx = Mathf.Lerp(mainCam.transform.position.x, tar.x, Time.deltaTime * speed), 
                  yy = Mathf.Lerp(mainCam.transform.position.y, tar.y + shot.pos.y, Time.deltaTime * speed),
                  zz = Mathf.Lerp(mainCam.transform.position.z, tar.z, Time.deltaTime * speed);
            mainCam.transform.position = new Vector3(xx, yy, zz);
            
            float rrX = Mathf.LerpAngle(mainCam.transform.localEulerAngles.x, shot.rotation.x, Time.deltaTime * speed), 
                rrY = Mathf.LerpAngle(mainCam.transform.localEulerAngles.y, shot.rotation.y, Time.deltaTime * speed),
                rrZ = Mathf.LerpAngle(mainCam.transform.localEulerAngles.z, shot.rotation.z, Time.deltaTime * speed);
            mainCam.transform.rotation = Quaternion.Euler(rrX, rrY, rrZ);
        } else {
            float x = Mathf.Lerp(mainCam.transform.position.x, shot.pos.x, Time.deltaTime * speed), 
                    y = Mathf.Lerp(mainCam.transform.position.y, shot.pos.y, Time.deltaTime * speed),
                    z = Mathf.Lerp(mainCam.transform.position.z, shot.pos.z, Time.deltaTime * speed);
            mainCam.transform.position = new Vector3(x /*+ Mathf.Sin(Time.time) / 100*/, y, z);

            float rX = Mathf.LerpAngle(mainCam.transform.localEulerAngles.x, shot.rotation.x, Time.deltaTime * speed), 
                    rY = Mathf.LerpAngle(mainCam.transform.localEulerAngles.y, shot.rotation.y, Time.deltaTime * speed),
                    rZ = Mathf.LerpAngle(mainCam.transform.localEulerAngles.z, shot.rotation.z, Time.deltaTime * speed);
            mainCam.transform.rotation = Quaternion.Euler(rX, rY, rZ);
        }
    }

    public void forceApplyShot(Shot shot) {
        mainCam.transform.position = shot.pos;
        mainCam.transform.rotation = Quaternion.Euler(shot.rotation.x, shot.rotation.y, shot.rotation.z);
    }

    public void EnableStartObjects() {
        foreach(GameObject g in disableOnStartObjects) g.SetActive(true);
    }

    public void QueueCountdownTXT() {
        countdown.SetActive(true);
        StartCoroutine("StartCountdown");
    }

    private void EndCutscene() {
        countdown.SetActive(false);
        playerManager.self.Ready();
        playCutscene = false;
        StopCoroutine("StartCountdown");
    }

    private float count = 4;
    private int prevCount, countInt;
    IEnumerator StartCountdown() {
            while(count > 0) {
            count -= Time.deltaTime * 2;
            string finString = ((int)count).ToString();
            if(prevCount != count && countInt != prevCount) {
                audioManager.PLAY_CAM("Hit", 0.05f, 0.7f);
                if(count < 1) audioManager.PLAY_CAM("Tick2", 0.2f, 0.9f);
                countInt = prevCount;
            }
            if(count < 1) finString = "FIGHT!";
            else countdown.transform.position = new Vector3(countdown.transform.position.x, countdownPos + Mathf.Sin(Time.time * 10) * 1.2f, countdown.transform.position.z);
            foreach(Text t in countdownText) t.text = finString;
            prevCount = (int)count;
            yield return new WaitForSeconds(.02f);
        }
        EndCutscene();
    }

    private void OnDrawGizmosSelected() {
        for (int i = 0; i < shots.Length-1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(shots[i].pos,shots[i+1].pos);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(shots[i].pos, shots[i].pos+ Quaternion.Euler(shots[i].rotation) * Vector3.forward);
            Gizmos.DrawLine(shots[i+1].pos, shots[i+1].pos+ Quaternion.Euler(shots[i+1].rotation) * Vector3.forward); //redundant but easy
        }
    }
}
