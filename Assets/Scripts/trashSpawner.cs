﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trashSpawner : MonoBehaviour {
    public GameObject[] trashPrefab;

    private GameObject dropPoint;
    private float spawnDelay = 0;
    private int hit = 0;

    private float shake = 0;
    private float shakeDelay = 0;

    private GameObject item;

    void Start() {
        dropPoint = transform.Find("Drop").gameObject;
    }

    void FixedUpdate() {
        if(shakeDelay > 0) shakeDelay -= Time.deltaTime;
        if(spawnDelay > 0) spawnDelay -= Time.deltaTime;
        if(item == null && spawnDelay <= 0) item = SpawnItem();
        if(shake > 0) {
            shake -= Time.deltaTime;
            transform.rotation = Quaternion.Euler(Mathf.Sin(shake*shake), Mathf.Cos(shake*shake*5)*2, Mathf.Sin(shake*shake));
        }
        else transform.rotation = Quaternion.Euler(Mathf.LerpAngle(transform.rotation.x, 0, Time.deltaTime*2), Mathf.LerpAngle(transform.rotation.y, 0, Time.deltaTime*2), Mathf.LerpAngle(transform.rotation.z, 0, Time.deltaTime*2));
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Hitter" && item != null && shakeDelay <= 0) {
            trashManager.CountTrash();
            if(other.GetComponentInParent<player>().energy > trashManager.self.energyUntilHit && trashManager.trashAmount < trashManager.self.maxTrash) {// && Mathf.Abs(other.transform.localEulerAngles.y) < 45) {
                hit += 3;
                shakeDelay = 3;
                audioManager.PLAY_SOUND("Hit", transform.position, 1200, Random.Range(0.9f, 1.2f));
                audioManager.PLAY_SOUND("Collide", transform.position, 1200, Random.Range(0.9f, 1.2f));
                Shake();
            }
        }
    }

    private void Shake() {
        shake = 3;
        Camera.main.GetComponent<cameraShake>().ShakeCamera(0.25f, 0.05f);
        if(hit >= trashManager.self.hitsUntilDrop) DropItem();
    }

    public void scored() {
        trashManager.CountTrash();
        if(trashManager.trashAmount >= trashManager.self.maxTrash) return;
        shakeDelay = 5;
        hit++;
        if(hit >= trashManager.self.hitsUntilDrop) DropItem();
    }

    private GameObject SpawnItem() {
        GameObject go = Instantiate(trashPrefab[Random.Range(0, trashPrefab.Length)]);
        go.GetComponent<Rigidbody>().isKinematic = true;
        go.transform.position = dropPoint.transform.position;
        go.transform.SetParent(transform);
        audioManager.PLAY_SOUND("Plop", transform.position, 1200, Random.Range(0.9f, 1.2f));
        return go;
    }

    private void DropItem() {
        if(item == null) return;
        item.transform.SetParent(null);
        item.GetComponent<Rigidbody>().isKinematic = false;
        Vector3 dir = -item.transform.position.normalized * Random.Range(100,300);
        dir.y = 0;
        item.GetComponent<Rigidbody>().AddForce(dir);
        item = null;
        spawnDelay = 10;
        hit = 0;
    }

    private void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(transform.position,1);
    }
}
