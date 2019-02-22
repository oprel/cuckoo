using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class antiStack : MonoBehaviour
{
    private float minHeightDifference = .1f;
    private float pushForce = 100f;

    private void OnCollisionEnter(Collision other) {
        Vector3 dir = other.transform.position-transform.position;
        if (dir.y<minHeightDifference) return;
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb && rb.GetComponent<antiStack>()){
            dir.y = 0;
            rb.AddForce(dir.normalized * pushForce);
        }
    }
}
