using UnityEngine;
using System.Collections;

public class KoboldLow : KoboldBase {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        HealthMax = 9;
        healthCur = HealthMax;
        Attack = 3;
        Defense = 1;
    }
}
