using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class speedChangeDisplay : MonoBehaviour {
	public float displayTime;
	private float timer;

	private Vector2 originalPos;
	private RectTransform rt;
	public Vector2 offscreenoffset;
	[Header("components")]
	public GameObject speedup;
	public GameObject speeddown;
	public autoRotate handle;
	
	// Use this for initialization
	void Start() {
		rt = GetComponent<RectTransform>();
		originalPos = rt.anchoredPosition;
		rt.anchoredPosition += offscreenoffset;
	}


	public void speedChange(bool isUp) {
		speedup.SetActive(isUp);
		speeddown.SetActive(!isUp);
		StartCoroutine(display());
		if (isUp) handle.speed = -1500;
		else handle.speed = 500;
	}

	private IEnumerator display() {
		timer = 0;
		rt.anchoredPosition = originalPos+offscreenoffset;
		float travelTime = displayTime / 4;
		
		while (timer <= travelTime) { 
			timer+= Time.deltaTime;
			float normalizedValue = timer/travelTime;
 			rt.anchoredPosition=Vector3.Lerp(originalPos+offscreenoffset, originalPos, normalizedValue); 
			yield return null;
		}
		while (timer>travelTime) {
			timer+= Time.deltaTime;
			yield return null;
			if (timer>=displayTime-travelTime){
				float normalizedValue = timer/travelTime;
 				rt.anchoredPosition=Vector3.Lerp(originalPos, originalPos+offscreenoffset, normalizedValue); 
			}
			if (timer>displayTime)break;
		}
	}
}
