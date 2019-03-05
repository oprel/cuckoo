using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playtestChanges : MonoBehaviour
{
    public static playtestChanges self;
    public float playerSpeedTick;
    public float playerSpeedCharge;
    // Start is called before the first frame update
    private void Awake(){
        self = this;
    }
}
