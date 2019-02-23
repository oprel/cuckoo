using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;

public class playerManager : MonoBehaviour {
	public static Dictionary<int, player> leftPlayers = new Dictionary<int, player>();
	public static Dictionary<int, player> rightPlayers = new Dictionary<int, player>();
	public static playerManager self;

	[HideInInspector]
	public List<GameObject> balls = new List<GameObject>();
	
	public GameObject textPrefab, oilPrefab;

	private int playerCount;
	private bool cutscene = false;

	public enum Port {
		COM1,
		COM2,
		COM3,
		COM4,
		COM5,
		COM6,
		COM7,
		COM8,
		COM9,
		COM10,
		COM11
	}
	public Port port;

	[System.Serializable]
	public class Team {
		public string name;
		public Sprite texture, ballTexture;
	}

	[SerializeField]
	public Team[] teams;
	public Team GetTeam(string color) {
		foreach(Team t in teams) if(t.name.ToLower() == color.ToLower()) return t;
		return null;
	}

	[Space(5)]
	[Header("Settings")]
	public bool DebugMode;
	public float frequency;
	public float tickSpeed, chargeSpeed;
	public float beakBoostOnPlayers = 300f;
	[Space(5)]
	[Header("Inputs")]
	public Input[] leftInput;
	public Input[] rightInput;

	public SerialPort stream;

	private Dictionary<int, float> rotations = new Dictionary<int, float>();
	private Dictionary<int, Impulse> impulses = new Dictionary<int, Impulse>();

	public class Impulse {
		public int lastImpulse = -1;
		public bool shouldImpulse = false;
		public float energy = 0f;
	}

	[System.Serializable]
	public class Input {
		public string name;
		[Range(0,10)]
		public float energy;

		[HideInInspector]
		public float prevEnergy;

		[Range(-360, 360)]
		public float direction = 0;
		public bool enableAI;
	};

	public static void addPlayer(bool addToLeft, player p) {
		if(addToLeft) leftPlayers[p.number] = p;
		else rightPlayers[p.number] = p;
		self.rotations.Add(self.playerCount, 0);
		self.impulses.Add(self.playerCount, new Impulse());
		self.playerCount++;
	}

	public string readArduinoInputs(int timeout = 1) {
		stream.ReadTimeout = timeout;
		try { return stream.ReadLine(); }
		catch(System.TimeoutException) { return null; }
	}
	public void applyInput() {
		if(playerCount <= 0) return;
		string str = "";
		if (!DebugMode) {
			str = readArduinoInputs();
			if(str == null) return;
		}
		try { for(int i = 0; i < playerCount; i++) impulses[i].energy = 0; }
		catch(KeyNotFoundException) {}

		if(str.Length - 1 < 0) return;
		string[] players = str.Substring(0, str.Length - 1).Split('|');
		for(int i = 0; i < players.Length; i++) {
			if (!impulses.ContainsKey(i)) return;
			string[] val = players[i].Split(':');
			int rot = 0;
			if(!int.TryParse(val[0], out rot)) continue;
			rotations[i] = rot + ((i > 2)? -90 : 90);
			int imp = 0;
			if(!int.TryParse(val[1], out imp)) continue;
			if(imp != impulses[i].lastImpulse && !impulses[i].shouldImpulse) impulses[i].shouldImpulse = true;
			if(imp == impulses[i].lastImpulse && impulses[i].shouldImpulse) {	
				impulses[i].shouldImpulse = false;
				impulses[i].energy += 0.5f;
				continue;
			}
			impulses[i].lastImpulse = imp;
		}
		if (!DebugMode) {
				try {
					try {
						for(int i = 0; i < leftPlayers.Count; i++) {
							leftInput[i].direction = rotations[i];
							leftInput[i].energy += impulses[i].energy;
							//audioManager.PLAY_SOUND("CrankLong", leftPlayers[i].transform.position, impulses[i].energy);
						}
						for(int i = 0; i < rightPlayers.Count; i++) {
							rightInput[i].direction = rotations[i + 3];
							rightInput[i].energy += impulses[i + 3].energy;
							//audioManager.PLAY_SOUND("CrankLong", rightPlayers[i].transform.position, impulses[i + 3].energy);
						}
					}
					catch(KeyNotFoundException) {}
				} catch(System.IndexOutOfRangeException) {}
		}
	}

