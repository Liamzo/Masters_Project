using UnityEngine;
using System.Collections;

public class KoboldMed : KoboldBase {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        HealthMax = 12;
        healthCur = HealthMax;
        Attack = 7;
        Defense = 2;
    }
}
