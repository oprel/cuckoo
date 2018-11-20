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
	void Start () {
		rt = GetComponent<RectTransform>();
		originalPos = rt.anchoredPosition;
		rt.anchoredPosition += offscreenoffset;
	}


	public void speedChange(bool isUp){
		speedup.SetActive(isUp);
		speeddown.SetActive(!isUp);
		StartCoroutine(display());
		 if (isUp){
			 handle.speed = -1500;
		 }else{
			 handle.speed = 500;
		 }
	}

	private IEnumerator display(){
		timer=0;
		rt.anchoredPosition = originalPos+offscreenoffset;
		float travelTime = displayTime/8;
		
		while (timer <= travelTime) { 
			timer+= Time.deltaTime;
			float normalizedValue = timer/travelTime;
			Vector2 a = originalPos;
			a.x = Vector3.Slerp(a+offscreenoffset, a, normalizedValue).x; 
 			rt.anchoredPosition=a;
			yield return null;
		}
		while (timer<displayTime){
			timer+= Time.deltaTime;
			yield return null;
		}
		rt.anchoredPosition=originalPos+offscreenoffset;

	}
}
