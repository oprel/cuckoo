using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour {
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
	[HideInInspector]
	public static Port port;

	public static SerialPort stream;

    void Start() {
		DontDestroyOnLoad(gameObject);
		StartCoroutine("ConnectInput");
		SceneManager.LoadScene("main_cutscene");
    }

    IEnumerator ConnectInput() {
		int baudRate = 250000;
		if(playerManager.self == null) yield return new WaitForSeconds(1);
		if (!playerManager.self.DebugMode) {
			string[] ports = System.Enum.GetNames(typeof(Port));
           	for(int i = 0; i < ports.Length; i++) {
				string port = ports[i];
				string[] portNums = System.Text.RegularExpressions.Regex.Split(port, @"\D+");
				stream = (int.Parse(portNums[1]) >= 10) ? new SerialPort("\\\\.\\" + port, baudRate, Parity.None, 8, StopBits.One) :
														new SerialPort(port.ToString(), baudRate, Parity.None, 8, StopBits.One);
				try {
					stream.Open();
					stream.ReadTimeout = 1;
					KooKoo.print("Inputs found on Port " + port, KooKoo.MessageType.WARN);
					break;
				} catch(System.IO.IOException) { 
					if(i >= ports.Length - 1) {
						KooKoo.print("Nothing found on any ports, entering debug mode.", KooKoo.MessageType.WARN);
						playerManager.self.DebugMode = true;
					}
				}
			}
			playerManager.self.SetCutsceneFlag(true);
		} else playerManager.self.SetCutsceneFlag(Camera.main.GetComponent<Cutscene>().playCutscene);
	}

	public static PlayerInput Connect() {
		return GameObject.FindObjectOfType<PlayerInput>();
	}

	public static string readArduinoInputs(int timeout = 1) {
		if(stream == null) return null;
		stream.ReadTimeout = timeout;
		try { return stream.ReadLine();}
		catch(System.TimeoutException) {return null;}
		catch(System.IO.IOException) {
			KooKoo.print("Connection lost!", KooKoo.MessageType.ERR);
			return null;
		}
		catch(System.UnauthorizedAccessException) {
			KooKoo.print("Connection lost!", KooKoo.MessageType.ERR); 
			return null;
		}
		catch(System.InvalidOperationException) {return null;}
	}
}
