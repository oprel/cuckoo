﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class edgePusher : MonoBehaviour
{
    public float force = 5;
    private SphereCollider collider;

    private void Awake() {
        collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other) {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb && other.tag != "Plank" && !rb.isKinematic){
            Vector3 diff = transform.position - other.transform.position;
            rb.AddForce(diff.normalized * force);
            
            player p = other.GetComponent<player>();
            if (p) p.suspendForwardMovement(.2f);
            StopAllCoroutines();
            StartCoroutine(recoil());
        }
    }

    private IEnumerator recoil(){
        for (float i = 0; i < 1; i+=1/50f)
        {
            collider.enabled = i>.2f;
            collider.radius = i;
            yield return null;
        }
    }
}