	void Awake() {
		int baudRate = 250000;
		if (!DebugMode) {
            string[] portNums = System.Text.RegularExpressions.Regex.Split(port.ToString(), @"\D+");
            stream = (int.Parse(portNums[1]) >= 10) ? new SerialPort("\\\\.\\" + port.ToString(), baudRate, Parity.None, 8, StopBits.One) :
                                                      new SerialPort(port.ToString(), baudRate, Parity.None, 8, StopBits.One);
			try {
				stream.Open();
				stream.ReadTimeout = 1;
			} catch(System.IO.IOException) { 
				DebugMode = true;
				KooKoo.print("Nothing found on Port " + port.ToString() + ", entering Debug mode.", KooKoo.MessageType.WARN);
			}
			cutscene = true;
		} else cutscene = Camera.main.GetComponent<Cutscene>().playCutscene;
		self = this;
		ball[] bs = FindObjectsOfType<ball>();
		foreach (ball b in bs) if (!b.trash) balls.Add(b.gameObject);
	}

	public void Ready() {
		gameManager.self.ballSpawner.GetComponent<ballSpawner>().ResetClock();
		cutscene = false;
		StopCoroutine("MovePlayer");
		StopCoroutine("AnimatePlayer");
		DeanimatePlayers();
		for(int i = 0; i < leftPlayers.Count; i++) {
			if(leftPlayers[i] == null) continue;
			leftPlayers[i].GetComponent<Rigidbody>().isKinematic = false;
			leftInput[i].direction = leftPlayers[i].transform.eulerAngles.y;
			leftInput[i].energy = 0;
		}
		for(int i = 0; i < rightPlayers.Count; i++) {
			if(rightPlayers[i] == null) continue;
			rightPlayers[i].GetComponent<Rigidbody>().isKinematic = false;
			rightInput[i].direction = rightPlayers[i].transform.eulerAngles.y;
			rightInput[i].energy = 0;
		}
	}

	public void CapturePlayers() {
		for(int i = 0; i < leftPlayers.Count; i++) {
			leftPlayers[i].transform.position = new Vector3(-15 - i * 2, leftPlayers[i].transform.position.y, -10);
			leftPlayers[i].GetComponent<Rigidbody>().isKinematic = true;
			leftPlayers[i].transform.Find("trail").gameObject.SetActive(false);
		}
		for(int i = 0; i < rightPlayers.Count; i++) {
			rightPlayers[i].transform.position = new Vector3(15 + i * 2, rightPlayers[i].transform.position.y, 10);
			rightPlayers[i].GetComponent<Rigidbody>().isKinematic = true;
			rightPlayers[i].transform.Find("trail").gameObject.SetActive(false);
		}
	}
	public void ReleasePlayers() {
		StartCoroutine("MovePlayer");
		for(int i = 0; i < leftPlayers.Count; i++) leftPlayers[i].transform.Find("trail").gameObject.SetActive(true);
		for(int i = 0; i < rightPlayers.Count; i++) rightPlayers[i].transform.Find("trail").gameObject.SetActive(true);
		for(int i = 0; i < 3; i++) audioManager.PLAY_STATIONARY("MachineLong", 0.5f, Random.Range(0.9f, 1.1f));
	}

	public void AnimatePlayers() {
		StopCoroutine("MovePlayer");
		StartCoroutine("AnimatePlayer");
	}
	private void DeanimatePlayers() {
		for(int i = 0; i < rightPlayers.Count; i++) rightPlayers[i].Deanimate();
		for(int i = 0; i < leftPlayers.Count; i++) leftPlayers[i].Deanimate();
	}

	private float animTime = 0;
	IEnumerator AnimatePlayer() {
		while(animTime < 20) {
			for(int i = 0; i < leftPlayers.Count; i++) leftPlayers[i].Animate();
			for(int i = 0; i < rightPlayers.Count; i++) rightPlayers[i].Animate();
			animTime += Time.deltaTime;
			yield return new WaitForSeconds(.05f);
		}
	}

