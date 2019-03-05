using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steamController : MonoBehaviour
{
    public float rateOverTimeMultiplier;
    private ParticleSystem[] systems;
    private ParticleSystem.EmissionModule[] emissions;
    // Start is called before the first frame update
    void Start()
    {
        systems = GetComponentsInChildren<ParticleSystem>();
        emissions = new ParticleSystem.EmissionModule[systems.Length];
        for (int i = 0; i < systems.Length; i++)
        {
            emissions[i] = systems[i].emission;
        }
        setRate(rateOverTimeMultiplier);
    }

    public void setRate(float rate){
        for (int i = 0; i < emissions.Length; i++)
        {
            emissions[i].rateMultiplier = rate;
        }
    }
}
