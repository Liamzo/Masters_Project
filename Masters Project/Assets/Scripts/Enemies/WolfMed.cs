using UnityEngine;
using System.Collections;

public class WolfMed : WolfBase {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        HealthMax = 13;
        healthCur = HealthMax;
        Attack = 7;
        Defense = 1;
    }
}
