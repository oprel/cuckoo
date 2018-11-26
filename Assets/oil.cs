using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oil : MonoBehaviour {
	public Material puddle;
	private float time = 0;
	private bool spill = false;
	private float fade = 0;
	private bool end = false;
	private MeshRenderer rend;
	private Quaternion rot;


private void Awake(){
	rend = GetComponent<MeshRenderer>();
	rot = transform.rotation = Quaternion.Euler(90,Random.value*360,0);
}
	void Update () {
		time += Time.deltaTime;
		transform.rotation=rot;
		
		if(!spill) {
			if(time > 0.6f) {
				spill = true;
				GetComponent<Rigidbody>().useGravity = false;
				GetComponent<Rigidbody>().isKinematic = true;
				GetComponent<SphereCollider>().isTrigger = true;
			}
		}
		else {
			rend.material = puddle;
			transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x, 2, Time.deltaTime * 2), Mathf.Lerp(transform.localScale.y, 1.4f, Time.deltaTime * 2), Mathf.Lerp(transform.localScale.z, 2, Time.deltaTime * 2));
			gameObject.layer = 0;
		}
		if(time > 10 && !end) {
			fade = 1;
			end = true;
		}
		if(fade > 0) {
			fade -= Time.deltaTime;
			Material mat = rend.material;
			mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, fade);
		}
		else if(end) Destroy(gameObject);
	}

	void OnTriggerEnter(Collider col) {
		if(col.tag == "Player") col.GetComponent<player>().changeSpeed(2);
		else if(col.tag == "Ball") {
			col.GetComponent<Rigidbody>().mass = 0.1f;
			col.GetComponent<ball>().rotationSpeed *= 2;
		}
	}

	void OnTriggerExit(Collider col) {
		if(col.tag == "Player") col.GetComponent<player>().changeSpeed(1);
		else if(col.tag == "Ball") {
			col.GetComponent<Rigidbody>().mass = 1;
			col.GetComponent<ball>().resetRotation();
		}
	}
}
