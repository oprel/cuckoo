using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    public float rotSpeed = 1f, posSpeed = 1f;
    public Vector3 posDir = new Vector3(0, 1, 0);
    public Vector3 rotDir = new Vector3(1, 0, 0);

    private Vector3 pos, rot;

    void Start() {
        pos = transform.position;
        rot = transform.eulerAngles;
    }

    void FixedUpdate() {
        Vector3 lerp = new Vector3(Mathf.Sin(Time.time * posSpeed)*posDir.x, Mathf.Cos(Time.time * posSpeed)*posDir.y, Mathf.Sin(Time.time * posSpeed)*posDir.z);
        Vector3 rotLerp = new Vector3(Mathf.Sin(Time.time * rotSpeed)*rotDir.x, Mathf.Cos(Time.time * rotSpeed)*rotDir.y, Mathf.Sin(Time.time * rotSpeed)*rotDir.z);

        transform.position = new Vector3(pos.x, pos.y, pos.z) + lerp;
        transform.rotation = Quaternion.Euler(rot.x + rotLerp.x, rot.y + rotLerp.y, rot.z + rotLerp.z);
    }
}