	private float playerTime = 0;
	IEnumerator MovePlayer() {
		while(playerTime < 20) {
			float speed = 3;
			for(int i = 0; i < leftPlayers.Count; i++) {
				float targetX = -10 + i * 2;
				float targetY = 6 / leftPlayers.Count * i;
				float tar = Mathf.Lerp(leftPlayers[i].transform.position.x, targetX, Time.deltaTime * speed);
				float tarY = Mathf.Lerp(leftPlayers[i].transform.position.z, targetY, Time.deltaTime * speed);
				leftPlayers[i].transform.position = new Vector3(tar, leftPlayers[i].transform.position.y, tarY);
				leftPlayers[i].transform.LookAt(new Vector3(0, 0 ,0));
			}
			for(int i = 0; i < rightPlayers.Count; i++) {
				float targetX = 10 - i * 2;
				float targetY = 6 / rightPlayers.Count * i;
				float tar = Mathf.Lerp(rightPlayers[i].transform.position.x, targetX, Time.deltaTime * speed);
				float tarY = Mathf.Lerp(rightPlayers[i].transform.position.z, targetY, Time.deltaTime * speed);
				rightPlayers[i].transform.position = new Vector3(tar, rightPlayers[i].transform.position.y, tarY);
				rightPlayers[i].transform.LookAt(new Vector3(0, 0 ,0));
			}
			playerTime += Time.deltaTime;
			yield return new WaitForSeconds(.02f);
		}
	}

	void FixedUpdate() {
		if(!cutscene) {
			applyInput();
			for(int i = 0; i < leftPlayers.Count; i++) {
				if(leftInput[i] == null || leftPlayers[i] == null) continue;
				if(!leftPlayers[i].isStunned()) updatePlayer(leftInput[i], leftPlayers[i]);
			}
			for (int i = 0; i < rightPlayers.Count; i++) {
				if(rightInput[i] == null || rightPlayers[i] == null) continue;
				if(!rightPlayers[i].isStunned()) updatePlayer(rightInput[i], rightPlayers[i]);
			}
		}
	}

	public void tickPlayers() {
		for(int i = 0; i < leftPlayers.Count; i++) {
			if(leftPlayers[i] == null || leftInput[i] == null) continue;
			player p = leftPlayers[i];
			if(leftInput[i].energy > 0) {
				p.impulse(tickSpeed);
				leftInput[i].energy -= frequency;
			}
		}
		for(int i = 0; i < rightPlayers.Count; i++) {
			if(rightPlayers[i] == null || rightInput[i] == null) continue;
			player p = rightPlayers[i];
			if(rightInput[i].energy > 0) {
				p.impulse(tickSpeed);
				rightInput[i].energy -= frequency;
			}
		}
	}

	void updatePlayer(Input inp, player p) {
		if (inp.enableAI) {
			balls.Sort((a, b) => 1 - 2 * Random.Range(0, 1));
			List<GameObject> bcopy = new List<GameObject>(balls);
			if (!p.aiTarget || Random.value < .001f) {
				foreach (GameObject b in bcopy) {
					if (b) p.aiTarget = b.transform;
					else balls.Remove(b);
				}
			}
			p.transform.LookAt(p.aiTarget);
			Vector3 eulerAngles = p.transform.rotation.eulerAngles;
			inp.direction = eulerAngles.y / 10 + inp.direction * .9f;
			if (inp.energy < 1 && Random.value < .03f) inp.energy = Random.value * 10;
		}
		p.transform.rotation =  Quaternion.Euler(0, inp.direction, 0);
		p.energy = inp.energy;
		p.rotationSpeed = inp.energy * 50;
		if (p.leftTeam) p.rotationSpeed *= -1;

		//Charge
		if(inp.energy > inp.prevEnergy) {
			p.impulse(chargeSpeed * (inp.energy - inp.prevEnergy));
			audioManager.PLAY_SOUND("Hit", p.transform.position * 5, 15, Random.Range(1.2f, 2.5f));
		}
		inp.prevEnergy = inp.energy;
	}
}
