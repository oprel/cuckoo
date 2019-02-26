using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trashSpawner : MonoBehaviour {
    public GameObject[] trashPrefab;

	private static int trashAmount;
    public int maxTrash = 3;

    public float energyUntilHit = 2;
    public int hitsUntilDrop = 2;
    private GameObject dropPoint;
    private float spawnDelay = 0;
    private int hit = 0;

    private float shake = 0;
    private float shakeDelay = 0;

    private GameObject item;

    void Start() {
        dropPoint = transform.Find("Drop").gameObject;
        trashAmount = CountTrash();
    }

    private int CountTrash() {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Trash");
		int num = 0;
		foreach(GameObject ball in balls) num++;
		return num;
    }

    void FixedUpdate() {
        trashAmount = CountTrash();
        if(shakeDelay > 0) shakeDelay -= Time.deltaTime;
        if(spawnDelay > 0) spawnDelay -= Time.deltaTime;
        if(item == null && spawnDelay <= 0 && trashAmount < maxTrash) item = SpawnItem();
        if(shake > 0) {
            shake -= Time.deltaTime;
            transform.rotation = Quaternion.Euler(Mathf.Sin(shake*shake), Mathf.Cos(shake*shake*5)*2, Mathf.Sin(shake*shake));
        }
        else transform.rotation = Quaternion.Euler(Mathf.LerpAngle(transform.rotation.x, 0, Time.deltaTime*2), Mathf.LerpAngle(transform.rotation.y, 0, Time.deltaTime*2), Mathf.LerpAngle(transform.rotation.z, 0, Time.deltaTime*2));
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Hitter" && item != null && shakeDelay <= 0 && trashAmount < maxTrash) {
            if(other.GetComponentInParent<player>().energy > energyUntilHit && Mathf.Abs(other.transform.localEulerAngles.y) < 45) {
                hit++;
                shakeDelay = 5;
                audioManager.PLAY_SOUND("Hit", transform.position, 1200, Random.Range(0.9f, 1.2f));
                audioManager.PLAY_SOUND("Collide", transform.position, 1200, Random.Range(0.9f, 1.2f));
                Shake();
            }
        }
    }

    private void Shake() {
        shake = 3;
        Camera.main.GetComponent<cameraShake>().ShakeCamera(0.25f, 0.05f);
        if(hit >= hitsUntilDrop) DropItem();
    }

    public void scored() {
        shakeDelay = 5;
        hit++;
        if(hit >= hitsUntilDrop) DropItem();
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
        dir.y = 0 ;
        item.GetComponent<Rigidbody>().AddForce(dir);
        item = null;
        spawnDelay = 10;
        hit = 0;
    }

    private void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(transform.position,1);
    }
}
