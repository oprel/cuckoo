using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;

public class playerManager : MonoBehaviour {
	public static Dictionary<int, player> leftPlayers = new Dictionary<int, player>();
	public static Dictionary<int, player> rightPlayers = new Dictionary<int, player>();
	public static playerManager self;
	public List<GameObject> balls = new List<GameObject>();
	
	public GameObject textPrefab, oilPrefab;

	private int playerCount;

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

	[Header("TeamPlay Settings")]
	[SerializeField]
	public Team[] teams;
	public Team GetTeam(string color) {
		foreach(Team t in teams) if(t.name.ToLower() == color.ToLower()) return t;
		return null;
	}

	[Space(10)]
	public bool DebugMode;
	public float frequency;
	public float speed;
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
		[Range(-360, 360)]
		public float direction = 0;
		[HideInInspector]
		public float timer;
		public bool enableAI;
	};

	public static void addPlayer(bool addToLeft, player p) {
		if(addToLeft) leftPlayers[p.number] = p;
		else rightPlayers[p.number] = p;
		self.playerCount++;
	}

	public string readArduinoInputs(int timeout = 1) {
		stream.ReadTimeout = timeout;
		try { return stream.ReadLine(); }
		catch(System.TimeoutException) { return null; }
	}

	public void applyInput() {
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
			string[] val = players[i].Split(':');
			int rot = 0;
			if(!int.TryParse(val[0], out rot)) continue;
			rotations[i] = rot;
			
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
							audioManager.PLAY_SOUND("CrankLong", leftPlayers[i].transform.position, impulses[i].energy);
						}
						for(int i = 0; i < rightPlayers.Count; i++) {
							rightInput[i].direction = rotations[i + 3];
							rightInput[i].energy += impulses[i + 3].energy;
							audioManager.PLAY_SOUND("CrankLong", rightPlayers[i].transform.position, impulses[i + 3].energy);
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
		}
		self = this;
		ball[] bs = FindObjectsOfType<ball>();
		foreach (ball b in bs) if (!b.trash) balls.Add(b.gameObject);
	}

	void Start() {
		for(int i = 0; i < playerCount; i++) {
			rotations.Add(i, 0);
			impulses.Add(i, new Impulse());
		}
	}

	void FixedUpdate() {
		applyInput();

		for(int i = 0; i < leftPlayers.Count; i++) {
			player p = leftPlayers[i];
			Input inp = leftInput[i];
			if(p) updatePlayer(p,inp);
		}
		for (int i = 0; i < rightPlayers.Count; i++) {
			player p = rightPlayers[i];
			Input inp = rightInput[i];
			if(p) updatePlayer(p,inp);
		}
	}

	public void tickPlayers() {
		for(int i = 0; i < leftPlayers.Count; i++) {
			player p = leftPlayers[i];
			if(leftInput[i].energy > 0) p.impulse(speed / 5);
		}
		for(int i = 0; i < rightPlayers.Count; i++) {
			player p = rightPlayers[i];
			if(rightInput[i].energy > 0) p.impulse(speed / 5);
		}
	}

	void updatePlayer(player p, Input inp) {
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
		
		p.rotationSpeed = inp.energy * 50;
		if (p.leftTeam) p.rotationSpeed *= -1;
		if(inp.energy > 0) {
			inp.timer += Time.deltaTime;
			if (inp.timer > frequency) {
				p.impulse(speed);
				inp.timer = 0;
				inp.energy--;
			}
		}
	}
}
