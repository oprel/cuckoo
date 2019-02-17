using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour {
    public float lifeSpan = 2;
    private float time = 0;

    private MeshRenderer meshRenderer;
    private Material[] materials;
    private float baseA;
    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
    }

    void Update() {
        time += Time.deltaTime;
        foreach(Material mat in materials)  mat.color = new Color(mat.color.r, mat.color.r, mat.color.b, lifeSpan - time);
        if(time > lifeSpan) Destroy(gameObject);
    }
}
