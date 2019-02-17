using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoRotate : MonoBehaviour {
	public float speed;
	private float time = 0;

	private AudioSource src;
	public bool goal = false;
	public bool right = false;

	void Start () {
		if (speed == 0) speed = Random.value * 20 - 10;

		src = gameObject.AddComponent<AudioSource>();
		src.playOnAwake = false;
		src.loop = false;
		src.spatialBlend = 1;
		src.clip = audioManager.GET_AUDIO("Twist");
		src.volume = 4;
		if(goal) {
			src.pitch = Random.Range(0.5f, 0.85f);
			src.volume = 0.3f;
			src.loop = true;
		}
	}
	
	void Update () {
		if(src.clip == null && goal) src.clip = audioManager.GET_AUDIO("Turn");
		transform.Rotate(Vector3.forward, speed * ((goal)? 0.4f : 1) * Time.deltaTime);

		time += Time.deltaTime;
		if(time > 1 / Mathf.Abs(speed) && !goal) {
			time = 0;
			src.pitch = Random.Range(0.9f, 1.2f);
			src.Play();
		}
		if(Mathf.Abs(speed) > 0 && goal) {
			float val;
			if(right) val = gameManager.self.scoreRight / 10f + 0.4f;
			else val = gameManager.self.scoreLeft / 10f + 0.4f;
			src.pitch = Mathf.Clamp(val, 0, 1.5f);
			if(!src.isPlaying) src.Play();
		}
		if(Mathf.Abs(speed) == 0) src.Stop();
	}
}
