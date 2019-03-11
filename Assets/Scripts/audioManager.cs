using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour {
    [Header("Music")]
	[Range(0, 10)]
	public float masterVolume = 1;
	public bool muteMusic = false;
    public AudioClip mainBass, mainDrums, mainMelody;

	public class Voice {
		public float targetVol = 0, changeSpeed = 0.5f;
		public float currentVol = 0.1f;

		private AudioSource source;

		public Voice(AudioSource source, float baseVol) {
			this.source = source;
			currentVol = baseVol;
		}

		public void Update() {
			currentVol = Mathf.Lerp(currentVol, Mathf.Clamp(targetVol, 0, audioManager.instance.masterVolume), Time.deltaTime * changeSpeed);
			source.volume = currentVol * audioManager.instance.masterVolume;
		}
	}
	public Voice BASS, DRUMS, MELODY;
	[HideInInspector]
	public Voice[] voices;

	[HideInInspector]
	public AudioSource mainBassSrc, mainDrumsSrc, mainMelodySrc;
	public static audioManager instance;

	[Header("Sounds")]
	public AudioClip[] sounds;
	private string[] names;

	private float steamDelay = 0;
	private bool boostMusic = false;

	void Start () {
		instance = this;

		//Music
		mainBassSrc = gameObject.AddComponent<AudioSource>();
		mainDrumsSrc = gameObject.AddComponent<AudioSource>();
		mainMelodySrc = gameObject.AddComponent<AudioSource>();
		mainBassSrc.clip = mainBass;
		mainDrumsSrc.clip = mainDrums;
		mainMelodySrc.clip = mainMelody;
		mainBassSrc.loop = mainDrumsSrc.loop = mainMelodySrc.loop = true;
		mainBassSrc.playOnAwake = mainDrumsSrc.playOnAwake = mainMelodySrc.playOnAwake = false;
		mainBassSrc.Play();
		mainDrumsSrc.Play();
		mainMelodySrc.Play();
		BASS = new Voice(mainBassSrc, 0.1f);
		DRUMS = new Voice(mainDrumsSrc, 0.1f);
		MELODY = new Voice(mainMelodySrc, 0f);
		voices = new Voice[]{BASS, DRUMS, MELODY};

		FADE_MUSIC(0.07f, BASS, 0.8f);
		FADE_MUSIC(0.03f, DRUMS, 0.5f);

		//Sounds
		names = new string[sounds.Length];
		for(int i = 0; i < sounds.Length; i++) names[i] = sounds[i].name;
	}

	public void BoostMusic() {
		boostMusic = true;
		muteMusic = false;
		mainBassSrc.volume = mainDrumsSrc.volume = 2;
		mainMelodySrc.volume = 2.5f;
		mainBassSrc.loop = mainDrumsSrc.loop = mainMelodySrc.loop = false;
	}

	void FixedUpdate() {
		if(!boostMusic) mainBassSrc.mute = mainDrumsSrc.mute = mainMelodySrc.mute = muteMusic;
		mainBassSrc.timeSamples = mainMelodySrc.timeSamples = mainDrumsSrc.timeSamples;

		if(!boostMusic) foreach(Voice v in voices) v.Update();

		//Ambient level sounds
		if(steamDelay > 0) steamDelay -= Time.deltaTime;
		if(Random.Range(0, 400) <= 5) {
			PLAY_SOUND("SteamLong", Camera.main.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-1, 1), Random.Range(-1, 1)), 0.2f, Random.Range(0.9f, 1.5f));
			steamDelay = 1f;
		}
	}

	public static void FADE_MUSIC(float val, Voice voice, float change = 0.5f) {
		voice.changeSpeed = change;
		voice.targetVol = val;
	}

	public static void PLAY_STATIONARY(string name, float volume = 1, float pitch = 1) {
		PLAY_SOUND(name, Camera.main.transform.position, volume, pitch);
	}

	public static void PLAY_CAM(string name, float volume = 1, float pitch = 1) {
		if(instance == null) return;
		for(int i = 0; i < instance.sounds.Length; i++) {
			if(name == instance.names[i]) {
				PlayClipAt(instance.sounds[i], "", volume * instance.masterVolume, pitch);
				return;
			}
		}
		KooKoo.print("Could not find '" + name + "' sound file!", KooKoo.MessageType.ERR);
	}

	public static void PLAY_SOUND(string name, Vector3 pos, float volume = 1, float pitch = 1) {
		if(instance == null) return;
		for(int i = 0; i < instance.sounds.Length; i++) {
			if(name == instance.names[i]) {
				PlayClipAt(instance.sounds[i], pos, volume * instance.masterVolume, pitch);
				return;
			}
		}
		KooKoo.print("Could not find '" + name + "' sound file!", KooKoo.MessageType.ERR);
	}

	public static AudioClip GET_AUDIO(string name) {
		if(instance == null) return null;
		for(int i = 0; i < instance.sounds.Length; i++) {
			if(instance.names[i] == name) return instance.sounds[i];
		}
		return null;
	}

	private static AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float vol, float pitch) {
		GameObject tempGO = new GameObject("Sound: " + clip.name);
		tempGO.transform.position = pos;
		var aSource = tempGO.AddComponent<AudioSource>();
		aSource.clip = clip;
		aSource.pitch = pitch;
		aSource.spatialBlend = 1;
		aSource.volume = vol;
		aSource.Play();
		Destroy(tempGO, clip.length);
		return aSource;
	}

	private static AudioSource PlayClipAt(AudioClip clip, string pos, float vol, float pitch) {
		GameObject tempGO = new GameObject("Sound: " + clip.name);
		var aSource = tempGO.AddComponent<AudioSource>();
		aSource.clip = clip;
		aSource.pitch = pitch;
		aSource.spatialBlend = 0;
		aSource.volume = vol;
		aSource.Play();
		Destroy(tempGO, clip.length);
		return aSource;
	}
}
