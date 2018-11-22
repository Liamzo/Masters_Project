using UnityEngine;
using System.Collections;

public class ZombieLow : ZombieBase {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        HealthMax = 15;
        healthCur = HealthMax;
        Attack = 2;
        Defense = 1;
    }
}
