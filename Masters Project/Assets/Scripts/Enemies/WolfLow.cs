using UnityEngine;
using System.Collections;

public class WolfLow : WolfBase {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        HealthMax = 8;
        healthCur = HealthMax;
        Attack = 4;
        Defense = 0;
    }
}
