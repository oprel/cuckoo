using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerManager : MonoBehaviour {

	public static List<player> leftPlayers = new List<player>();
	public static List<player> rightPlayers = new List<player>();
	public static playerManager self;
	
	public float frequency;
	public float speed;
	public input[] leftInput;
	public input[] rightInput;

	[System.Serializable]
	public class input{
		public string name;
		[Range(0,10)]
		public float energy;
		[Range(0,360)]
		public float direction;
		[HideInInspector]
		public float timer;
	};

	public static void addPlayer(bool addToLeft, player p){
		if (addToLeft){
			leftPlayers.Add(p);
		}else{
			rightPlayers.Add(p);
		}
	}
	// Use this for initialization
	void Awake () {
		self = this;
	}
	void Start(){
		foreach (input i in leftInput){
			i.direction = 90;
		}
		foreach (input i in rightInput){
			i.direction = 270;
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < leftInput.Length; i++){
			player p = leftPlayers[i];
			input inp = leftInput[i];
			if (p) updatePlayer(p,inp);
		}
		for (int i = 0; i < rightInput.Length; i++){
			player p = rightPlayers[i];
			input inp = rightInput[i];
			if (p) updatePlayer(p,inp);
		}
	}

	void updatePlayer(player p, input inp){
		p.transform.rotation =  Quaternion.Euler(0,inp.direction,0);
		

		if (inp.energy>0){
			inp.timer+=Time.deltaTime;
			if (inp.timer>frequency){
				p.impulse(speed);
				inp.timer=0;
				inp.energy--;
			}
		}
	}
}
