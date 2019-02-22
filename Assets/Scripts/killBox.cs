using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        player p = other.GetComponent<player>();
        if (p) p.Reset();
        ball b = other.GetComponent<ball>();
        if (b) b.Destroy();
    }
}
