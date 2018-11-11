using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballSpawner : MonoBehaviour {

	public GameObject ballPrefab;
	public float frequency;
	public float radius;
	private float timer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timer+= Time.deltaTime;
		if (timer>frequency){
			timer=0;
			Vector3 pos = Random.insideUnitSphere * radius;
			pos.y=0;
			Instantiate(ballPrefab,transform.position+pos,transform.rotation);
		}
	}

	private void OnDrawGizmosSelected(){
		Gizmos.DrawWireSphere(transform.position,radius);
	}
}
