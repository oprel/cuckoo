using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour {
    public GameObject target;
	public float baseScale = 0.2f;
	public AnimationCurve animationCurve;
	public float animationSpeed, extensionLength;

	private Transform armSprite;
	private Vector3 armBase;
	private CharacterJoint holdJoint;

	void Start () {
		armSprite = GetComponentInChildren<SpriteRenderer>().transform;
		holdJoint = GetComponentInChildren<CharacterJoint>();
		armBase = new Vector3(armSprite.localScale.x, baseScale, armSprite.localScale.z);
		armSprite.localScale = armBase;
	}

	public void Activate(GameObject target) {
		this.target = target;
		gameObject.SetActive(true);
		this.target.GetComponent<player>().cutscene = true;
		target.transform.position = holdJoint.transform.position; 
		Rigidbody rb = target.GetComponent<Rigidbody>();
		holdJoint.connectedBody = rb;
		StartCoroutine("extendArm");
	}

	IEnumerator extendArm() {
		while(true) {
			target.transform.rotation = Quaternion.Euler(new Vector3(Random.value * 360, 90, -90));
			endingManager.moveDoors(true);
			for (float i = 0; i < 1; i+=1f/animationSpeed)
			{
				Debug.Log(i);
				float tar = animationCurve.Evaluate(i)* extensionLength + baseScale;
				armSprite.localScale = new Vector3(armBase.x, tar, armBase.z);
				yield return null;
			}
			endingManager.moveDoors(false);
			yield return new WaitForSeconds(.5f);
		}
	}

}
