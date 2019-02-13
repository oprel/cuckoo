using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoRotate : MonoBehaviour {
	public float speed;
	
	private float time = 0;

	private AudioSource src;

	void Start () {
		if (speed == 0) speed = Random.value * 20 - 10;

		src = gameObject.AddComponent<AudioSource>();
		src.playOnAwake = false;
		src.loop = false;
		src.spatialBlend = 1;
		src.clip = audioManager.GET_AUDIO("Twist");
		src.volume = 8;
		src.pitch = Random.Range(0.9f, 1.2f);
	}
	
	void Update () {
		transform.Rotate(Vector3.forward, speed * Time.deltaTime);

		time += Time.deltaTime;
		if(time > 1 / Mathf.Abs(speed)) {
			time = 0;
			src.Play();
		}
	}
}
