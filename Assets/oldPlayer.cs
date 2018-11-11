using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class oldPlayer : MonoBehaviour {

	public class direction{
		public float s= 0;
		public Vector2 d = Vector2.zero;
	}
	public int playerID = 1;
	public float accel;
	public float deaccel;
	public float speed;
	public Text[] storageDisplay;
	public direction[] dirs = new direction[4]; //up, right, down left
	private Rigidbody rb;
	private Vector3 startPos;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		for (int i = 0; i < dirs.Length; i++)
		{
			dirs[i]= new direction();
		}
		dirs[0].d= Vector2.up;
		dirs[1].d= Vector2.right;
		dirs[2].d= Vector2.down;
		dirs[3].d= Vector2.left;
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 playerInput = new Vector2(Input.GetAxis("Horizontal " + playerID),Input.GetAxis("Vertical " + playerID)) ;
		foreach (direction dir in dirs){
			Vector2 clip = playerInput * dir.d * Time.deltaTime * accel;
			dir.s += Mathf.Max(0,clip.x) + Mathf.Max(0,clip.y);

			if (dir.s>0){
				float s = Time.deltaTime * deaccel;
				dir.s-=s;
				s*=speed;
				rb.AddForce(s*dir.d.x,0,s*dir.d.y);
			}
		}

		//display
		for (int i = 0; i < dirs.Length; i++)
		{
            if (!storageDisplay[i]){break;}else{
                int s = (int) (dirs[i].s*1000);
                storageDisplay[i].text = s.ToString();
            }
		}
		if (transform.position.y<-2) Reset();
	}

	public void Reset(){
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.position = startPos;
			foreach (direction dir in dirs){
				dir.s = 0;
			}
	}
}
